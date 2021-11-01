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
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            
            StartCoroutine(startWave());
        }
    }
    public IEnumerator startWave()
    {
        enemyPath = pathFinder.getPath();
        //towerPlacer.enabled = false;
        //wait till path is finished
        while (pathFinder.getPath().Count == 0) {
            yield return new WaitForSeconds(.5f);
            enemyPath = pathFinder.getPath();
        }
        for (int i = 0; i < numEnemiesPerRound; i++)
        {
            GameObject newEnemy = Instantiate(enemy, enemyPath[0][0], new Quaternion());
            enemyStorage.addEnemy(newEnemy);
            newEnemy.GetComponent<EnemyMovement>().setPath(enemyPath);
            yield return new WaitForSeconds(1);
        }
    }
}
