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
    private AudioSource source;
    // Start is called before the first frame update
    void Awake()
    {
       source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (cv.activeSelf == false)
            {
                cv.SetActive(true);
                source.PlayOneShot(openMenuSFX, pauseVol);
                Time.timeScale = 0;
            }
            else
            {
                source.PlayOneShot(closeMenuSFX, pauseVol);
                cv.SetActive(false);
                Time.timeScale = 1.0f;
            }
        }
    }
}
