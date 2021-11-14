using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialControl : MonoBehaviour
{
    public TextMeshPro tutorialText;
    float lastUpdate;
    int currInstruction;
    KeyCode[] keycodes = new KeyCode[]{ KeyCode.Space, KeyCode.Mouse1, KeyCode.Mouse0, KeyCode.G, KeyCode.P};

    // Start is called before the first frame update
    void Start()
    {
        currInstruction = 0;
        lastUpdate = Time.time;
        StartCoroutine(DisplayText(currInstruction));
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currInstruction > 4) {
            return;
        }
        if ((Time.time - lastUpdate > 10.0) || Input.GetKey(keycodes[currInstruction])) {
            currInstruction += 1;
            lastUpdate = Time.time;
            StartCoroutine(DisplayText(currInstruction));
        }
    }

    IEnumerator DisplayText(int instruction){
        
        if (instruction == 0) {
            yield return new WaitForSeconds(2);
            tutorialText.text = "Use the mouse to look around \nPress space to hold position";
        }
        else if (instruction == 1) {
            tutorialText.text = "";
            yield return new WaitForSeconds(1);
            tutorialText.text = "Right click to place towers";

        }
        else if (instruction == 2) {
            tutorialText.text = "";
            yield return new WaitForSeconds(1);
            tutorialText.text = "Left click to shoot enemies";

        }
        else if (instruction == 3) {
            tutorialText.text = "";
            yield return new WaitForSeconds(1);
            tutorialText.text = "Press G to buy extra guns";

        }
        else if (instruction == 4) {
            tutorialText.text = "";
            yield return new WaitForSeconds(1);
            tutorialText.text = "Press P to pause and view instructions";
        }
        else {
            tutorialText.text = "";
        }
        yield return null;
        
    }
}
