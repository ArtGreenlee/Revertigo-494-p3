using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private List<GameObject> targets;
    private List<float> targetCooldownTimer;
    EnemyStorage enemyStorage;



    // Start is called before the first frame update
    void Start()
    {
        targets = new List<GameObject>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        targetCooldownTimer = new List<float>();
        for (int i = 0; i < numTargets; i++)
        {
            targetCooldownTimer.Add(0);
        }
    }

    private void setNewTarget()
    {
        if (targetSelection == TargetSelectionType.closest)
        {
            float minDistance = range;
            GameObject tempTarget = null;
            foreach (GameObject enemy in enemyStorage.enemies)
            {
                if (!targets.Contains(enemy))
                {
                    float curDistance = Vector3.Distance(enemy.transform.position, transform.position);
                    if (curDistance < minDistance)
                    {
                        tempTarget = enemy;
                        minDistance = curDistance;
                    }
                }
            }
            if (minDistance < range && tempTarget != null && !targets.Contains(tempTarget))
            {
                targets.Add(tempTarget);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (!enemyStorage.enemyIsAlive(targets[i]) ||
            Vector3.Distance(targets[i].transform.position, transform.position) >= range)
            {
                targets.RemoveAt(i);
                i--;
            }
            if (Time.time - targetCooldownTimer[i] > cooldown)
            {
                targetCooldownTimer[i] = Time.time;
                GameObject tempBullet = Instantiate(bullet, transform.position,
                    new Quaternion());
                tempBullet.GetComponent<Rigidbody>().velocity = (targets[i].transform.position - transform.position).normalized * bulletSpeed;
            }
        }
        if (targets.Count < numTargets)
        {
            setNewTarget();
        }
    }
}
