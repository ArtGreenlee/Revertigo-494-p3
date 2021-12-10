using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAboutCenter : MonoBehaviour
{
    private Vector3 rotationAxis;
    public float speed;
    private void Start()
    {
        rotationAxis = Random.onUnitSphere;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(Vector3.zero, rotationAxis, speed * Time.deltaTime);
    }
}
