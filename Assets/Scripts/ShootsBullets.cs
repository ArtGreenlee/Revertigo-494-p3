using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShootsBullets : MonoBehaviour
{
    public enum TargetSelectionType { closest, weakest, farthest, strongest }

    public TargetSelectionType targetSelection;
    public bool slowsEnemy;
    public bool canCriticallyHit;
    public bool aoe;
    public float range;
    public int numTargets;
    public float bulletSpeed;
    public float damageMin;
    public float damageMax;
    public float critChance;
    public float critMult;
    public float slowPercent;
    public float slowDuration;
    public float aoe_range;
    public float aoe_damage;
    public float cooldown;
    public GameObject bullet;
    private Dictionary<GameObject, float> targets;
    EnemyStorage enemyStorage;

    // Start is called before the first frame update
    void Start()
    {
        targets = new Dictionary<GameObject, float>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targets.Count < numTargets) {
            //look for a new target
            float minDistance = float.MaxValue;
            GameObject addEnemy = null;
            foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, range))
            {
                if (!targets.ContainsKey(enemy))
                {
                    float curDistance = Vector3.Distance(transform.position, enemy.transform.position);
                    if (curDistance < minDistance)
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
                Vector3.Distance(target.transform.position, transform.position) < range)
            {
                if (Time.time - targets[target] > cooldown)
                {
                    //shoot
                    GameObject tempBullet = Instantiate(bullet, transform.position, new Quaternion());
                    tempBullet.GetComponent<Rigidbody>().velocity = (target.transform.position - transform.position).normalized * bulletSpeed;
                    tempBullet.GetComponent<BulletController>().parent = this;
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
