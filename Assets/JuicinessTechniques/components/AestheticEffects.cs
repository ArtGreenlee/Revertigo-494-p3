using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AestheticEffects : MonoBehaviour {

    Vector3 initial_size;

    public float k = 0.5f;

	// Use this for initialization
	void Start () {
        initial_size = transform.localScale;
	}

    float velocity = 0.0f;

	// Update is called once per frame
	void Update () {

        transform.localScale = Vector3.LerpUnclamped(transform.localScale, initial_size, 0.1f);

        PerformHookes();
	}

    public void Bump(float magnitude)
    {
        magnitude = Mathf.Clamp01(magnitude);

        transform.localScale = (1.0f - magnitude) * Vector3.one;
    }

    void PerformHookes()
    {
        float delta = initial_size.x - transform.localScale.x;
        float acceleration = delta * k;

        velocity += acceleration;

        transform.localScale += velocity * Vector3.one;
    }
}
