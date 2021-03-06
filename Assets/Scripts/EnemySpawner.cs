using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnemySpawner : MonoBehaviour
{
    public GameObject tutorial;
    private TutorialControl tutorialController;
    public GameObject enemy;
    private Pathfinder pathFinder;
    private WallPlacer towerPlacer;
    public List<List<Vector3> > enemyPath;
    private EnemyStorage enemyStorage;
    public TextMeshPro countdownText;
    public GameObject pathIndicator;
    private List<GameObject> pathIndicatorList;
    private float pathIndicatorSpawnInvervalUtility;
    private int startDelay;
    private int numEnemies;
    private AudioSource source;
    public AudioClip upcomingWaveSFX;
    public TextMeshProUGUI spawnEarlyText;
    public Image spawnIcon;
    public GameObject waveStartEffect;
    private ScoreCounter scoreCounter;

    // public AudioClip waveStartingSFX;
    public float waveVol = 0.1f;

    public List<int> waveStartTimes;
    public List<int> enemyCountForWave;
    public List<int> enemyHealthForWave;
    public List<int> enemyValueForWave;
    private void Awake()
    {

        pathIndicatorSpawnInvervalUtility = Time.time;
        pathIndicatorList = new List<GameObject>();
        pathFinder = GetComponent<Pathfinder>();
        towerPlacer = GameObject.Find("GameController").GetComponent<WallPlacer>();
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        source = GetComponent<AudioSource>();
        tutorialController = tutorial.GetComponent<TutorialControl>();
    }

    private void OnMouseOver()
    {
        spawnEarlyText.enabled = true;
        spawnIcon.color = new Color(spawnIcon.color.r, spawnIcon.color.g, spawnIcon.color.b, 1);
        if (Input.GetMouseButtonDown(0) && (TutorialControl.instance.isFinished || !TutorialControl.instance.isActiveAndEnabled))
        {
            scoreCounter.score += startDelay * 100;
            startDelay = 0;
            Instantiate(waveStartEffect, transform.position, Quaternion.identity);
        }
    }

    private void OnMouseExit()
    {
        spawnEarlyText.enabled = false;
        spawnIcon.color = new Color(spawnIcon.color.r, spawnIcon.color.g, spawnIcon.color.b, .2f);
    }

    public void resetPathIndicators()
    {
        foreach (GameObject pathIndicator in pathIndicatorList)
        {
            Destroy(pathIndicator);
        }
        pathIndicatorList.Clear();
    }

    private IEnumerator Start()
    {
        scoreCounter = ScoreCounter.instance;
        while(!tutorialController.isFinished && tutorialController.isActiveAndEnabled) {
            countdownText.text = "";
            yield return new WaitForSeconds(1);
        }
        enemyPath = pathFinder.getPath();
        for (int i = 0; i < enemyCountForWave.Count; i++)
        {
            startDelay = waveStartTimes[i];
            numEnemies = enemyCountForWave[i];
            bool SFXPlaying = false;
            for (int j = 0; j < startDelay; j++)
            {
                int timeRemaining = startDelay - j;
                if (timeRemaining <= 10)
                {
                    if (SFXPlaying) {
                        SFXPlaying = false;
                    }
                    if (!SFXPlaying) {
                        SFXPlaying = true;
                        source.PlayOneShot(upcomingWaveSFX, waveVol);
                    }
                    countdownText.text = (startDelay - j).ToString();
                }
                else
                {
                    countdownText.text = "";
                }
                yield return new WaitForSeconds(1);
            }
            StartCoroutine(startWave(numEnemies, enemyHealthForWave[i], enemyValueForWave[i]));
        }
        Destroy(countdownText);

        //no more waves. just spawn a bunch of enemies i guess

        while (true)
        {
            while (!pathFinder.pathFinished())
            {
                yield return new WaitForSeconds(.5f);
            }
            GameObject newEnemy = Instantiate(enemy, enemyPath[0][0], Quaternion.identity);
            enemyStorage.addEnemy(newEnemy);
            newEnemy.GetComponent<EnemyHealth>().setMaxHealth(1000);
            newEnemy.GetComponent<EnemyHealth>().goldValue = 2;
            newEnemy.GetComponent<EnemyMovement>().path = enemyPath;
            yield return new WaitForSeconds(2);
        }
        
    }

    private void Update()
    {
        if (enemyPath != null && enemyPath.Count > 0 && Time.time - pathIndicatorSpawnInvervalUtility > 12)
        {
            pathIndicatorSpawnInvervalUtility = Time.time;
            Instantiate(pathIndicator, enemyPath[0][0], Quaternion.identity).GetComponent<PathIndicatorController>().path = enemyPath;
        }
    }

    public IEnumerator startWave(int spawnCount, float enemyHealth, int goldValue)
    {
        for (int i = 0; i < spawnCount; i++)
        {
            while (!pathFinder.pathFinished())
            {
                yield return new WaitForSeconds(.5f);
            }
            GameObject newEnemy = Instantiate(enemy, enemyPath[0][0], Quaternion.identity);
            enemyStorage.addEnemy(newEnemy);
            newEnemy.GetComponent<EnemyHealth>().setMaxHealth(enemyHealth);
            newEnemy.GetComponent<EnemyHealth>().goldValue = 3 + Mathf.RoundToInt(i / 6);
            newEnemy.GetComponent<EnemyMovement>().path = enemyPath;
            yield return new WaitForSeconds(1.25f);
        }
    }
}
