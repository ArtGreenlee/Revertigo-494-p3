using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TowerInventory : MonoBehaviour
{
    public int maxGemInventory;
    public AudioClip getTowerSFX;

    public static TowerInventory instance;

    public bool debugMode;
    public bool towerPlacementEnabled;
    public float inventoryDistanceFromPlayer;
    public float towerSnapSpeed;
    public List<GameObject> playerInventory;

    public List<GameObject> debugRoster;

    public List<GameObject> towerRoster;
    private GoldStorage goldStorage;
    public float price;
    public TextMeshProUGUI priceText;

    public GameObject selectionDisplayEffect;
    private GameObject selectionDisplayEffectInstance;
    private TowerDisplay towerDisplay;
    private Transform cameraTransform;
    private TowerPlacer towerPlacer;
    public GameObject combineEffect;
    public int gemsPerRound;
    public GameObject towerDestroyEffect;

    private PlayerInputControl playerInputControl;

    public bool selectionEnabled;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerInputControl = PlayerInputControl.instance;
        towerPlacer = TowerPlacer.instance;
        cameraTransform = Camera.main.transform;
        towerDisplay = TowerDisplay.instance;
        priceText.text = "Tower Cost " + price.ToString();
        goldStorage = GoldStorage.instance;
        playerInventory = new List<GameObject>();
        selectionEnabled = false;
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.G) && (goldStorage.gold >= price && !selectionEnabled)) || debugMode)
        {
            foreach (GameObject tower in playerInventory)
            {
                Destroy(tower);
            }
            if (selectionDisplayEffectInstance != null)
            {
                Destroy(selectionDisplayEffectInstance);
            }
            playerInventory.Clear();
            goldStorage.changeGoldAmount(-price);
            price += 7;
            priceText.text = "Tower Cost " + price.ToString();
            StartCoroutine(buyRoundOfGems());
            
        }

        if (Input.GetKeyDown(KeyCode.Q) && selectionEnabled)
        {
            GameObject rotateTemp = playerInventory[playerInventory.Count - 1];
            playerInventory.RemoveAt(playerInventory.Count - 1);
            playerInventory.Insert(0, rotateTemp);
        }
        else if (Input.GetKeyDown(KeyCode.E) && selectionEnabled)
        {
            GameObject rotateTemp = playerInventory[0];
            playerInventory.RemoveAt(0);
            playerInventory.Add(rotateTemp);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float degreesBetween = (2 * Mathf.PI) / playerInventory.Count;
        float distanceFromPlayer = inventoryDistanceFromPlayer * playerInventory.Count / 3 + .5f;
        for (int i = 0; i < playerInventory.Count; i++)
        {
            Vector3 vecRotation = transform.rotation.eulerAngles;
            Vector3 position = transform.position;

            float xDiff = distanceFromPlayer * Mathf.Cos(i * degreesBetween + vecRotation.z * Mathf.Deg2Rad) * Mathf.Cos(vecRotation.y * Mathf.Deg2Rad);
            float yDiff = distanceFromPlayer * Mathf.Sin(i * degreesBetween + vecRotation.z * Mathf.Deg2Rad) * Mathf.Cos(vecRotation.x * Mathf.Deg2Rad);
            float zDiffHorizontal = distanceFromPlayer * Mathf.Cos(i * degreesBetween + vecRotation.z * Mathf.Deg2Rad) * Mathf.Sin(vecRotation.y * Mathf.Deg2Rad) * -1;
            float zDiffVertical = distanceFromPlayer * Mathf.Sin(i * degreesBetween + vecRotation.z * Mathf.Deg2Rad) * Mathf.Sin(vecRotation.x * Mathf.Deg2Rad);


            //i dont know why, I shouldnt HAVE to know why, but for whatever god awful reason this is the closest i can get to the desired behaviour. 

            //float zDiffHorizontal = xDiff;
            //float zDiffVertical = yDiff;
            //position.z += zDiffHorizontal - zDiffVertical;
            //.Log(zDiffHorizontal);

            position.z += zDiffHorizontal + zDiffVertical;
            position.x += xDiff;
            position.y += yDiff;


            //position.z += inventoryDistanceFromPlayer * Mathf.Sin(i * degreesBetween) * Mathf.Cos(i * degreesBetween) * Mathf.Cos(vecRotation.x * Mathf.Deg2Rad) * Mathf.Cos(vecRotation.y * Mathf.Deg2Rad);

            /*position.x += inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween) * Mathf.Sin(vecRotation.y * Mathf.Deg2Rad);
            position.x -= inventoryDistanceFromPlayer * Mathf.Sin(i * degreesBetween) * Mathf.Cos(vecRotation.y * Mathf.Deg2Rad);

            position.z += inventoryDistanceFromPlayer * Mathf.Sin(i * degreesBetween) * Mathf.Cos(vecRotation.x * Mathf.Deg2Rad);
            position.z -= inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween) * Mathf.Sin(vecRotation.y * Mathf.Deg2Rad);

            position.y += inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween) * Mathf.Sin(vecRotation.x * Mathf.Deg2Rad);
            position.y -= inventoryDistanceFromPlayer * Mathf.Sin(i * degreesBetween) * Mathf.Cos(vecRotation.x * Mathf.Deg2Rad);
            */

            //position.z += inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween) * Mathf.Cos(vecRotation.x * Mathf.Deg2Rad);

            playerInventory[i].transform.position = Vector3.Lerp(playerInventory[i].transform.position, position, towerSnapSpeed * Time.deltaTime);
            //PlayerInventory[i].transform.position = position;
        }

        if (playerInventory.Count > 0 && selectionDisplayEffectInstance != null && selectionEnabled)
        {
            selectionDisplayEffectInstance.transform.position = playerInventory[0].transform.position;
            selectionDisplayEffectInstance.transform.rotation = Quaternion.LookRotation(playerInventory[0].transform.position - cameraTransform.position);
        }
        else if (selectionDisplayEffectInstance != null)
        {
            Destroy(selectionDisplayEffectInstance);
        }
    }

    private IEnumerator buyRoundOfGems()
    {
        for (int i = 0; i < gemsPerRound; i++)
        {
            GameObject newTower = Instantiate(getRandomTower(), transform.position, Quaternion.identity);
            newTower.GetComponent<TowerStats>().attachedToPlayer = true;
            playerInventory.Add(newTower);
            newTower.GetComponent<Rigidbody>().angularVelocity = Random.onUnitSphere * .8f;
            AudioSource.PlayClipAtPoint(getTowerSFX, Camera.main.transform.position);
            yield return new WaitForSeconds(.4f);
            checkRosterAndCombineTowers();
        }

        yield return new WaitForSeconds(.5f);

        while (checkRosterAndCombineTowers())
        {
            yield return new WaitForSeconds(2f);
        }

        selectionEnabled = true;
        selectionDisplayEffectInstance = Instantiate(selectionDisplayEffect, playerInventory[0].transform.position, Quaternion.identity);
    }

    public IEnumerator destroyPlayerInventory()
    {
        Debug.Log("destroy invent");
        while (playerInventory.Count > 0)
        {
            for (int i = 0; i < playerInventory.Count; i++)
            {
                playerInventory[i].GetComponent<Rigidbody>().AddForce((transform.position - playerInventory[i].transform.position).normalized * 10);
                if (Vector3.Distance(playerInventory[i].transform.position, transform.position) < .4f)
                {
                    Instantiate(towerDestroyEffect, Vector3.Lerp(playerInventory[i].transform.position, transform.position, .5f), Quaternion.identity);
                    Destroy(playerInventory[i]);
                    playerInventory.RemoveAt(i);
                    i--;
                }
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private bool checkRosterAndCombineTowers()
    {
        for (int a = 0; a < playerInventory.Count; a++)
        {
            for (int b = a; b < playerInventory.Count; b++)
            {
                if (a != b)
                {
                    GameObject towerA = playerInventory[a];
                    GameObject towerB = playerInventory[b];
                    if (canCombine(towerA, towerB))
                    {
                        playerInventory.Remove(towerA);
                        playerInventory.Remove(towerB);
                        StartCoroutine(combineTowers(towerA, towerB));
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public IEnumerator combineTowers(GameObject towerA, GameObject towerB)
    {
        while (Vector3.Distance(towerA.transform.position, towerB.transform.position) > .5f)
        {
            towerB.transform.position = Vector3.Lerp(towerB.transform.position, towerA.transform.position, 3 * Time.deltaTime);
            towerB.transform.position = Vector3.Lerp(towerB.transform.position, cameraTransform.transform.position, .5f * Time.deltaTime);
            towerA.transform.position = Vector3.Lerp(towerA.transform.position, towerB.transform.position, 3 * Time.deltaTime);
            towerA.transform.position = Vector3.Lerp(towerA.transform.position, cameraTransform.transform.position, .5f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        TowerStats towerAStats = towerA.GetComponent<TowerStats>();
        TowerStats towerBStats = towerB.GetComponent<TowerStats>();
        towerAStats.kills += towerBStats.kills;
        towerAStats.level++;
        Vector3 inbetween = Vector3.Lerp(towerA.transform.position, towerB.transform.position, .5f);
        Instantiate(combineEffect, inbetween, Quaternion.identity);
        yield return new WaitForSeconds(.5f);
        Destroy(towerB);
        towerAStats.levelUp();
        towerA.transform.position = inbetween;
        yield return new WaitForSeconds(.5f);
        playerInventory.Add(towerA);
        yield return new WaitForSeconds(.5f);

    }

    public bool canCombine(GameObject towerA, GameObject towerB)
    {
        TowerStats towerStatsA = towerA.GetComponent<TowerStats>();
        TowerStats towerStatsB = towerB.GetComponent<TowerStats>();
        if (!towerStatsA.specialTower &&
            !towerStatsB.specialTower &&
            towerStatsA.level == towerStatsB.level &&
            towerStatsA.towerName == towerStatsB.towerName &&
            towerStatsA.level != towerStatsA.killsToUpgrade.Count - 1)
        {
            return true;
        }
        return false;
    }

    private GameObject getRandomTower()
    {
        if (debugMode)
        {
            return debugRoster[Random.Range(0, debugRoster.Count)];
        }
        GameObject tower = towerRoster[Random.Range(0, towerRoster.Count)];
        if (tower.GetComponent<TowerStats>().specialTower)
        {
            tower = towerRoster[Random.Range(0, towerRoster.Count)];
        }
        if (tower.GetComponent<TowerStats>().specialTower)
        {
            tower = towerRoster[Random.Range(0, towerRoster.Count)];
        }
        if (playerInventory.Count > 0)
        {
            while (tower.GetComponent<TowerStats>().towerName == playerInventory[playerInventory.Count - 1].GetComponent<TowerStats>().towerName)
            {
                tower = towerRoster[Random.Range(0, towerRoster.Count)];
            }
        }
        return tower;
    }
}
