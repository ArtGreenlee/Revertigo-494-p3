using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealth : MonoBehaviour
{
    private float maxHealth;
    public float currentHealth;
    private EnemyStorage enemyStorage;
    public GameObject healthBar;
    private FlashOnHit flashOnHit;
    public GameObject onDeathEffect;
    private WallStorage wallStorage;
    private Pathfinder pathfinder;
    public GameObject FloatingDamageText;
    public float floatingDamageTextThreshold;
    // Start is called before the first frame update

    public void setMaxHealth(float maxHealthIn)
    {
        maxHealth = maxHealthIn;
    }

    public float getMaxHealth()
    {
        return maxHealth;
    }

    private void Awake()
    {
        pathfinder = GetComponent<Pathfinder>();
        wallStorage = GameObject.Find("GameController").GetComponent<WallStorage>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        flashOnHit = GetComponent<FlashOnHit>();
        healthBar = Instantiate(healthBar, transform.position, new Quaternion(), GameObject.FindGameObjectWithTag("Canvas").transform);
        healthBar.GetComponent<HealthBar>().enemyTransform = transform;
        healthBar.GetComponent<HealthBar>().enemyHealth = this;
    }

    void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            float damage = Random.Range(other.gameObject.GetComponent<BulletController>().towerStats.damageMin, 
                other.gameObject.GetComponent<BulletController>().towerStats.damageMax);
            takeDamage(damage, true);
        }
    }

    public void takeDamage(float damage, bool flashingDamage)
    {
        if (flashingDamage)
        {
            flashOnHit.flash();
        }
        currentHealth -= damage;
        healthBar.GetComponent<HealthBar>().showDamage();
        if (damage >= floatingDamageTextThreshold && Vector3.Distance(Camera.main.transform.position, transform.position) < 16)
        {
            Instantiate(FloatingDamageText, transform.position, new Quaternion()).GetComponent<FloatingDamageText>().setDamage(damage);
        }
        if (currentHealth <= 0)
        {
            enemyStorage.removeEnemy(gameObject);
            wallStorage.pathfinders.Remove(pathfinder);
            Instantiate(onDeathEffect, transform.position, new Quaternion());
            Destroy(healthBar);
            Destroy(gameObject);
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
