using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ScoreCounter : MonoBehaviour
{

    public static ScoreCounter instance;
    public int score;
    private TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    private void Awake()
    {
        score = 0;
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        scoreText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 360 == 0 && TutorialControl.instance.isFinished)
        {
            score += 10;
        }
        scoreText.text = "Score: " + score.ToString();
    }
}
