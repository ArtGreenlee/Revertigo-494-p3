using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public GameObject prefab;

    float delay = 0.0f;
    public float delay_time = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        delay -= Time.deltaTime;

        if(delay < 0.0f)
        {
            delay = delay_time;
            GameObject.Instantiate(prefab, transform.position + UnityEngine.Random.onUnitSphere, Quaternion.identity);
        }

        
	}
}
