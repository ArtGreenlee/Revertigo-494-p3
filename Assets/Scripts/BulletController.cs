using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public TowerStats towerStats;
    public GameObject onHitEffect;
    public GameObject criticalEffect;
    public GameObject onHitAoeEffect;
    private EnemyStorage enemyStorage;
    private TrailRenderer trailRenderer;
    private float lifeTime = 6;
    private float lifeTimeStart;
    private ObjectPooler objectPooler;
    private Transform hitTransform;
    private MeshRenderer meshRenderer;
    private SphereCollider sphereCollider;
    private Rigidbody rb;

    // Start is called before the first frame update
    private void Awake()
    {

        trailRenderer = GetComponent<TrailRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        sphereCollider = GetComponent<SphereCollider>();

    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        objectPooler = ObjectPooler.Instance;
        
        lifeTimeStart = Time.time;
        enemyStorage = EnemyStorage.instance;
        trailRenderer.startColor = towerStats.trailRendererColor;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time - lifeTimeStart > lifeTime)
        {
            gameObject.SetActive(false);
        }

        if (hitTransform != null)
        {
            transform.position = hitTransform.position;
        }
    }

    private void OnEnable()
    {
        if (trailRenderer != null && towerStats != null)
        {
            trailRenderer.enabled = true;
            trailRenderer.startColor = towerStats.trailRendererColor;
        }
        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }
        if (sphereCollider != null)
        {
            sphereCollider.enabled = true;
        }
        hitTransform = null;
        lifeTimeStart = Time.time;
       
    }

    private void OnCollisionEnter(Collision collision)
    {
        List<GameObject> hitEnemies = new List<GameObject>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        if (towerStats.aoe)
        {
            UtilityFunctions.changeScaleOfTransform(objectPooler.getObjectFromPool("AoeOnHitEffect", transform.position, Quaternion.identity).transform, towerStats.aoe_range / 1.5f);
            foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(collision.contacts[0].point, towerStats.aoe_range))
            {
                hitEnemies.Add(enemy);
            }
        }
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Playfield"))
        {
            objectPooler.getObjectFromPool("BulletOnHitEffect", transform.position, Quaternion.identity);
            if (collision.gameObject.CompareTag("Enemy"))
            {
                hitTransform = collision.gameObject.transform;
                hitEnemies.Add(collision.gameObject);
            }
        }


        float damage = towerStats.getDamage();
        if (towerStats.canCriticallyHit && Random.value < towerStats.critChance)
        {

            if (collision.gameObject.CompareTag("Enemy") && !towerStats.slowsEnemy)
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
            if (towerStats.poisons)
            {
                tempHealth.takeDoT(towerStats.poisonDPS, towerStats.poisonDuration, towerStats, towerStats.DOTEffect, new Color(1, 1, .9f));
            }
            tempHealth.takeDamage(damage, true);
        }
        meshRenderer.enabled = false;
        sphereCollider.enabled = false;
        trailRenderer.enabled = false;
        StartCoroutine(deactivateAfterTime(1));
    }

    private IEnumerator deactivateAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
