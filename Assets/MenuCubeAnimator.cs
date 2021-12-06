using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCubeAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    bool up = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(transform.position.ToString());
        transform.Rotate(Vector3.up* 6f * Time.deltaTime);
        if (!up && !(transform.position.y > 2)) {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.001f, transform.position.z);
        }
        if (!up && (transform.position.y > 2)) {
            up = true;
        }
        if (up && !(transform.position.y < 0)) {

            transform.position = new Vector3(transform.position.x, transform.position.y - 0.001f, transform.position.z);
        }
        if (up && (transform.position.y < 0)) {
            up = false;
        }
    }
}
