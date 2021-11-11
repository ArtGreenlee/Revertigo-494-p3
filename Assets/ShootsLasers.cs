using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShootsLasers : MonoBehaviour
{
    public enum TargetSelectionType { closest, weakest, farthest, strongest }
    private TowerStats towerStats;
    public TargetSelectionType targetSelection;
    public GameObject laser;
    private Dictionary<GameObject, float> targets;
    private EnemyStorage enemyStorage;
    private float playerShootCooldownUtility;

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
        playerShootCooldownUtility = 0;
        targets = new Dictionary<GameObject, float>();
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
                    targets.Add(addEnemy, 0);
                }
            }
            List<GameObject> enemyRemovalBuffer = new List<GameObject>();
            List<GameObject> tempTargets = targets.Keys.ToList();
            


            foreach (GameObject target in tempTargets)
            {
                if (target != null &&
                    enemyStorage.enemyIsAlive(target))
                {
                    if (Time.time - targets[target] > towerStats.getCooldown())
                    {
                        shootLaser(target);
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

    public void shootLaser(GameObject target)
    {
        if (!towerStats.attachedToPlayer || (Time.time - playerShootCooldownUtility > towerStats.getCooldown()))
        {
            playerShootCooldownUtility = Time.time;
            GameObject tempLaser = Instantiate(laser, transform.position, new Quaternion());
            tempLaser.GetComponent<LaserController>().towerStats = towerStats;
            tempLaser.GetComponent<LaserController>().target = target;
        }
    }
}
