using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToPosition : MonoBehaviour
{
    // Start is called before the first frame update
    public float rotSpeed;
    public float posSpeed;
    CameraController controller;
    float startTime;
    public bool isLerping;
    Vector3 endPos;
    Quaternion endRot;
    Vector3 startPos;
    Quaternion startRot;

    void Start()
    {
        isLerping = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isLerping) {
            // position lerp
            if (transform.position != endPos) {
                float distCovered = (Time.time - startTime) * posSpeed;
                float fraction = distCovered / Vector3.Distance(startPos, endPos);
                transform.position = Vector3.Lerp(startPos, endPos, fraction);
            }

            // rotation lerp
            if (transform.rotation != endRot) {
                transform.rotation = Quaternion.Lerp(startRot, endRot, Time.time * rotSpeed);
            }
            
            // check if done
            if (Time.time - startTime > 0.3f) {
                isLerping = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            //red - back
            // -Z
            Debug.Log("hit 1");
            endPos = new Vector3(0.0f, 0.0f, 7.66f);
            endRot = Quaternion.Euler(0, 180, 0);
            StartLerp();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            //blue - left
            // -X
            endPos = new Vector3(7.66f, 0f, 0f);
            endRot = Quaternion.Euler(0, -90 , 0);
            StartLerp();

        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            //yellow - front
            // +Z
            endPos = new Vector3(0f, 0f, -7.66f);
            endRot = Quaternion.Euler(0, 0 , 0);
            StartLerp();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            //green - right
            // +X
            endPos = new Vector3(-7.66f, 0f, 0f);
            endRot = Quaternion.Euler(0, 90, 0);
            StartLerp();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) {
            //purple - top
            // +Y
            endPos = new Vector3(0f, 7.66f, 0f);
            endRot = Quaternion.Euler(90, 0 , 0);
            StartLerp();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) {
            //pink - bottom
            // -Y
            endPos = new Vector3(0f, -7.66f, 0f);
            endRot = Quaternion.Euler(-90, 0 , 0);
            StartLerp();
            
        }
    }

    void StartLerp() {
        isLerping = true;
        startTime = Time.time;
        startPos = transform.position;
        startRot = transform.rotation;
    }
}
