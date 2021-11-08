using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootsFireBalls : MonoBehaviour
{
    public GameObject fireBall;
    private EnemyStorage enemyStorage;
    private float cooldownTimer;
    private TowerStats towerStats;
    public bool controlledByPlayer;
    private void Awake()
    {
        towerStats = GetComponent<TowerStats>();
        
        cooldownTimer = 0;
    }

    private void Start()
    {
        enemyStorage = EnemyStorage.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (towerStats.automaticallyShoots && Time.time - cooldownTimer > towerStats.cooldown)
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
                    fireBallControllerTemp.controlledByPlayer = false;
                }
                else if (fireBallTemp.TryGetComponent<ClusterFireballController>(out clusterFireballControllerTemp))
                {
                    clusterFireballControllerTemp.disableDuration = 1f;
                    clusterFireballControllerTemp.towerStats = towerStats;
                    clusterFireballControllerTemp.controlledByPlayer = false;
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
                fireBallControllerTemp.controlledByPlayer = true;
            }
            else if (fireBallTemp.TryGetComponent<ClusterFireballController>(out clusterFireballControllerTemp))
            {
                clusterFireballControllerTemp.towerStats = towerStats;
                clusterFireballControllerTemp.disableDuration = .5f;
                clusterFireballControllerTemp.controlledByPlayer = true;
            }
            cooldownTimer = Time.time;
        }
        
    }
}
