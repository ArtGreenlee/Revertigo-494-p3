using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShootsBullets : MonoBehaviour
{
    public enum TargetSelectionType { closest, weakest, farthest, strongest }
    private TowerStats towerStats;
    public TargetSelectionType targetSelection;
    public AudioClip towerShootSFX;
    public float towerShootLow = 0.01f;
    public float towerShootHigh = 0.05f;
    private AudioSource source;
    public GameObject bullet;
    public float bulletSpeed;
    private Dictionary<GameObject, float> targets;
    private EnemyStorage enemyStorage;
    private float playerShootCooldownUtility;
    private Rigidbody rb;
    private float rangeSquared;
    public GameObject shootEffect;

    public Vector3 snapPosition;

    private ObjectPooler objectPooler;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        objectPooler = ObjectPooler.Instance;
        enemyStorage = EnemyStorage.instance;
        towerStats = GetComponent<TowerStats>();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerShootCooldownUtility = 0;
        targets = new Dictionary<GameObject, float>();
        rangeSquared = towerStats.range * towerStats.range;
    }

    // Update is called once per frame
    void Update()
    {
        if (!towerStats.attachedToPlayer)
        {
            if (targets.Count < towerStats.numTargets)
            {
                //look for a new target
                float minDistance = float.MaxValue;
                GameObject addEnemy = null;
                foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, towerStats.range))
                {
                    if (!targets.ContainsKey(enemy))
                    {
                        float curDistance = (transform.position - enemy.transform.position).sqrMagnitude;
                        if (curDistance < minDistance * minDistance)
                        {
                            addEnemy = enemy;
                            minDistance = curDistance;
                        }
                    }
                }
                if (addEnemy != null)
                {
                    targets.Add(addEnemy, Time.time);
                }
            }
            List<GameObject> enemyRemovalBuffer = new List<GameObject>();
            List<GameObject> tempTargets = targets.Keys.ToList();
            
            foreach (GameObject target in tempTargets)
            {
                if (target != null &&
                    enemyStorage.enemyIsAlive(target) &&
                    (transform.position - target.transform.position).sqrMagnitude <= rangeSquared)
                {
                    if (Time.time - targets[target] > towerStats.getCooldown())
                    {
                        shootBullet(target.transform.position - transform.position);
                        targets[target] = Time.time;
                    }
                    /*else if (!towerStats.specialTower && targets.Count == 1 && Time.time - playerShootCooldownUtility > towerStats.getCooldown() / 1.5f)
                    {
                        rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                                       Quaternion.LookRotation(target.transform.position - transform.position),
                                       10 * Time.deltaTime));
                    }
                    else if (rb.angularVelocity == Vector3.zero)
                    {
                        rb.angularVelocity = Random.onUnitSphere * .5f;
                    }*/
                }
                else
                {
                    enemyRemovalBuffer.Add(target);
                }
            }
            foreach (GameObject enemy in enemyRemovalBuffer)
            {
                targets.Remove(enemy);
            }

            
        }
        
        
    }

    private void FixedUpdate()
    {
        if (transform.position != snapPosition && snapPosition != Vector3.zero)
        {
            rb.AddForce((snapPosition - transform.position) * 10);
            //transform.position = Vector3.Lerp(transform.position, snapPosition, Time.deltaTime * 10);
        }

    }

    public void shootBullet(Vector3 direction)
    {
        if (!towerStats.attachedToPlayer || (Time.time - playerShootCooldownUtility > towerStats.getCooldown()))
        {
            playerShootCooldownUtility = Time.time;
            GameObject tempBullet = objectPooler.getObjectFromPool("Bullet", transform.position, Quaternion.LookRotation(direction));
            if (!towerStats.attachedToPlayer)
            {
                Instantiate(shootEffect, transform.position, Quaternion.identity);
            }
            tempBullet.GetComponent<Rigidbody>().velocity = direction.normalized * bulletSpeed * Random.Range(.9f, 1.1f);
            tempBullet.GetComponent<BulletController>().towerStats = towerStats;
            tempBullet.GetComponent<MeshRenderer>().material.color = towerStats.trailRendererColor;
            if (!towerStats.attachedToPlayer && snapPosition != Vector3.zero)
            {
                source.PlayOneShot(towerShootSFX, Random.Range(towerShootLow, towerShootHigh));
                rb.AddForce(direction.normalized * bulletSpeed * -.1f, ForceMode.Impulse);
            }
        }
    }
}
