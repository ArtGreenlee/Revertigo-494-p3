using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundObjectSpawner : MonoBehaviour
{

    public GameObject backgroundObject;

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount % 360 == 0)
        {
            Instantiate(backgroundObject, Random.onUnitSphere * 25, Quaternion.identity);
        }
    }

}
