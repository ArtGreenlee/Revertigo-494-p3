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
    private Dictionary<GameObject, float> targets;
    EnemyStorage enemyStorage;

    // Start is called before the first frame update
    void Start()
    {
        targets = new Dictionary<GameObject, float>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
    }

    private void setTargets()
    {
        foreach (GameObject enemy in enemyStorage.enemies)
        {
            float minDistance = range;
            GameObject tempTarget = null;
            if (!targets.ContainsKey(enemy))
            {
                float curDistance = Vector3.Distance(enemy.transform.position, transform.position);
                if (curDistance < minDistance)
                {
                    tempTarget = enemy;
                    minDistance = curDistance;
                }
            }
            if (minDistance < range)
            {
                targets.Add(tempTarget, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject target in enemyStorage.enemies) //bad bad bad hwy why why TODO
        {
            if (targets.ContainsKey(target))
            {
                if (!enemyStorage.enemyIsAlive(target) ||
                    Vector3.Distance(target.transform.position, transform.position) >= range)
                {
                    targets.Remove(target);
                }
                else if (Time.time - targets[target] > cooldown)
                {
                    targets[target] = Time.time;
                    
                    GameObject tempBullet = Instantiate(bullet, transform.position, new Quaternion());
                    tempBullet.GetComponent<Rigidbody>().velocity = (target.transform.position - transform.position).normalized * bulletSpeed;
                    tempBullet.GetComponent<TrailRenderer>().material.color = GetComponent<MeshRenderer>().material.color;
                }
            }
        }
        setTargets();
    }
}
