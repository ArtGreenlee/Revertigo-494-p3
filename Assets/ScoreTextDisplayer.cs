using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreTextDisplayer : MonoBehaviour
{
    // Start is called before the first frame update
    public Text title;
    public Text score;
    void Start()
    {
        score.text = "Score  " + ScoreCounter.instance.score.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerLivesTemp.instance.timeRemaining > 0) {
            title.text = "Game Over";

        }
        else {
            title.text = "You win!";
        }
    }
}
