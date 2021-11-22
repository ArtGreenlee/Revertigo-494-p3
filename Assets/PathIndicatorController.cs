using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathIndicatorController : MonoBehaviour
{
    // Start is called before the first frame update

    public List<List<Vector3>> path;
    private Vector3 nextPoint;
    private int currentPointIndex;
    public float speed;
    private int pathIndex;
    void Start()
    {
        currentPointIndex = 0;
        pathIndex = 0;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (path != null && path.Count != 0 && path[pathIndex].Count != 0)
        {
            nextPoint = path[pathIndex][currentPointIndex];
            transform.position += (nextPoint - transform.position).normalized * Time.deltaTime * speed;
            if ((transform.position - nextPoint).sqrMagnitude < .01f)
            {
                currentPointIndex++;
                if (currentPointIndex == path[pathIndex].Count)
                {
                    currentPointIndex = 0;
                    pathIndex++;
                    if (pathIndex == path.Count)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

}
