using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialControl : MonoBehaviour
{
    public TextMeshPro tutorialText;
    float lastUpdate;
    int currInstruction;
    KeyCode[] keycodes = new KeyCode[]{ KeyCode.Space, KeyCode.Mouse1, KeyCode.Mouse0, KeyCode.G, KeyCode.F, KeyCode.P};
    public bool firstPlayThrough;
    private Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
        if (firstPlayThrough)
        {
            currInstruction = 0;
            lastUpdate = Time.time;
            StartCoroutine(DisplayText(currInstruction));
        }
        
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currInstruction > 5) {
            return;
        }
        if ((Time.time - lastUpdate > 10.0) || Input.GetKey(keycodes[currInstruction])) {
            
            currInstruction += 1;
            lastUpdate = Time.time;
            StartCoroutine(DisplayText(currInstruction));
        }
        tutorialText.transform.rotation = Quaternion.LookRotation(tutorialText.transform.position - cameraTransform.position);
    }

    IEnumerator DisplayText(int instruction){
        
        if (instruction == 0) {
            yield return new WaitForSeconds(2);
            tutorialText.text = "Use the mouse to look around \nPress space to hold position";
        }
        else if (instruction == 1) {
            tutorialText.text = "";
            yield return new WaitForSeconds(1);
            tutorialText.text = "Right click to place walls to block the enemy path";

        }
        else if (instruction == 2) {
            tutorialText.text = "";
            yield return new WaitForSeconds(1);
            tutorialText.text = "Left click to shoot enemies";

        }
        else if (instruction == 3) {
            tutorialText.text = "";
            yield return new WaitForSeconds(1);
            tutorialText.text = "Press G to buy towers to shoot alongside you";
        }
        else if (instruction == 4)
        {
            tutorialText.text = "";
            yield return new WaitForSeconds(1);
            tutorialText.text = "Press F while over a wall to place a tower on the wall, right click on towers to view information about it";
        }
        else if (instruction == 5) {
            tutorialText.text = "";
            yield return new WaitForSeconds(1);
            tutorialText.text = "Press P to pause and view instructions again";
        }
        else {
            tutorialText.text = "";
        }
        yield break;
        
    }
}
