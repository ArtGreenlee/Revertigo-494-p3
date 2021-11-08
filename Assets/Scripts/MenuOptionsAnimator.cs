using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptionsAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> Options;
    public Text display;
    private bool animating = false;
    private int cur_option = 0;
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
    }
    IEnumerator animate(string option)
    {
        animating = true;
        yield return new WaitForSeconds(.1f);
        animating = false;
    }
}
