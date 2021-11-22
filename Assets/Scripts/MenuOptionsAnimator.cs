using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MenuOptionsAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> Options;
    private bool animating = false;
    public GameObject loadingPanel;
    public Slider loadingBar;
    public TextMeshProUGUI loadingText;
    private void Awake()
    {
        if (!loadingPanel)
        {
            loadingPanel = GetComponent<GameObject>().transform.GetChild(4).gameObject;
        }
        loadingPanel.SetActive(false);
    }
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            loadingPanel.SetActive(true);
            StartCoroutine(LoadSceneAsync());
        }
    }
    IEnumerator LoadSceneAsync()
    {
        //Begin to load the Scene you specify
        loadingBar.value = 1;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        //
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;

        //When the load is still in progress, output the Text and progress bar
        while (asyncOperation.progress < 0.9f)
        {
            //Output the current progress
            float progress = Mathf.Clamp01(asyncOperation.progress / .9f);
            loadingText.text = "Loading progress: " + progress * 100 + "%";
            Debug.Log("Loading Progress :" + asyncOperation.progress);
            loadingBar.value = progress;

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                loadingText.text = "Loading progress: " + progress * 100 + "%";
                Debug.Log("Progress :" + asyncOperation.progress);
                loadingBar.value = progress;
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }
        if (asyncOperation.progress >= 0.9f)
        {
            loadingText.text = "Loading progress: " + Mathf.Clamp01(asyncOperation.progress / .9f) * 100 + "%";
            Debug.Log("Progress :" + asyncOperation.progress);
            loadingBar.value = Mathf.Clamp01(asyncOperation.progress / .9f);
            asyncOperation.allowSceneActivation = true;
            yield return null;
        }
    }
}