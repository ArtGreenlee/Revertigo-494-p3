using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeactivate : MonoBehaviour
{
    public float deactivateTimer;
    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(disableAfterTime(deactivateTimer));
    }

    private IEnumerator disableAfterTime(float time)
    {
        yield return new WaitForSeconds(deactivateTimer);
        Debug.Log("deactivate on hit");
        gameObject.SetActive(false);
    }


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(deactivateTimer);
        Debug.Log("deactivate on hit");
        gameObject.SetActive(false);
    }
}
