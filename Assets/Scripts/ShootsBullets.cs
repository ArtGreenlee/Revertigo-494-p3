using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShootsBullets : MonoBehaviour
{
    public enum TargetSelectionType { closest, weakest, farthest, strongest }
    private TowerStats towerStats;
    public TargetSelectionType targetSelection;
    public GameObject bullet;
    public float bulletSpeed;
    private Dictionary<GameObject, float> targets;
    EnemyStorage enemyStorage;
    private float playerShootCooldownUtility;

    private ObjectPooler objectPooler;

    private void Awake()
    {
        objectPooler = ObjectPooler.Instance;
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        towerStats = GetComponent<TowerStats>();
    }
    // Start is called before the first frame update
    void Start()
    {
        playerShootCooldownUtility = 0;
        targets = new Dictionary<GameObject, float>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targets.Count < towerStats.numTargets) {
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
        if (towerStats.automaticallyShoots)
        {
            foreach (GameObject target in tempTargets)
            {
                if (target != null &&
                    enemyStorage.enemyIsAlive(target) &&
                    (target.transform.position - transform.position).sqrMagnitude < towerStats.range * towerStats.range)
                {
                    if (Time.time - targets[target] > towerStats.cooldown)
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

    public void shootBullet(Vector3 direction)
    {
        if (Time.time - playerShootCooldownUtility > towerStats.cooldown)
        {
            playerShootCooldownUtility = Time.time;
            GameObject tempBullet = objectPooler.getObjectFromPool("Bullet", transform.position, new Quaternion());
            tempBullet.GetComponent<Rigidbody>().velocity = direction.normalized * bulletSpeed;
            tempBullet.GetComponent<BulletController>().towerStats = towerStats;
        }
       
    }
}
