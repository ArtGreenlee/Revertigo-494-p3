using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public TowerStats towerStats;
    private MeshRenderer meshRenderer;
    private SphereCollider sphereCollider;
    public GameObject onHitEffect;
    private Rigidbody rb;
    public GameObject onHitAoeEffect;
    private EnemyStorage enemyStorage;
    // Start is called before the first frame update
    private void Awake()
    {
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Instantiate(onHitEffect, transform.position, new Quaternion());
            StartCoroutine(fadeAway(other.gameObject.transform));
        }
        if (towerStats.aoe)
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                Instantiate(onHitAoeEffect, transform.position, new Quaternion());
            }
            else
            {
                Instantiate(onHitAoeEffect, Vector3.Lerp(Vector3.zero, transform.position, .95f), new Quaternion());
            }
            foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, towerStats.aoe_range))
            {
                enemy.GetComponent<EnemyHealth>().takeDamage(Random.Range(towerStats.damageMin, towerStats.damageMax), true);
                if (towerStats.slowsEnemy)
                {
                    enemy.GetComponent<EnemyMovement>().slowEnemy(towerStats.slowPercent, towerStats.slowDuration);
                }
            }
            gameObject.SetActive(false);
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
