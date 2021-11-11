using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualizerEffects : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void fadeOut()
    {
        StartCoroutine(fadeOutRoutine());
    }

    public void fadeIn()
    {
        StartCoroutine(fadeInRoutine());
    }

    private IEnumerator fadeOutRoutine()
    {
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        while (transform.localScale.x > .01f)
        {
            float decreaseScale = .2f * Time.deltaTime;
            Vector3 newScale = new Vector3(transform.localScale.x - decreaseScale, transform.localScale.y - decreaseScale, transform.localScale.z - decreaseScale);
            transform.localScale = newScale;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    private IEnumerator fadeInRoutine()
    {
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        while (this != null && transform.localScale.x < .2f)
        {
            float increaseScale = .2f * Time.deltaTime;
            Vector3 newScale = new Vector3(transform.localScale.x + increaseScale, transform.localScale.y + increaseScale, transform.localScale.z + increaseScale);
            transform.localScale = newScale;
            yield return new WaitForEndOfFrame();
        }
    }

}
