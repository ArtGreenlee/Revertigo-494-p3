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
    private ObjectPooler objectPooler;
    // Start is called before the first frame update

    public void setMaxHealth(float maxHealthIn)
    {
        maxHealth = maxHealthIn;
        objectPooler = ObjectPooler.Instance;
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
            TowerStats tempStats = other.gameObject.GetComponent<BulletController>().towerStats;
            float damage = Random.Range(tempStats.damageMin, tempStats.damageMax);
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
        if (damage >= floatingDamageTextThreshold)
        {
            objectPooler.getObjectFromPool("FloatingDamageText", transform.position, new Quaternion()).GetComponent<FloatingDamageText>().setDamage(damage);
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
}
