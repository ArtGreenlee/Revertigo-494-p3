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

    private void Update()
    {
        transform.position = (Vector3.zero - player.transform.position).normalized * distanceFromZero;
        transform.LookAt(player.transform);

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

        if (Camera.main.fieldOfView > 90 || Camera.main.fieldOfView < 30)
        {
            zoomSpeed = 0;
        }
    }
}
