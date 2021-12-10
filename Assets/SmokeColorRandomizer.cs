using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeColorRandomizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        main.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }
}
