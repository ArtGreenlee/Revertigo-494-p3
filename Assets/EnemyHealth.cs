using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;
    private float currentHealth;
    private EnemyStorage enemyStorage;
    public GameObject HealthBarObject;
    private HealthBar healthBar;
    // Start is called before the first frame update
    void Start()
    {
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        currentHealth = maxHealth;
        healthBar = Instantiate(HealthBarObject, transform.position, new Quaternion(), GameObject.FindGameObjectWithTag("Canvas").transform).GetComponent<HealthBar>();
        healthBar.enemyTransform = transform;
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
