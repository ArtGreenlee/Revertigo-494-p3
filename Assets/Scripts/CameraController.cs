using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera mainCamera;
    [Range(0.1f, 5f)]
    [Tooltip("How sensitive the mouse drag to camera rotation")]
    public float mouseRotateSpeed = 0.8f;
    [Range(0.01f, 100)]
    [Tooltip("How sensitive the touch drag to camera rotation")]
    public float slerpValue = 0.25f;

    //private float minXRotAngle = -80; //min angle around x axis
    //private float maxXRotAngle = 80; // max angle around x axis

    //Mouse rotation related
    private float rotX; // around x
    private float xSpeed;
    private float ySpeed;
    private float rotY; // around y
    public float decelSpeed;
    public float maxRotationSpeed;
    public float moveSpeed;
    // public bool enabled;

    public float zoomAcceleration;
    public float zoomDecel;
    private float zoomSpeed;

    public int inverted = 1;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        zoomSpeed = 0;
        rb = GetComponent<Rigidbody>();
        enabled = true;
        if(inverted >= 0){
            inverted = 1;
        }
        else{
            inverted = -1;
        }
    }

    // Update is called once per frame
    void Update()  
    {
        rotX = transform.rotation.eulerAngles.x;
        rotY = transform.rotation.eulerAngles.y;
        rb.AddRelativeForce(Vector3.forward * moveSpeed * Input.GetAxis("Vertical"));
        rb.AddRelativeForce(Vector3.left * moveSpeed * Input.GetAxis("Horizontal") * -1);
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * moveSpeed);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddForce(Vector3.down * moveSpeed);
        }

        if(Input.GetKey(KeyCode.I)){
            inverted *= -1;
        }

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

        mainCamera.fieldOfView += zoomSpeed;

        if (mainCamera.fieldOfView > 90 || mainCamera.fieldOfView < 30)
        {
            zoomSpeed = 0;
        }

        //handle rotation
        if (Input.GetMouseButton(1))
        {
            rotX += -Input.GetAxis("Mouse Y") * mouseRotateSpeed * inverted; // around X
            rotY += Input.GetAxis("Mouse X") * mouseRotateSpeed * inverted;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            xSpeed = -Input.GetAxis("Mouse Y") * mouseRotateSpeed * inverted;
            ySpeed = Input.GetAxis("Mouse X") * mouseRotateSpeed * inverted;
        }
        else
        {
            if (xSpeed > maxRotationSpeed)
            {
                xSpeed = maxRotationSpeed;
            }
            else if (xSpeed < -maxRotationSpeed)
            {
                xSpeed = -maxRotationSpeed;
            }
            if (ySpeed > maxRotationSpeed)
            {
                ySpeed = maxRotationSpeed;
            }
            else if (ySpeed < -maxRotationSpeed)
            {
                ySpeed = -maxRotationSpeed;
            }
            if (xSpeed < 0)
            {
                xSpeed += decelSpeed * Time.deltaTime;
                if (xSpeed > 0)
                {
                    xSpeed = 0;
                }
            }
            else if (xSpeed > 0)
            {
                xSpeed -= decelSpeed * Time.deltaTime;
                if (xSpeed < 0)
                {
                    xSpeed = 0;
                }
            }
            else
            {
                xSpeed = 0;
            }

            if (ySpeed < 0)
            {
                ySpeed += decelSpeed * Time.deltaTime;
                if (ySpeed > 0)
                {
                    ySpeed = 0;
                }
            }
            else if (ySpeed > 0)
            {
                ySpeed -= decelSpeed * Time.deltaTime;
                if (ySpeed < 0)
                {
                    ySpeed = 0;
                }
            }
            else
            {
                ySpeed = 0;
            }

            rotX += xSpeed;
            rotY += ySpeed;
        }

        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
    }
}
