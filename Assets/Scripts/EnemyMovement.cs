using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Vector3 nextPoint;
    private int currentPointIndex;
    public List<List<Vector3>> path;
    public float maxSpeed;
    private float curSpeed;
    private int pathIndex;
    private Pathfinder pathFinder;
    public float pathResetThreshold;
    public GameObject onPathFinishEffect;
    public AudioClip loseLifeSFX;
    bool lookingForPath;
    private PlayerLivesTemp playerLifes;
    private Transform cameraTransform;

    private WallStorage wallStorage;

    private void Awake()
    {
        pathFinder = GetComponent<Pathfinder>();
    }
    void Start()
    {
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        wallStorage = WallStorage.instance;
        playerLifes = PlayerLivesTemp.instance;
        curSpeed = maxSpeed;
        currentPointIndex = 0;
        pathIndex = 0;
        lookingForPath = false;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            TowerStats stats = other.gameObject.GetComponent<BulletController>().towerStats;
            if (stats.slowsEnemy)
            {
                if (stats.canCriticallyHit)
                {
                    if (Random.Range(0f, 1f) < stats.critChance)
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
    }*/

    public void resetPath()
    {
        if (path != null && !lookingForPath)
        {
            lookingForPath = true;
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
        lookingForPath = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (path != null && path.Count != 0 && path[pathIndex].Count != 0)
        {
            nextPoint = path[pathIndex][currentPointIndex];
            /*if (wallStorage.isWall(nextPoint))
            {
                resetPath();
                return;
            }*/
            transform.position += (nextPoint - transform.position).normalized * Time.deltaTime * curSpeed;
            if ((transform.position - nextPoint).sqrMagnitude < .01f)
            {
                currentPointIndex++;
                if (currentPointIndex == path[pathIndex].Count)
                {
                    currentPointIndex = 0;
                    pathIndex++;
                    if (pathIndex == path.Count)
                    {
                        Instantiate(onPathFinishEffect, transform.position, Quaternion.identity);
                        pathFinder.StopAllCoroutines();
                        AudioSource.PlayClipAtPoint(loseLifeSFX, cameraTransform.position);
                        playerLifes.loseLife();
                        Destroy(gameObject);
                    }
                }
            }
            if ((transform.position - nextPoint).sqrMagnitude > pathResetThreshold * pathResetThreshold)
            {
                resetPath();
            }
        }
    }

    public void slowEnemy(float slowPercentage, float slowDuration, GameObject slowEffect)
    {
        if (slowPercentage < (curSpeed / maxSpeed))
        {
            StartCoroutine(slowEnemyCoroutine(slowPercentage, slowDuration, slowEffect));
        }
    }

    private IEnumerator slowEnemyCoroutine(float slowPercentage, float slowDuration, GameObject slowEffect)
    {
        
        if (slowEffect != null)
        {
            Instantiate(slowEffect, transform.position, Quaternion.identity);
        }        
        curSpeed *= slowPercentage;
        yield return new WaitForSeconds(slowDuration);
        curSpeed = maxSpeed;
       
    }

}
