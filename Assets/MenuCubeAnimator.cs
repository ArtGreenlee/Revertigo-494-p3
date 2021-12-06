using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCubeAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    bool up = false;
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 360 == 0)
        {
            rb.AddTorque(Random.onUnitSphere * 3, ForceMode.Impulse);
        }
        
        if (!up && !(transform.position.y > 2)) {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.001f, transform.position.z);
        }
        if (!up && (transform.position.y > 2)) {
            up = true;
        }
        if (up && !(transform.position.y < 0)) {

            transform.position = new Vector3(transform.position.x, transform.position.y - 0.001f, transform.position.z);
        }
        if (up && (transform.position.y < 0)) {
            up = false;
        }
    }
}
