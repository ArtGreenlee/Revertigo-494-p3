using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootsAlongAxis : MonoBehaviour
{
    public float cooldown;
    public float cooldownTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(UtilityFunctions.snapVector(transform.position), transform.TransformDirection(Vector3.up)))
        {

        }
    }
}
