using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDeactivate : MonoBehaviour
{
    public float deactivateTimer;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(deactivateTimer);
        gameObject.SetActive(false);
    }
}
