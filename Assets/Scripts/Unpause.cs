using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unpause : MonoBehaviour
{
    public GameObject cv;
    public AudioClip openMenuSFX;
    public AudioClip closeMenuSFX;
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
                AudioSource.PlayClipAtPoint(openMenuSFX, Camera.main.transform.position);
                Time.timeScale = 0;
            }
            else
            {
                AudioSource.PlayClipAtPoint(closeMenuSFX, Camera.main.transform.position);
                cv.SetActive(false);
                Time.timeScale = 1.0f;
            }
        }
    }
}
