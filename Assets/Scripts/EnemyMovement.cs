using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Vector3 nextPoint;
    private int currentPointIndex;
    private List<Vector3> path;
    private Pathfinder pathFinder;

    // Start is called before the first frame update
    void Start()
    {
        pathFinder = GameObject.Find("GameController").GetComponent<Pathfinder>();
        path = new List<Vector3>();
        currentPointIndex = 1;
        List<List<Vector3>> pathIn = pathFinder.getPath();
        foreach (List<Vector3> pathTemp in pathIn)
        {
            path.AddRange(pathTemp);
        }
        nextPoint = path[currentPointIndex];
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (nextPoint - transform.position).normalized * Time.deltaTime * 1;
        if (Vector3.Distance(transform.position, nextPoint) < .1f)
        {
            if (currentPointIndex == path.Count - 1)
            {
                Destroy(gameObject);
            }
            nextPoint = path[currentPointIndex++];
        }
    }


}
