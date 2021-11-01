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
    public float pathResetThreshold;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            ShootsBullets stats = other.gameObject.GetComponent<BulletController>().parent;
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
        else if (other.gameObject.CompareTag("Wall"))
        {
            resetPath();
        }
    }

    public void resetPath()
    {
        if (path != null)
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
    void FixedUpdate()
    {
        if (path != null && path.Count != 0 && path[pathIndex].Count != 0)
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
                        Destroy(gameObject);
                        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                }
            }
            if (Vector3.Distance(transform.position, nextPoint) > pathResetThreshold)
            {
                Debug.Log("enemy path threshold reset");
                resetPath();
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
