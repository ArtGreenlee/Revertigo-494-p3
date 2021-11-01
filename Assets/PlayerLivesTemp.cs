using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerLivesTemp : MonoBehaviour
{
    public int numLives = 10;


    private void Start()
    {
        numLives = 10;
    }
    private void Update()
    {
        if (numLives <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
