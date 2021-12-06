using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayerLivesTemp : MonoBehaviour
{
    public int max_lives;
    //public TextMeshProUGUI livesCounter;
    public static PlayerLivesTemp instance;
    public AudioClip loseLifeSFX;
    public float loseLifeVol = 0.3f;
    private AudioSource source;
    private TowerInventory towerInventory;
    private int numLives; 
    public Slider healthBar;
    private float timeRemaining;
    public TextMeshProUGUI TimerText;
    public Texture fade_shape = null;
    public bool transition_flag = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        source = GetComponent<AudioSource>();
    }
    private void Start()
    {
        towerInventory = TowerInventory.instance;
        numLives = max_lives;
        healthBar.value = Mathf.Clamp01((float)numLives / max_lives);
        timeRemaining = 60 * 15 + 1;
    }

    private void Update()
    {
        if (numLives <= 0)
        {
            if(!transition_flag){
                Debug.Log("TRANSITION DEATH");
                transition_flag = true;
                SceneTransitionController.RequestSceneTransition("End Scene", 1.5f, _SceneTransitionCallback, null);
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            SceneTransitionController.RequestSceneTransition("Menu Scene", 1.5f, _SceneTransitionCallback, fade_shape);
        }
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            TimeSpan time = TimeSpan.FromSeconds(timeRemaining);
            TimerText.text = time.ToString("mm':'ss");
        }
        else
        {
            if(!transition_flag){
                Debug.Log("TRANSITION TIMER");
                transition_flag = true;
                SceneTransitionController.RequestSceneTransition("End Scene", 1.5f, _SceneTransitionCallback, null);
            }
        }
    }

    public void loseLife()
    {

        numLives--;
        source.PlayOneShot(loseLifeSFX, loseLifeVol);

        //livesCounter.text = "Lives " + numLives.ToString();
        //Debug.Log(Mathf.Clamp01((float)numLives / max_lives));
        healthBar.value = Mathf.Clamp01((float)numLives / max_lives);
        EventBus.Publish<PlayerLifeEvent>(new PlayerLifeEvent(-1));
    }

    void _SceneTransitionCallback(SceneTransitionState transition_state, string scene_name)
    {
        Debug.Log(transition_state);
    }
}
