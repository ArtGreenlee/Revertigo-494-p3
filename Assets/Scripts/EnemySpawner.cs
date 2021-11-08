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
    public int numEnemiesPerRound;
    private EnemyStorage enemyStorage;
    public float enemyStartingHealth;
    public float startDelay;
    public float startInterval;
    public TextMeshPro countDownText;

    private void Awake()
    {
        pathFinder = GetComponent<Pathfinder>();
        towerPlacer = GameObject.Find("GameController").GetComponent<WallPlacer>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
    }
    private IEnumerator Start()
    {
        if (startDelay > 0 && countDownText != null)
        {
            for (int i = 0; i < startDelay; i++)
            {
                countDownText.text = (startDelay - i).ToString();
                yield return new WaitForSecondsRealtime(1);
            }
            Destroy(countDownText);
        }
        
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
            GameObject newEnemy = Instantiate(enemy, enemyPath[0][0], Quaternion.identity);
            enemyStorage.addEnemy(newEnemy);
            newEnemy.GetComponent<EnemyHealth>().setMaxHealth(enemyStartingHealth + i * 8);
            newEnemy.GetComponent<EnemyMovement>().path = enemyPath;
            yield return new WaitForSeconds(startInterval);
        }
    }
}
