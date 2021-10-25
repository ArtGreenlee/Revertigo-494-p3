using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    public bool waveStarted;
    private Pathfinder pathFinder;
    private TowerPlacer towerPlacer;
    private List<List<Vector3> > enemyPath;
    public int numEnemiesPerRound;
    public List<GameObject> enemies;

    private void Start()
    {
        pathFinder = GetComponent<Pathfinder>();
        towerPlacer = GetComponent<TowerPlacer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            
            StartCoroutine(startWave());
        }
    }
    public IEnumerator startWave()
    {
        enemyPath = pathFinder.getPath();
        towerPlacer.enabled = false;
        //wait till path is finished
        while (pathFinder.getPath().Count == 0) {
            yield return new WaitForSeconds(.5f);
            enemyPath = pathFinder.getPath();
        }
        for (int i = 0; i < numEnemiesPerRound; i++)
        {
            Debug.Log("spawn");
            Instantiate(enemy, enemyPath[0][0], new Quaternion());
            yield return new WaitForSeconds(1);
        }
    }
}
