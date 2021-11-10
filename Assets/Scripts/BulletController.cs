using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public TowerStats towerStats;
    private MeshRenderer meshRenderer;
    private SphereCollider sphereCollider;
    public GameObject onHitEffect;
    public GameObject criticalEffect;
    private Rigidbody rb;
    public GameObject onHitAoeEffect;
    private EnemyStorage enemyStorage;
    private TrailRenderer trailRenderer;
    private float lifeTime = 10;
    private float lifeTimeStart;
    // Start is called before the first frame update
    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        lifeTimeStart = Time.time;
        enemyStorage = EnemyStorage.instance;
        trailRenderer.startColor = towerStats.trailRendererColor;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lifeTimeStart > lifeTime)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        lifeTimeStart = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        

        List<GameObject> hitEnemies = new List<GameObject>();

        if (towerStats.aoe)
        {
            UtilityFunctions.changeScaleOfTransform(Instantiate(onHitAoeEffect, collision.contacts[0].point, Quaternion.identity).transform, towerStats.aoe_range);
            foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(collision.contacts[0].point, towerStats.aoe_range))
            {
                hitEnemies.Add(enemy);
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Instantiate(onHitEffect, collision.contacts[0].point, Quaternion.identity);
            hitEnemies.Add(collision.gameObject);
        }

        float damage = Random.Range(towerStats.damageMin, towerStats.damageMax);
        if (towerStats.canCriticallyHit && Random.value < towerStats.critChance)
        {
            
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Instantiate(criticalEffect, collision.contacts[0].point, Quaternion.identity);
            }
            damage *= towerStats.critMult;
        }

        float slowPercentage = 1;
        if (towerStats.slowsEnemy)
        {
            if ((towerStats.canCriticallyHit && Random.value < towerStats.critChance) || !towerStats.canCriticallyHit)
            {
                slowPercentage = towerStats.slowPercent;
            }
        }

        foreach (GameObject enemy in hitEnemies)
        {
            if (slowPercentage != 1)
            {
                enemy.GetComponent<EnemyMovement>().slowEnemy(slowPercentage, towerStats.slowDuration, towerStats.slowEffect);
            }
            EnemyHealth tempHealth = enemy.GetComponent<EnemyHealth>();
            if (tempHealth.currentHealth - damage < 0 && !towerStats.attachedToPlayer)
            {
                towerStats.increaseKills();
            }
            tempHealth.takeDamage(damage, true);
        }

        gameObject.SetActive(false);
    }
}
