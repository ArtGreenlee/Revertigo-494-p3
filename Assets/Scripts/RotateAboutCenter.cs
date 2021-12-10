using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAboutCenter : MonoBehaviour
{
    private Vector3 rotationAxis;
    public float speed;
    private void Start()
    {
        rotationAxis = Random.onUnitSphere;
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        main.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        main = GetComponentInChildren<ParticleSystem>().main;
        main.startColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        GetComponentInChildren<Light>().color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        StartCoroutine(disableAndDestroyAfterTime());   
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(Vector3.zero, rotationAxis, speed * Time.deltaTime);
    }

    private IEnumerator disableAndDestroyAfterTime()
    {
        yield return new WaitForSeconds(10);
        GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
