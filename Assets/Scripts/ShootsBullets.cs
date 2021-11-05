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

    private void Awake()
    {
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        towerStats = GetComponent<TowerStats>();
    }
    // Start is called before the first frame update
    void Start()
    {
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
        foreach (GameObject target in tempTargets)
        {
            if (target != null &&
                enemyStorage.enemyIsAlive(target) &&
                (target.transform.position - transform.position).sqrMagnitude < towerStats.range * towerStats.range)
            {
                if (Time.time - targets[target] > towerStats.cooldown)
                {
                    //shoot
                    GameObject tempBullet = Instantiate(bullet, transform.position, new Quaternion());
                    tempBullet.GetComponent<Rigidbody>().velocity = (target.transform.position - transform.position).normalized * bulletSpeed;
                    tempBullet.GetComponent<BulletController>().towerStats = towerStats;
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
