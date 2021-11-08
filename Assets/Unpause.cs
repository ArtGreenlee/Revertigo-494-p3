using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unpause : MonoBehaviour
{
    public GameObject cv;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (cv.activeSelf == false)
            {
                cv.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                cv.SetActive(false);
                Time.timeScale = 1.0f;
            }
        }
    }
}
