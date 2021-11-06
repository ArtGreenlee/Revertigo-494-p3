using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterFireballController : MonoBehaviour
{
    public GameObject splitEffect;
    private EnemyStorage enemyStorage;
    public GameObject fireBall;
    public TowerStats towerStats;
    private Rigidbody rb;
    public float disableDuration;
    public bool controlledByPlayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Playfield") || other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("big explosion");
            Destroy(gameObject);
        }
    }

    private List<GameObject> getTargets()
    {
        List<GameObject> targets = new List<GameObject>();
        foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, towerStats.range * 2))
        {
            if (!targets.Contains(enemy))
            {
                targets.Add(enemy);
                if (targets.Count >= 5)
                {
                    return targets;
                }
            }
        }
        if (targets.Count < 5)
        {
            foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, towerStats.range * 2))
            {
                targets.Add(enemy);
                if (targets.Count >= 5)
                {
                    return targets;
                }
            }
        }
        return targets;
    }

    private IEnumerator Start()
    {
        enemyStorage = EnemyStorage.instance;
        rb.angularVelocity = Random.onUnitSphere * 3;
        yield return new WaitForSeconds(disableDuration);
        Vector3 side = UtilityFunctions.getClosestSide(transform.position);
        List<GameObject> targets = getTargets();
        List<Vector3> launchDirections = UtilityFunctions.sideVectors;
        int launchDirectionIndex = 0;
        Instantiate(splitEffect, transform.position, UtilityFunctions.getRotationawayFromSide(side));
        foreach (GameObject target in targets)
        {
            if (side == launchDirections[launchDirectionIndex])
            {
                launchDirectionIndex++;
            }
            Vector3 launchDirection = launchDirections[launchDirectionIndex];
            launchDirectionIndex++;
            GameObject tempRocket = Instantiate(fireBall, transform.position, Quaternion.LookRotation(launchDirection));
            FireBallController tempController = tempRocket.GetComponent<FireBallController>();
            tempController.disableDuration = .75f;
            tempController.target = target;
            tempController.towerStats = towerStats;
            tempController.controlledByPlayer = controlledByPlayer;
            Vector3 initialForce = new Vector3(launchDirection.x * 16, launchDirection.y * 16, launchDirection.z * 16);
            initialForce = (transform.position - initialForce).normalized * 2;
            tempRocket.GetComponent<Rigidbody>().AddForce(initialForce, ForceMode.Impulse);
            //yield return new WaitForSeconds(.5f);
        }
        Destroy(gameObject);
    }
}
