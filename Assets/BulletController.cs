using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    private float lifeStart;
    public float lifeTime;
    public ShootsBullets parent;
    private MeshRenderer meshRenderer;
    private SphereCollider sphereCollider;
    private Rigidbody rb;
    // Start is called before the first frame update
    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        
        lifeStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lifeStart > lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(fadeAway(collision.gameObject.transform));
    }

    private IEnumerator fadeAway(Transform fix)
    {
        rb.velocity = Vector3.zero;
        sphereCollider.enabled = false;
        meshRenderer.enabled = false;
        while (transform.localScale.magnitude > .05f && fix != null)
        {
            transform.position = fix.position;
            float decreaseScale = .05f * Time.deltaTime;
            Vector3 newScale = new Vector3(transform.localScale.x - decreaseScale, transform.localScale.y - decreaseScale, transform.localScale.z - decreaseScale);
            transform.localScale = newScale;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
