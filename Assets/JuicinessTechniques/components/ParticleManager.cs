using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour {

    static ParticleManager instance;

    bool should_shake = false;
    float magnitude = 1.0f;

    float timestamp_next_stable = 0.0f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    ParticleSystem ps; 

	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
    public static void RequestBurst(Vector3 pos)
    {
        instance.transform.position = pos;
        instance.ps.Play();
    }
}
