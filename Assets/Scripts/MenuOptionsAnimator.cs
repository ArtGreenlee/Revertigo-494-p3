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
        else if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            StartCoroutine(LoadSceneAsync());
        }
    }
    IEnumerator LoadSceneAsync()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        loadingPanel.SetActive(true);
        while (!op.isDone)
        {
            float progress = Mathf.Clamp01(op.progress / .9f);
            loadingBar.value = progress;
            loadingText.text = progress * 100f + "%";

            yield return 1;
        }
    }
}
