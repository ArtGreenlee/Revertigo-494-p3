using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerTwo : MonoBehaviour
{
    public GameObject player;
    public float distanceFromZero;
    public float zoomAcceleration;
    public float zoomDecel;
    private float zoomSpeed;
    private Rigidbody rb;
    public float rotateSpeed;
    public float moveSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, (Vector3.zero - player.transform.position).normalized * distanceFromZero, moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.transform.position), rotateSpeed * Time.deltaTime);

        
    }

    private void Update()
    {

        /*rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                               Quaternion.LookRotation(transform.position - player.transform.position),
                               rotateSpeed * Time.deltaTime));

        transform.position = (Vector3.zero - player.transform.position).normalized * distanceFromZero;
        //rb.AddRelativeTorque(player.transform.position.normalized * rotateSpeed);
        //transform.LookAt(Vector3.Slerp(transform.rotation.eulerAngles, player.transform.rotation.eulerAngles, rotateSpeed * Time.deltaTime));
        //transform.LookAt(player.transform);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(player.transform.position), rotateSpeed * Time.deltaTime);
        */
        float zoomInput = Input.mouseScrollDelta.y;

        if (zoomInput != 0)
        {
            zoomSpeed += zoomAcceleration * zoomInput * Time.deltaTime * -1;
        }
        else
        {
            if (zoomSpeed > 0)
            {
                zoomSpeed -= zoomDecel * Time.deltaTime;
                if (zoomSpeed < 0)
                {
                    zoomSpeed = 0;
                }
            }
            else if (zoomSpeed < 0)
            {
                zoomSpeed += zoomDecel * Time.deltaTime;
                if (zoomSpeed > 0)
                {
                    zoomSpeed = 0;
                }
            }   
        }

        Camera.main.fieldOfView += zoomSpeed;

        if (Camera.main.fieldOfView > 90)
        {
            Camera.main.fieldOfView = 90;
            zoomSpeed = 0;
        }
        else if (Camera.main.fieldOfView < 30)
        {
            Camera.main.fieldOfView = 30;
            zoomSpeed = 0;
        }
    }
}
