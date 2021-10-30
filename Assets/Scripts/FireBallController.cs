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
    public float rotationSpeed;
    private bool thrusting;
    public float damage;
    public float aoeRange;

    private void Awake()
    {
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        thrusting = false;
        trailRenderer.enabled = false;
        yield return new WaitForSeconds(1f);
        trailRenderer.enabled = true;
        thrusting = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(collisionEffect, transform.position, new Quaternion());
        foreach (GameObject enemy in enemyStorage.getAllEnemiesWithinRange(transform.position, aoeRange))
        {
            enemy.GetComponent<EnemyHealth>().takeDamage(damage, true);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyStorage.enemyIsAlive(target))
        {
            GameObject newTarget = enemyStorage.getClosestEnemyToPointWithinRange(transform.position, 100);
            if (newTarget == null)
            {
                Destroy(gameObject);
            }
            else
            {
                target = newTarget;
            }
        }

        if (target != null)
        {
            rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                               Quaternion.LookRotation(target.transform.position - transform.position),
                               rotationSpeed * Time.deltaTime));
            if (thrusting)
            {
                rb.AddRelativeForce(Vector3.forward * speed);
            }
        }
    }
}
