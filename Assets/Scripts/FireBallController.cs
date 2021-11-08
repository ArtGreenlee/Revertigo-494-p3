using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{

    private Rigidbody rb;
    public GameObject target;
    public GameObject collisionEffect;
    private EnemyStorage enemyStorage;
    private TrailRenderer trailRenderer;
    public float speed;
    public TowerStats towerStats;
    public float rotationSpeed;
    private bool thrusting;
    private SphereCollider sphereCollider;
    public float disableDuration;
    public bool controlledByPlayer;
    public PlayerInputControl playerInputControl;

    private void Awake()
    {
        
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        enemyStorage = EnemyStorage.instance;
        playerInputControl = PlayerInputControl.instance;
        thrusting = false;
        trailRenderer.enabled = false;
        sphereCollider.enabled = false;
        if (target == null)
        {
            target = enemyStorage.getClosestEnemyToPointWithinRange(transform.position, 10);
        }
        yield return new WaitForSeconds(disableDuration);
        trailRenderer.enabled = true;
        thrusting = true;
        sphereCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(collisionEffect, transform.position, Quaternion.identity);
        foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, towerStats.aoe_range))
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            float damage = Random.Range(towerStats.damageMin, towerStats.damageMax);
            if (enemyHealth.currentHealth - damage < 0)
            {
                towerStats.increaseKills();
            }
            enemy.GetComponent<EnemyHealth>().takeDamage(damage, true);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!enemyStorage.enemyIsAlive(target))
        {
            GameObject newTarget = enemyStorage.getClosestEnemyToPointWithinRange(transform.position, 100);
            if (newTarget == null)
            {
                Instantiate(collisionEffect, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            else
            {
                target = newTarget;
            }
        }

        if (target != null)
        {
            if (controlledByPlayer)
            {
                transform.LookAt(playerInputControl.currentLookPoint);
            }
            else
            {
                rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                               Quaternion.LookRotation(target.transform.position - transform.position),
                               rotationSpeed * Time.deltaTime));
            }
            
            if (thrusting)
            {
                rb.AddRelativeForce(Vector3.forward * speed);
            }
        }
        else
        {
            Instantiate(collisionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
