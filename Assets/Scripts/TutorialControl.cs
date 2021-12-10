using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialControl : MonoBehaviour
{
    public TextMeshPro tutorialText;
    public bool isFinished;
    public bool podiumPlaced;
    bool conditionMet;
    public GameObject GameController;
    GoldStorage goldStorage;
    float lastUpdate;
    int currInstruction;

    public static TutorialControl instance;

    bool isRunning;
    KeyCode[] keycodes = new KeyCode[]{KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.F, KeyCode.G, KeyCode.Space, KeyCode.LeftShift, KeyCode.F, KeyCode.W};
    // public bool firstPlayThrough;
    private Transform cameraTransform;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        currInstruction = 0;
        lastUpdate = Time.time;
        isRunning = false;
        isFinished = false;
        podiumPlaced = false;
        conditionMet = false;
        goldStorage = GameController.GetComponent<GoldStorage>();
        StartCoroutine(DisplayText(currInstruction));
    }

    // Update is called once per frame
    void Update()
    {
        if (currInstruction > 8) {
             return;
        }
        if (goldStorage.gold < 10f) {
            goldStorage.changeGoldAmount(10f - goldStorage.gold);
        }
        if (currInstruction != 2 && currInstruction != 4 && currInstruction != 8) {
            if (Input.GetKey(keycodes[currInstruction])) {
                conditionMet = true;
            }
            if ((Time.time - lastUpdate > 6.0) && conditionMet) {
                conditionMet = false;
                currInstruction += 1;
                lastUpdate = Time.time;
                StartCoroutine(DisplayText(currInstruction));
            }
        }
        else if (currInstruction == 2) {
            if ((Time.time - lastUpdate > 6.0) && podiumPlaced) {
                currInstruction += 1;
                lastUpdate = Time.time;
                StartCoroutine(DisplayText(currInstruction));
            }
        }
        else if (currInstruction == 4) {
            if ((Time.time - lastUpdate > 10.0) || conditionMet) {
                conditionMet = false;
                currInstruction += 1;
                lastUpdate = Time.time;
                StartCoroutine(DisplayText(currInstruction));
            }
        }
        else if (currInstruction == 8) {
            isFinished = true;
            if (Time.time - lastUpdate > 14.0) {
                currInstruction += 1;
                lastUpdate = Time.time;
                StartCoroutine(DisplayText(currInstruction));
            }
        }
        tutorialText.transform.rotation = Quaternion.LookRotation(tutorialText.transform.position - cameraTransform.position);
    }

    IEnumerator DisplayText(int instruction){
        
        if (instruction == 0) {
            yield return new WaitForSeconds(2);
            tutorialText.text = "Left click and hold to shoot";
        }
        else if (instruction == 1) {
            tutorialText.text = "";
            yield return new WaitForSeconds(2);
            tutorialText.text = "Right click to place walls to alter the enemy path \nHold right click to place multiple walls in a straight line";
        }
        else if (instruction == 2) {
            tutorialText.text = "";
            yield return new WaitForSeconds(2);
            tutorialText.text = "Place four walls in a square to form a podium";
        }
        else if (instruction == 3) {
            tutorialText.text = "";
            yield return new WaitForSeconds(2);
            tutorialText.text = "Press G to buy towers to shoot alongside you";
        }
        else if (instruction == 4) {
            tutorialText.text = "";
            yield return new WaitForSeconds(2);
            tutorialText.text = "Some towers can combine!\n If you see a line connecting towers in your inventory \n press space to combine them!";
        }
        else if (instruction == 5) {
            tutorialText.text = "";
            yield return new WaitForSeconds(2);
            tutorialText.text = "Left shift over a tower to view its stats";
        }
        else if (instruction == 6) {
            tutorialText.text = "";
            yield return new WaitForSeconds(2);
            tutorialText.text = "Press Q and E to rotate towers\n Press F while over a podium to place a tower on it";
        }
        else if (instruction == 7)
        {
            tutorialText.text = "";
            yield return new WaitForSeconds(2);
            tutorialText.text = "Use WASD to move between sides of the cube";
        }
        else if (instruction == 8) {
            tutorialText.text = "";
            yield return new WaitForSeconds(1);
            tutorialText.text = "Click a spawn point to start a wave early";
            yield return new WaitForSeconds(8);
            tutorialText.text = "Press ESC to pause and view instructions again";
        }
        else {
            tutorialText.text = "";
        }
        yield break;
        
    }
}
