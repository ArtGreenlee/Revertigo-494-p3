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
    public float rescaleFactor;

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
                /*Vector3 currentSize = rangeIndicatorInstance.GetComponent<MeshRenderer>().bounds.size;
                Vector3 newScale = new Vector3(2 * range / currentSize.x, 2 * range / currentSize.y, 2 * range / currentSize.z);*/
                fireRingInstance = Instantiate(fireRing, transform.position, Quaternion.Euler(new Vector3(0, 90, 0) + UtilityFunctions.getClosestSide(transform.position) * 90));
                fireRingInstance.transform.localScale = new Vector3(rescaleFactor * towerStats.range, rescaleFactor * towerStats.range, rescaleFactor * towerStats.range);
                attached = true;
            }
            cooldownTimerUtility = Time.time;
            foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, towerStats.range))
            {
                enemy.GetComponent<EnemyHealth>().takeDoT(towerStats.getDamage(), 2, towerStats, towerStats.DOTEffect, new Color(1, .9f, .9f));
                //enemy.GetComponent<EnemyHealth>().takeDamage(towerStats.getDamage(), false, false);
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
