using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootsFireBalls : MonoBehaviour
{
    public GameObject fireBall;
    private EnemyStorage enemyStorage;
    private float cooldownTimer;
    private TowerStats towerStats;
    private void Awake()
    {
        towerStats = GetComponent<TowerStats>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        cooldownTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - cooldownTimer > towerStats.cooldown)
        {
            if (enemyStorage.getClosestEnemyToPointWithinRange(transform.position, towerStats.range) != null)
            {
                //shoot fireball
                GameObject fireBallTemp = Instantiate(fireBall, transform.position, UtilityFunctions.getRotationawayFromSide(transform.position));
                fireBallTemp.GetComponent<Rigidbody>().AddRelativeForce(Vector3.back * 3, ForceMode.Impulse);
                FireBallController fireBallControllerTemp;
                ClusterFireballController clusterFireballControllerTemp;
                if (fireBallTemp.TryGetComponent<FireBallController>(out fireBallControllerTemp))
                {
                    fireBallControllerTemp.towerStats = towerStats;
                }
                else if (fireBallTemp.TryGetComponent<ClusterFireballController>(out clusterFireballControllerTemp))
                {
                    clusterFireballControllerTemp.towerStats = towerStats;
                }
                cooldownTimer = Time.time;
            }
        }
    }
     
    public void ShootFireball(Vector3 direction)
    {
        if (Time.time - cooldownTimer > towerStats.cooldown)
        {
            GameObject fireBallTemp = Instantiate(fireBall, transform.position, UtilityFunctions.getRotationawayFromSide(transform.position));
            fireBallTemp.GetComponent<Rigidbody>().AddForce(direction * 7, ForceMode.Impulse);
            FireBallController fireBallControllerTemp;
            ClusterFireballController clusterFireballControllerTemp;
            if (fireBallTemp.TryGetComponent<FireBallController>(out fireBallControllerTemp))
            {
                fireBallControllerTemp.towerStats = towerStats;
                fireBallControllerTemp.disableDuration = 0;
            }
            else if (fireBallTemp.TryGetComponent<ClusterFireballController>(out clusterFireballControllerTemp))
            {
                clusterFireballControllerTemp.towerStats = towerStats;
                clusterFireballControllerTemp.disableDuration = .5f;
            }
            cooldownTimer = Time.time;
        }
        
    }
}
