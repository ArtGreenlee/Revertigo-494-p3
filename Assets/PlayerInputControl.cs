using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputControl : MonoBehaviour
{

    public float moveSpeed;
    public float rotateSpeed;
    public GameObject mainCamera;
    private CameraControllerTwo cameraController;
    // public bool enabled;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        cameraController = mainCamera.GetComponent<CameraControllerTwo>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(Vector3.forward * moveSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(Vector3.forward * moveSpeed * -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeForce(Vector3.left * moveSpeed );
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeForce(Vector3.left * moveSpeed * -1);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddRelativeForce(Vector3.up * moveSpeed);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddRelativeForce(Vector3.down * moveSpeed);
        }

        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 rotatePoint = hit.point;
            rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                               Quaternion.LookRotation(rotatePoint - transform.position),
                               rotateSpeed * Time.deltaTime));
        }

            /*if (Input.GetKey(KeyCode.UpArrow))
            {
                rb.AddRelativeTorque(Vector3.left * rotateSpeed);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                rb.AddRelativeTorque(Vector3.right * rotateSpeed);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rb.AddRelativeTorque(Vector3.down * rotateSpeed);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                rb.AddRelativeTorque(Vector3.up * rotateSpeed);
            }*/
        }
}
