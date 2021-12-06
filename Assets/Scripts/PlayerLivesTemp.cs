using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public float timeRemaining = 10;
    public GameObject TimerText;


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
    }

    private void Update()
    {
        if (numLives <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            TimerText.GetComponent<TextMeshPro>().text = timeRemaining.ToString();
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
}
