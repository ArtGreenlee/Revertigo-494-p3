using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowKeyMovement : MonoBehaviour {

    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        float horizontal_input = Input.GetAxisRaw("Horizontal");
        float vertical_input = Input.GetAxisRaw("Vertical");

        rb.AddForce(new Vector3(horizontal_input, 0, vertical_input));
	}
}
