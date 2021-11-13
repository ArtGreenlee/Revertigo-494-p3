using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class PlayerLivesTemp : MonoBehaviour
{
    public int numLives;
    public TextMeshProUGUI livesCounter;
    public static PlayerLivesTemp instance;
    private TowerInventory towerInventory;



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        towerInventory = TowerInventory.instance;
    }

    private void Update()
    {
        if (numLives <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }

    public void loseLife()
    {
        if (!towerInventory.debugMode)
        {
            numLives--;
        }

        
        livesCounter.text = "Lives " + numLives.ToString();
    }
}
