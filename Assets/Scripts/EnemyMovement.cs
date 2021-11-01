using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class EnemyMovement : MonoBehaviour
{
    private Vector3 nextPoint;
    private int currentPointIndex;
    public List<List<Vector3>> path;
    public float maxSpeed;
    private float curSpeed;
    public GameObject slowEffect;
    private int pathIndex;
    private Pathfinder pathFinder;

    private void Awake()
    {
        pathFinder = GetComponent<Pathfinder>();
    }
    void Start()
    {
        curSpeed = maxSpeed;
        currentPointIndex = 0;
        pathIndex = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            ShootsBullets stats = collision.gameObject.GetComponent<BulletController>().parent;
            if (stats.slowsEnemy)
            {
                if (stats.canCriticallyHit)
                {
                    if (Random.Range(0f, 1f) > stats.critChance)
                    {
                        slowEnemy(stats.slowPercent, stats.slowDuration);
                    }
                }
                else
                {
                    slowEnemy(stats.slowPercent, stats.slowDuration);
                }
            }
        }
    }

    public void resetPath()
    {
        pathFinder.checkPointVectors = new List<Vector3>();
        pathFinder.checkPointVectors.Add(UtilityFunctions.snapVector(transform.position));
        for (int i = pathIndex; i < path.Count; i++)
        {
            pathFinder.checkPointVectors.Add(path[i][path[i].Count - 1]);
        }
        path = null;
        pathFinder.findPath();
        StartCoroutine(waitForPathToFinish());
    }

    private IEnumerator waitForPathToFinish()
    {
        while (!pathFinder.pathFinished())
        {
            yield return new WaitForSeconds(.5f); //totally arbitrary
        }
        path = pathFinder.getPath();
        currentPointIndex = 0;
        pathIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (path != null)
        {
            nextPoint = path[pathIndex][currentPointIndex];
            transform.position += (nextPoint - transform.position).normalized * Time.deltaTime * curSpeed;
            if (Vector3.Distance(transform.position, nextPoint) < .1f)
            {
                currentPointIndex++;
                if (currentPointIndex == path[pathIndex].Count)
                {
                    currentPointIndex = 0;
                    pathIndex++;
                    if (pathIndex == path.Count)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                }
            }
        }
    }

    public void slowEnemy(float slowPercentage, float slowDuration)
    {
        if (slowPercentage < (curSpeed / maxSpeed))
        {
            StartCoroutine(slowEnemyCoroutine(slowPercentage, slowDuration));
        }
    }

    private IEnumerator slowEnemyCoroutine(float slowPercentage, float slowDuration)
    {
        Instantiate(slowEffect, transform.position, new Quaternion());
        curSpeed *= slowPercentage;
        yield return new WaitForSeconds(slowDuration);
        curSpeed = maxSpeed;
    }

}
