using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    private Pathfinder pathFinder;
    private TowerPlacer towerPlacer;
    private List<List<Vector3> > enemyPath;
    public int numEnemiesPerRound;
    private EnemyStorage enemyStorage;

    private void Awake()
    {
        pathFinder = GetComponent<Pathfinder>();
        towerPlacer = GameObject.Find("GameController").GetComponent<TowerPlacer>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
    }
    private void Start()
    {
        StartCoroutine(startWave());
    }

    public IEnumerator startWave()
    {
      
        for (int i = 0; i < numEnemiesPerRound; i++)
        {
            while (!pathFinder.pathFinished())
            {
                yield return new WaitForSeconds(.5f);
            }
            enemyPath = pathFinder.getPath();
            GameObject newEnemy = Instantiate(enemy, enemyPath[0][0], new Quaternion());
            enemyStorage.addEnemy(newEnemy);
            newEnemy.GetComponent<EnemyHealth>().maxHealth = 20 + i * 2;
            newEnemy.GetComponent<EnemyMovement>().path = enemyPath;
            yield return new WaitForSeconds(5);
        }
    }
}
