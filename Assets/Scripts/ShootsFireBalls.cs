using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootsFireBalls : MonoBehaviour
{
    public GameObject fireBall;
    public float damage;
    public float range;
    private EnemyStorage enemyStorage;
    public float cooldown;
    private float cooldownTimer;

    private void Awake()
    {
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        cooldownTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - cooldownTimer > cooldown)
        {
            GameObject target = enemyStorage.getClosestEnemyToPointWithinRange(transform.position, range);
            if (target != null)
            {
                //shoot fireball
                GameObject fireBallTemp = Instantiate(fireBall, transform.position, new Quaternion());
                fireBallTemp.transform.rotation = UtilityFunctions.getRotationawayFromSide(transform.position);
                fireBallTemp.GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * 5, ForceMode.Impulse);
                fireBallTemp.GetComponent<FireBallController>().target = target;
                cooldownTimer = Time.time;
            }
        }
    }
}
