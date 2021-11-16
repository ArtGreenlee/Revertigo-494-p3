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
    private GameObject gold;
    public int goldValue;
    private Transform cameraTransform;
    public GameObject DoTEffect;
    public GameObject poisonEffect;
    private float currentDPS;
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
        cameraTransform = Camera.main.transform;
        pathfinder = GetComponent<Pathfinder>();
        wallStorage = GameObject.Find("GameController").GetComponent<WallStorage>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        flashOnHit = GetComponent<FlashOnHit>();
        healthBar = Instantiate(healthBar, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("Canvas").transform);
        healthBar.GetComponent<HealthBar>().enemyTransform = transform;
        healthBar.GetComponent<HealthBar>().enemyHealth = this;
    }

    void Start()
    {
        currentDPS = 0;
        objectPooler = ObjectPooler.Instance;
        currentHealth = maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (other.gameObject.CompareTag("Bullet"))
        {
            TowerStats tempStats = other.gameObject.GetComponent<BulletController>().towerStats;
            float damage = Random.Range(tempStats.damageMin, tempStats.damageMax);
            takeDamage(damage, true);
        }*/
    }

    public void takeDamage(float damage, bool flashingDamage, bool isDoT)
    {
        if (flashingDamage)
        {
            flashOnHit.flash();
        }
        currentHealth -= damage;
        healthBar.GetComponent<HealthBar>().showDamage();
        if (damage >= floatingDamageTextThreshold)
        {
            Vector3 textPos = Vector3.Lerp(transform.position, cameraTransform.position, .05f) + Random.insideUnitSphere;
            FloatingDamageText damageText = objectPooler.getObjectFromPool("FloatingDamageText", textPos, Quaternion.identity).GetComponent<FloatingDamageText>();
            damageText.setDamage(damage);
            if (isDoT)
            {
                damageText.color = Color.green;
            }
            else
            {
                float redColorRatio = (maxHealth - damage) / (maxHealth * 1.5f);
                damageText.color = new Color(1, redColorRatio, redColorRatio);
                damageText.color = new Color(1, redColorRatio, redColorRatio);
            }
        }
        if (currentHealth <= 0)
        {
            for (int i = 0; i < goldValue; i++)
            {
                GameObject tempGold = objectPooler.getObjectFromPool("Gold", transform.position + Random.insideUnitSphere / 2, Quaternion.LookRotation(Random.insideUnitSphere));
                tempGold.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 10, ForceMode.Impulse);
            }
            enemyStorage.removeEnemy(gameObject);
            wallStorage.pathfinders.Remove(pathfinder);
            Instantiate(onDeathEffect, transform.position, Quaternion.identity);
            Destroy(healthBar);
            Destroy(gameObject);
        }
    }

    public void takeDoT(float DPS, float duration)
    {
        if (DPS > currentDPS)
        {
            currentDPS = DPS;
            StartCoroutine(DoTroutine(DPS, duration));
        }
    }

    private IEnumerator DoTroutine(float DPS, float duration)
    {
        Color beforeColor = GetComponent<MeshRenderer>().material.color;
        GetComponent<MeshRenderer>().material.color = new Color(beforeColor.r, beforeColor.g + 1f, beforeColor.b);
        Instantiate(poisonEffect, transform.position, UtilityFunctions.getRotationawayFromSide(transform.position));
        for (float i = 0; i < duration; i += .5f)
        {
            Instantiate(DoTEffect, transform.position, UtilityFunctions.getRotationawayFromSide(transform.position));
            takeDamage(DPS / 2, true, true);
            yield return new WaitForSeconds(.5f);
        }
        currentDPS = 0;
        GetComponent<MeshRenderer>().material.color = beforeColor;
    }
}
