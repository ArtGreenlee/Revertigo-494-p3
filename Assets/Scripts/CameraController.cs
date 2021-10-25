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

    private float minXRotAngle = -80; //min angle around x axis
    private float maxXRotAngle = 80; // max angle around x axis

    //Mouse rotation related
    private float rotX; // around x
    private float xSpeed;
    private float ySpeed;
    private float rotY; // around y
    public float decelSpeed;
    public float maxSpeed;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rotX = transform.rotation.eulerAngles.x;
        rotY = transform.rotation.eulerAngles.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            rotX += -Input.GetAxis("Mouse Y") * mouseRotateSpeed; // around X
            rotY += Input.GetAxis("Mouse X") * mouseRotateSpeed;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            xSpeed = -Input.GetAxis("Mouse Y") * mouseRotateSpeed;
            ySpeed = Input.GetAxis("Mouse X") * mouseRotateSpeed;
        }
        else
        {
            if (xSpeed > maxSpeed)
            {
                xSpeed = maxSpeed;
            }
            else if (xSpeed < -maxSpeed)
            {
                xSpeed = -maxSpeed;
            }
            if (ySpeed > maxSpeed)
            {
                ySpeed = maxSpeed;
            }
            else if (ySpeed < -maxSpeed)
            {
                ySpeed = -maxSpeed;
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

        if (rotX < minXRotAngle)
        {
            rotX = minXRotAngle;
        }
        else if (rotX > maxXRotAngle)
        {
            rotX = maxXRotAngle;
        }

        transform.rotation = Quaternion.Euler(rotX, rotY, 0);
    }
}
