using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealsAreaDOT : MonoBehaviour
{
    public float DPS;
    public GameObject DamageEffect;
    public float range;
    private EnemyStorage enemyStorage;

    private void Awake()
    {
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
    }

    // Start is called before the first frame update
    void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, range))
        {
            if (Vector3.Distance(enemy.transform.position, transform.position) <= range)
            {
                //bad bad bad TODO: fix this, not optimized
                enemy.GetComponent<EnemyHealth>().takeDamage(DPS * Time.deltaTime, false);
            }
        }
    }
}
