using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unpause : MonoBehaviour
{
    public GameObject cv;
    public AudioClip openMenuSFX;
    public AudioClip closeMenuSFX;
    public float pauseVol = 0.2f;
    private bool gameisPaused = false;
    private AudioSource source;
    // Start is called before the first frame update
    void Awake()
    {
       source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (cv.activeSelf == false)
            {
                source.PlayOneShot(openMenuSFX, 1.0f);
                cv.SetActive(true);                
                Time.timeScale = 0;
                gameisPaused = true;
            }
            else
            {
                source.PlayOneShot(closeMenuSFX, pauseVol);
                cv.SetActive(false);
                Time.timeScale = 1.0f;
                gameisPaused = false;
            }
        }
    }

    public bool isPaused()
    {
        return (gameisPaused);
    }
}
