using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{

    private Rigidbody rb;
    public GameObject target;
    public GameObject collisionEffect;
    private EnemyStorage enemyStorage;
    public float speed;
    public TowerStats towerStats;
    public float rotationSpeed;
    private bool thrusting;
    private SphereCollider sphereCollider;
    public float disableDuration;
    public PlayerInputControl playerInputControl;
    Vector3 lastSeenTargetLocation;

    private void Awake()
    {
        
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        lastSeenTargetLocation = Vector3.zero;
        enemyStorage = EnemyStorage.instance;
        playerInputControl = PlayerInputControl.instance;
        thrusting = false;
        sphereCollider.enabled = false;
        if (target == null && !towerStats.attachedToPlayer)
        {
            target = enemyStorage.getClosestEnemyToPointWithinRange(transform.position, 10);
        }
        yield return new WaitForSeconds(disableDuration);
        thrusting = true;
        sphereCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(collisionEffect, transform.position, Quaternion.identity);
        foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, towerStats.aoe_range))
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            float damage = towerStats.getDamage();
            if (enemyHealth.currentHealth - damage < 0)
            {
                Debug.Log("fireball kill count");   
                towerStats.increaseKills();
            }
            enemy.GetComponent<EnemyHealth>().takeDamage(damage, true, false);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!enemyStorage.enemyIsAlive(target) && !towerStats.attachedToPlayer)
        {
            GameObject newTarget = enemyStorage.getClosestEnemyToPointWithinRange(transform.position, 100);
            target = newTarget;
        }

        if (towerStats.attachedToPlayer)
        {
            transform.LookAt(playerInputControl.currentLookPoint);
        }
        else if (target != null)
        {
            lastSeenTargetLocation = target.transform.position;
            rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                           Quaternion.LookRotation(target.transform.position - transform.position),
                           rotationSpeed * Time.deltaTime));
        }
        else if (lastSeenTargetLocation != Vector3.zero)
        {
            rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                           Quaternion.LookRotation(lastSeenTargetLocation - transform.position),
                           rotationSpeed * Time.deltaTime));
        }

        if (thrusting)
        {
            rb.AddRelativeForce(Vector3.forward * speed);
        }
    }
}
