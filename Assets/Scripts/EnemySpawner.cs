using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    private Pathfinder pathFinder;
    private TowerPlacer towerPlacer;
    private List<List<Vector3> > enemyPath;
    public int numEnemiesPerRound;
    private EnemyStorage enemyStorage;
<<<<<<< Updated upstream
=======
    public float enemyStartingHealth;
    public float startDelay;
    public float startInterval;
    public TextMeshPro countDownText;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes

    private void Awake()
    {
        pathFinder = GetComponent<Pathfinder>();
        towerPlacer = GameObject.Find("GameController").GetComponent<TowerPlacer>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
    }
    private void Start()
    {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
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
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
        enemyPath = pathFinder.getPath();
        //towerPlacer.enabled = false;
        //wait till path is finished
        while (pathFinder.getPath().Count == 0) {
            yield return new WaitForSeconds(.5f);
            enemyPath = pathFinder.getPath();
        }
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
        for (int i = 0; i < numEnemiesPerRound; i++)
        {
            GameObject newEnemy = Instantiate(enemy, enemyPath[0][0], new Quaternion());
            enemyStorage.addEnemy(newEnemy);
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            newEnemy.GetComponent<EnemyMovement>().setPath(enemyPath);
            yield return new WaitForSeconds(1);
=======
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            newEnemy.GetComponent<EnemyHealth>().setMaxHealth(enemyStartingHealth + i * 8);
            newEnemy.GetComponent<EnemyMovement>().path = enemyPath;
            yield return new WaitForSeconds(startInterval);
>>>>>>> Stashed changes
        }
    }
}
