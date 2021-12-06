using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAboutCenter : MonoBehaviour
{
    public float speed = 30;
    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(new Vector3(0, 0, 0), Vector3.up, speed * Time.deltaTime);
    }
}
