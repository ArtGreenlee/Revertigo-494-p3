using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuFireAnimator : MonoBehaviour
{
    public GameObject[] projectiles;
    bool running;
    float startTime;
    public int frequency = 500;
    GameObject effectInstance;


    // Start is called before the first frame update
    void Start()
    {
        running = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!running && Random.Range(0, frequency) == 0) {
            running = true;
            startTime = Time.time;
            //transform.position = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-5.0f, 5.0f), Random.Range(-10.0f, 10.0f));
            effectInstance = Instantiate(projectiles[Random.Range(0, 5)], new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-5.0f, 5.0f), Random.Range(-10.0f, 10.0f)), Quaternion.identity);
        }
        if (running && Time.time - startTime > 1.0f) {
            Destroy(effectInstance);
            running = false;
        }
        //fireRingInstance = Instantiate(fireRing, transform.position, Quaternion.Euler(new Vector3(0, 90, 0));
    }
}
