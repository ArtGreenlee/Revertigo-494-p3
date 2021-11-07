using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    private Pathfinder pathFinder;
    private WallPlacer towerPlacer;
    private List<List<Vector3> > enemyPath;
    public int numEnemiesPerRound;
    private EnemyStorage enemyStorage;
    public float enemyStartingHealth;
    public float startDelay;
    public float startInterval;

    private void Awake()
    {
        pathFinder = GetComponent<Pathfinder>();
        towerPlacer = GameObject.Find("GameController").GetComponent<WallPlacer>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        StartCoroutine(startWave());
    }

    public IEnumerator startWave()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < numEnemiesPerRound; i++)
        {
            while (!pathFinder.pathFinished())
            {
                yield return new WaitForSeconds(.5f);
            }
            enemyPath = pathFinder.getPath();
            GameObject newEnemy = Instantiate(enemy, enemyPath[0][0], new Quaternion());
            enemyStorage.addEnemy(newEnemy);
            newEnemy.GetComponent<EnemyHealth>().setMaxHealth(enemyStartingHealth + i * 2);
            newEnemy.GetComponent<EnemyMovement>().path = enemyPath;
            yield return new WaitForSeconds(startInterval);
        }
    }
}
