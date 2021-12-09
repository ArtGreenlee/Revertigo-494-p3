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
    public int goldValue;
    private Transform cameraTransform;
    private Color originalColor;
    private MeshRenderer meshRenderer;
    private ScoreCounter scoreCounter;
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
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
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
        scoreCounter = ScoreCounter.instance;
        meshRenderer = GetComponent<MeshRenderer>();
        originalColor = meshRenderer.material.color;
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
            Vector3 textPos = Vector3.Lerp(transform.position, cameraTransform.position, .05f) + Random.insideUnitSphere;
            FloatingDamageText damageText = objectPooler.getObjectFromPool("FloatingDamageText", textPos, Quaternion.identity).GetComponent<FloatingDamageText>();

            float redColorRatio = (maxHealth - damage) / (maxHealth * 1.5f);
            damageText.color = new Color(1, redColorRatio, redColorRatio);
            damageText.setDamage(damage);
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
            if (onDeathEffect != null)
            {
                Instantiate(onDeathEffect, transform.position, Quaternion.identity);
            }
            scoreCounter.score += Mathf.RoundToInt(maxHealth);
            Destroy(healthBar);
            Destroy(gameObject);
        }
    }

    public void takeDoT(float DPS, float duration, TowerStats towerStats, GameObject DoTEffect, Color newColor)
    {
        StartCoroutine(DoTroutine(DPS, duration, towerStats, DoTEffect, newColor));
    }

    private IEnumerator DoTroutine(float DPS, float duration, TowerStats towerStats, GameObject DoTEffect, Color newColor)
    {
        GetComponent<MeshRenderer>().material.color = newColor;
        if (DoTEffect != null)
        {
            Instantiate(DoTEffect, transform.position, Quaternion.identity, transform);
        }
        for (float i = 0; i < duration; i += .5f)
        {
            if (currentHealth - DPS / 2 < 0)
            {
                towerStats.increaseKills();
            }
            takeDamage(DPS / 2, false);
            yield return new WaitForSeconds(.5f);
        }
        GetComponent<MeshRenderer>().material.color = originalColor;
    }
}
