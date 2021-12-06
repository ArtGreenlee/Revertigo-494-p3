using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToPosition : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Player;
    MeshRenderer playerMesh;
    public float rotSpeed;
    public float posSpeed;
    CameraController controller;
    float startTime;
    public bool isLerping;
    Vector3 endPos;
    Quaternion endRot;
    Vector3 startPos;
    Quaternion startRot;
    int currentSide = 0;
    Vector3[] sidePositions = new [] { new Vector3(0f, 0f, -16f), new Vector3(16f, 0f, 0f), new Vector3(0f, 0f, 16f), new Vector3(-16f, 0f, 0f), new Vector3(0f, 16f, 0f), new Vector3(0f, -16f, 0f)};
    Quaternion[] sideRotations = new [] {Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, -90, 0), Quaternion.Euler(0, 180, 0), Quaternion.Euler(0, 90, 0), Quaternion.Euler(90, 0, 0), Quaternion.Euler(-90, 0, 0)};
    int[,] nextSide = new int[6,4] {{1, 3, 5, 4}, {2, 0, 5, 4}, {3, 1, 5, 4}, {0, 2, 5, 4}, {1, 3, 0, 2}, {1, 3, 2, 0}};

    void Start()
    {
        isLerping = false;
        playerMesh = Player.GetComponent<MeshRenderer>();
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
            // if (Time.time - startTime > 0.3f) {
            //     isLerping = false;
            // }
            if ((transform.position == endPos) && (transform.rotation == endRot)) {
                isLerping = false;
                playerMesh.enabled = !playerMesh.enabled;
            }
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            currentSide = nextSide[currentSide, 0];
            endPos = sidePositions[currentSide];
            endRot = sideRotations[currentSide];
            StartLerp();
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            currentSide = nextSide[currentSide, 1];
            endPos = sidePositions[currentSide];
            endRot = sideRotations[currentSide];
            StartLerp();
        }
        else if (Input.GetKeyDown(KeyCode.W)) {
            currentSide = nextSide[currentSide, 2];
            endPos = sidePositions[currentSide];
            endRot = sideRotations[currentSide];
            StartLerp();
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            currentSide = nextSide[currentSide, 3];
            endPos = sidePositions[currentSide];
            endRot = sideRotations[currentSide];
            StartLerp();
        }
        // if (Input.GetKeyDown(KeyCode.W)) {
        //     //red - back
        //     // -Z
        //     Debug.Log("hit 1");
        //     endPos = new Vector3(0.0f, 0.0f, 7.66f);
        //     endRot = Quaternion.Euler(0, 180, 0);
        //     StartLerp();
        // }
        // else if (Input.GetKeyDown(KeyCode.A)) {
        //     //blue - left
        //     // -X
        //     endPos = new Vector3(7.66f, 0f, 0f);
        //     endRot = Quaternion.Euler(0, -90 , 0);
        //     StartLerp();

        // }
        // else if (Input.GetKeyDown(KeyCode.S)) {
        //     //yellow - front
        //     // +Z
        //     endPos = new Vector3(0f, 0f, -7.66f);
        //     endRot = Quaternion.Euler(0, 0 , 0);
        //     StartLerp();
        // }
        // else if (Input.GetKeyDown(KeyCode.D)) {
        //     //green - right
        //     // +X
        //     endPos = new Vector3(-7.66f, 0f, 0f);
        //     endRot = Quaternion.Euler(0, 90, 0);
        //     StartLerp();
        // }
        // else if (Input.GetKeyDown(KeyCode.E)) {
        //     //purple - top
        //     // +Y
        //     endPos = new Vector3(0f, 7.66f, 0f);
        //     endRot = Quaternion.Euler(90, 0 , 0);
        //     StartLerp();
        // }
        // else if (Input.GetKeyDown(KeyCode.X)) {
        //     //pink - bottom
        //     // -Y
        //     endPos = new Vector3(0f, -7.66f, 0f);
        //     endRot = Quaternion.Euler(-90, 0 , 0);
        //     StartLerp();
            
        // }
    }

    void StartLerp() {
        isLerping = true;
        playerMesh.enabled = !playerMesh.enabled;
        startTime = Time.time;
        startPos = new Vector3(0f, 0f, 0f);
        startRot = transform.rotation;
        Debug.Log("start " + startPos.ToString() + " " + startRot.ToString() + " " + endPos.ToString() + " " + endRot.ToString() + " " + currentSide.ToString());
    }
}
