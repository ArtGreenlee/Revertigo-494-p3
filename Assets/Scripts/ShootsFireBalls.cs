using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootsFireBalls : MonoBehaviour
{
    public GameObject fireBall;
    public float damageMin;
    public float damageMax;
    public float aoeRange;
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
            if (enemyStorage.getClosestEnemyToPointWithinRange(transform.position, range) != null)
            {
                //shoot fireball
                GameObject fireBallTemp = Instantiate(fireBall, transform.position, UtilityFunctions.getRotationawayFromSide(transform.position));
                fireBallTemp.GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * 3, ForceMode.Impulse);
                FireBallController fireBallControllerTemp = fireBallTemp.GetComponent<FireBallController>();
                fireBallControllerTemp.damage = Random.Range(damageMin, damageMax);
                fireBallControllerTemp.aoeRange = aoeRange;
                cooldownTimer = Time.time;
            }
        }
    }
}
