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
        enemyStorage = EnemyStorage.instance;
        trailRenderer.startColor = towerStats.trailRendererColor;
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(fadeAway(collision.gameObject.transform));

        List<GameObject> hitEnemies = new List<GameObject>();

        if (towerStats.aoe)
        {
            UtilityFunctions.changeScaleOfTransform(Instantiate(onHitAoeEffect, collision.contacts[0].point, new Quaternion()).transform, towerStats.aoe_range);
            foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(collision.contacts[0].point, towerStats.aoe_range))
            {
                hitEnemies.Add(enemy);
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Instantiate(onHitEffect, collision.contacts[0].point, new Quaternion());
            hitEnemies.Add(collision.gameObject);
        }

        float damage = Random.Range(towerStats.damageMin, towerStats.damageMax);
        if (towerStats.canCriticallyHit && Random.value < towerStats.critChance)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Instantiate(criticalEffect, collision.contacts[0].point, new Quaternion());
            }
            damage *= towerStats.critMult;
        }

        foreach (GameObject enemy in hitEnemies)
        {
            if (towerStats.slowsEnemy)
            {
                enemy.GetComponent<EnemyMovement>().slowEnemy(towerStats.slowPercent, towerStats.slowDuration);
            }

            EnemyHealth tempHealth = enemy.GetComponent<EnemyHealth>();
            if (tempHealth.currentHealth - damage < 0)
            {
                towerStats.increaseKills();
            }
            tempHealth.takeDamage(damage, true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(fadeAway(other.gameObject.transform));

        List<GameObject> hitEnemies = new List<GameObject>();

        if (towerStats.aoe)
        {
            UtilityFunctions.changeScaleOfTransform(Instantiate(onHitAoeEffect, other.transform.position, new Quaternion()).transform, towerStats.aoe_range);
            foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, towerStats.aoe_range))
            {
                hitEnemies.Add(enemy);
            }
        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(onHitEffect, other.transform.position, new Quaternion());
            hitEnemies.Add(other.gameObject);
        }

        float damage = Random.Range(towerStats.damageMin, towerStats.damageMax);
        if (towerStats.canCriticallyHit && Random.value > towerStats.critChance)
        {
            Instantiate(criticalEffect, Vector3.Lerp(transform.position, Vector3.zero, .05f), new Quaternion());
            damage *= towerStats.critMult;
        }

        foreach (GameObject enemy in hitEnemies)
        {
            if (towerStats.slowsEnemy)
            {
                enemy.GetComponent<EnemyMovement>().slowEnemy(towerStats.slowPercent, towerStats.slowDuration);
            }

            EnemyHealth tempHealth = enemy.GetComponent<EnemyHealth>();
            tempHealth.takeDamage(damage, true);
            if (tempHealth == null)
            {
                towerStats.increaseKills();
            }
        }

    }

    private IEnumerator fadeAway(Transform fix)
    {
        rb.velocity = Vector3.zero;
        sphereCollider.enabled = false;
        meshRenderer.enabled = false;
        while (transform.localScale.magnitude > .05f && fix != null)
        {
            transform.position = fix.position;
            float decreaseScale = .1f * Time.deltaTime;
            Vector3 newScale = new Vector3(transform.localScale.x - decreaseScale, transform.localScale.y - decreaseScale, transform.localScale.z - decreaseScale);
            transform.localScale = newScale;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
