using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    private AudioSource audio;
    private Unpause up;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        up = GetComponent<Unpause>();
    }

    // Update is called once per frame
    void Update()
    {
        if (up.isPaused() == true)
        {
            audio.Pause();
        }
        else
        {
            audio.UnPause();
        }
    }
}
