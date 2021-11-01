using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Vector3 nextPoint;
    private int currentPointIndex;
    private List<Vector3> path;
    public float maxSpeed;
    public float curSpeed;
    public GameObject slowEffect;

    void Start()
    {
        curSpeed = maxSpeed;
        currentPointIndex = 1;
        nextPoint = path[currentPointIndex];
    }

    public void setPath(List<List<Vector3>> pathIn)
    {
        path = new List<Vector3>();
        foreach (List<Vector3> pathTemp in pathIn)
        {
            path.AddRange(pathTemp);
        }
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

    // Update is called once per frame
    void Update()
    {
        transform.position += (nextPoint - transform.position).normalized * Time.deltaTime * curSpeed;
        if (Vector3.Distance(transform.position, nextPoint) < .1f)
        {
            if (currentPointIndex == path.Count - 1)
            {
                Destroy(gameObject);
            }
            nextPoint = path[currentPointIndex++];
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
