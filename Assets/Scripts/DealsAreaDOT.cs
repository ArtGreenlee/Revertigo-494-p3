using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealsAreaDOT : MonoBehaviour
{
    public GameObject DamageEffect;
    private EnemyStorage enemyStorage;
    private TowerStats towerStats;
    private void Awake()
    {
        towerStats = GetComponent<TowerStats>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, towerStats.range))
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) <= towerStats.range)
            {
                //bad bad bad TODO: fix this, not optimized
                enemy.GetComponent<EnemyHealth>().takeDamage(towerStats.damageMax * Time.deltaTime, false);
            }
        }
    }
}
