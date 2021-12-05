using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE_DOT : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject fireRing;
    private GameObject fireRingInstance;
    public GameObject fireEffect;
    private EnemyStorage enemyStorage;
    private TowerStats towerStats;
    private float cooldownTimerUtility;
    private bool attached;

    void Start()
    {
        attached = false;
        towerStats = GetComponent<TowerStats>();
        enemyStorage = EnemyStorage.instance;
        cooldownTimerUtility = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!towerStats.attachedToPlayer && Time.time - cooldownTimerUtility > towerStats.getCooldown())
        {
            if (!attached)
            {
                Debug.Log("Ring instantiated");
                /* Vector3 currentSize = rangeIndicatorInstance.GetComponent<MeshRenderer>().bounds.size;
            Vector3 newScale = new Vector3(2 * range / currentSize.x, 2 * range / currentSize.y, 2 * range / currentSize.z);
                fireRingInstance = Instantiate(fireRing, transform.position, UtilityFunctions.getRotationawayFromSide(UtilityFunctions.getClosestSide(transform.position)));
                Vector3 curSize = new Vector3(fireRing.GetComponent<ParticleSystem>().main.startSizeX, fireRing.GetComponent<ParticleSystem>().main.startSizeY, fireRing.GetComponent<ParticleSystem>().main.startSizeZ);
                fireRing.transform.localScale = new Vector3(2 * towerStats.range / curSize.x, 2 * towerStats.range / curSize.y, 2 * towerStats.range / curSize.z);
                attached = true;*/
            }
            cooldownTimerUtility = Time.time;
            foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, towerStats.range))
            {
                enemy.GetComponent<EnemyHealth>().takeDamage(towerStats.getDamage(), false, false);
            }
        }

        if (towerStats.attachedToPlayer)
        {
            if (fireRingInstance != null)
            {
                Destroy(fireRing);
            }
            attached = false;
        }
    }
}
