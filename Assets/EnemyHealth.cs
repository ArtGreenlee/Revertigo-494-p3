using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
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
        healthBar.enemyHealth = this;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            ShootsBullets tower = collision.gameObject.GetComponent<BulletController>().parent;
            float damage = Random.Range(tower.damageMin, tower.damageMax);
            takeDamage(damage);
        }
    }

    public void takeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
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
