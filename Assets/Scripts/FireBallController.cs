using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallController : MonoBehaviour
{

    private Rigidbody rb;
    public GameObject target;
    private TrailRenderer trailRenderer;
    public GameObject collisionEffect;
    public float speed;
    public float rotationSpeed;
    private bool thrusting;

    private void Awake()
    {
        trailRenderer = GetComponent<TrailRenderer>();
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        trailRenderer.enabled = false;
        thrusting = false;
        yield return new WaitForSeconds(1f);
        thrusting = true;
        trailRenderer.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(collisionEffect, transform.position, new Quaternion());
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                   Quaternion.LookRotation(target.transform.position - transform.position),
                   rotationSpeed * Time.deltaTime));
        if (thrusting)
        {

            //transform.LookAt(target.transform);
            /*var qTo = Quaternion.LookRotation(target.position - transform.position);
     qTo = Quaternion.Slerp(transform.rotation, qTo, speed * Time.deltaTime);
     rigidbody.MoveRotation(qTo);
            */
            
            rb.AddRelativeForce(Vector3.forward * speed);
            //rb.AddForce((target.transform.position - transform.position).normalized * speed);
        }
    }
}
