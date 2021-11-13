using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MenuOptionsAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> Options;
    public Text display;
    private bool animating = false;
    public GameObject loadingPanel;
    public Slider loadingBar;
    public Text loadingText;
    void Start()
    {
        if (Options.Count < 1)
            Options.Add("");
    }

    // Update is called once per frame
    void Update()
    {
        if (!animating)
        {
            //animate(Options[cur_option % Options.Count]);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartCoroutine(LoadSceneAsync());
        }
    }
    IEnumerator LoadSceneAsync()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        loadingPanel.SetActive(true);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Pro :" + asyncOperation.progress);
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            loadingText.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";
            loadingBar.value = asyncOperation.progress;

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Change the Text to show the Scene is ready
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
