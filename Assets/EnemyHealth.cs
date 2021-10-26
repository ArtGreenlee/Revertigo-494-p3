using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;
    private float currentHealth;
    private EnemyStorage enemyStorage;
    // Start is called before the first frame update
    void Start()
    {
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        currentHealth = maxHealth;
    }

    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        if (damage <= 0)
        {
            enemyStorage.removeEnemy(gameObject);
            Destroy(gameObject);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
