using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectSpawner : MonoBehaviour
{

    public GameObject backgroundObject;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(1.0f / Time.deltaTime);
        if (Time.frameCount % 360 == 0)
        {
            Instantiate(backgroundObject, Random.onUnitSphere * 40, Quaternion.identity);
        }
    }

}
