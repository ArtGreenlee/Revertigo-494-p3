using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    private Pathfinder pathFinder;
    private WallPlacer towerPlacer;
    private List<List<Vector3> > enemyPath;
    private EnemyStorage enemyStorage;
    public TextMeshPro countdownText;
    private int startDelay;
    private int numEnemies;

    public List<int> waveStartTimes;
    public List<int> enemyCountForWave;
    public List<int> enemyHealthForWave;
    public List<int> enemyValueForWave;
    private void Awake()
    {
        pathFinder = GetComponent<Pathfinder>();
        towerPlacer = GameObject.Find("GameController").GetComponent<WallPlacer>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
    }
    private IEnumerator Start()
    {
        for (int i = 0; i < enemyCountForWave.Count; i++)
        {
            startDelay = waveStartTimes[i];
            numEnemies = enemyCountForWave[i];
            for (int j = 0; j < startDelay; j++)
            {
                int timeRemaining = startDelay - j;
                if (timeRemaining <= 10)
                {
                    countdownText.text = (startDelay - j).ToString();
                }
                else
                {
                    countdownText.text = "";
                }
                yield return new WaitForSeconds(1);
            }
            StartCoroutine(startWave(numEnemies, enemyHealthForWave[i]));
        }   
    }

    public IEnumerator startWave(int spawnCount, float enemyHealth)
    {
        for (int i = 0; i < numEnemies; i++)
        {
            while (!pathFinder.pathFinished())
            {
                yield return new WaitForSeconds(.5f);
            }
            enemyPath = pathFinder.getPath();
            GameObject newEnemy = Instantiate(enemy, enemyPath[0][0], Quaternion.identity);
            enemyStorage.addEnemy(newEnemy);
            newEnemy.GetComponent<EnemyHealth>().setMaxHealth(enemyHealth);
            newEnemy.GetComponent<EnemyHealth>().goldValue = 3 + Mathf.RoundToInt(i / 6);
            newEnemy.GetComponent<EnemyMovement>().path = enemyPath;
            yield return new WaitForSeconds(1.25f);
        }
    }
}
