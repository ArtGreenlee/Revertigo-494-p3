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
    public GameObject bullet;
    public float bulletSpeed;
    private Dictionary<GameObject, float> targets;
    private EnemyStorage enemyStorage;
    private float playerShootCooldownUtility;
    private Rigidbody rb;
    private float rangeSquared;

    public Vector3 snapPosition;

    private ObjectPooler objectPooler;

    private void Awake()
    {
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
            GameObject tempBullet = objectPooler.getObjectFromPool("Bullet", transform.position, Quaternion.identity);
            tempBullet.GetComponent<Rigidbody>().velocity = direction.normalized * bulletSpeed;
            tempBullet.GetComponent<BulletController>().towerStats = towerStats;

            if (!towerStats.attachedToPlayer && snapPosition != Vector3.zero)
            {
                AudioSource.PlayClipAtPoint(towerShootSFX, transform.position, 8);
                rb.AddForce(direction.normalized * bulletSpeed * -.1f, ForceMode.Impulse);
            }
        }
    }
}
