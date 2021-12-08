using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCameras : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera[] cameras;
    int currentCam;
    void Start()
    {
        currentCam = 0;
        for (int i = 0; i < cameras.Length; i++) {
            if (i == currentCam) {
                cameras[i].enabled = true;
            }
            else {
                cameras[i].enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) {
            currentCam += 1;
            if (currentCam >= cameras.Length) {
                currentCam = 0;
            }
            for (int i = 0; i < cameras.Length; i++) {
                if (i == currentCam) {
                    cameras[i].enabled = true;
                }
                else {
                    cameras[i].enabled = false;
                }
            }
        }
    }
}
