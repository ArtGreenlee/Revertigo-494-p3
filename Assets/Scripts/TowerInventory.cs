using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
public class TowerInventory : MonoBehaviour
{
    public AudioClip getTowerSFX;

    public static TowerInventory instance;

    public bool towerPlacementEnabled;
    public float inventoryDistanceFromPlayer;
    public float towerSnapSpeed;
    public List<GameObject> playerInventory;

    public List<GameObject> towerRoster;
    public List<GameObject> specialTowerRoster;
    private GoldStorage goldStorage;
    public float price;
    public TextMeshProUGUI priceText;

    public GameObject selectionDisplayEffect;
    private GameObject selectionDisplayEffectInstance;
    private TowerDisplay towerDisplay;
    private Transform cameraTransform;
    private TowerPlacer towerPlacer;
    public GameObject combineEffect;
    public int maxTowerInventory;
    public GameObject towerDestroyEffect;
    public float combineCooldown;
    private float combineCooldownUtility;

    public Dictionary<TowerStats.TowerName, GameObject> specialTowerDictionary;

    private PlayerInputControl playerInputControl;

    public bool selectionEnabled;

    public List<KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>> combinations;

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
        combineCooldownUtility = 0;
        playerInputControl = PlayerInputControl.instance;
        towerPlacer = TowerPlacer.instance;
        cameraTransform = Camera.main.transform;
        towerDisplay = TowerDisplay.instance;
        priceText.text = "Tower Cost " + price.ToString();
        goldStorage = GoldStorage.instance;
        playerInventory = new List<GameObject>();
        selectionEnabled = false;
        specialTowerDictionary = new Dictionary<TowerStats.TowerName, GameObject>();
        foreach (GameObject tower in specialTowerRoster)
        {
            TowerStats tempStats = tower.GetComponent<TowerStats>();
            if (tempStats.specialTower)
            {
                specialTowerDictionary.Add(tempStats.towerName, tower);
            }
        }

        combinations = new List<KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>>();

        List<KeyValuePair<int, TowerStats.TowerName>> temp = new List<KeyValuePair<int, TowerStats.TowerName>>();
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Blue));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Red));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.White));
        combinations.Add(new KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>(TowerStats.TowerName.Fireball,
            new List<KeyValuePair<int, TowerStats.TowerName>>(temp)));

        /*temp.Clear();
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Green));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Yellow));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Blue));
        combinations.Add(new KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>(TowerStats.TowerName.Fireball,
            new List<KeyValuePair<int, TowerStats.TowerName>>(temp)));
        */
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.G) && (goldStorage.gold >= price && playerInventory.Count < maxTowerInventory)))
        {
            combineCooldownUtility = Time.time;
            goldStorage.changeGoldAmount(-price);
            //price += 7;
            StartCoroutine(buyTower());
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

        if (Time.frameCount % 15 == 0 && Time.time - combineCooldownUtility > combineCooldown)
        {
            combineCooldownUtility = Time.time;
            checkRosterAndCombineTowers();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float degreesBetween = (2 * Mathf.PI) / playerInventory.Count;
        float distanceFromPlayer = inventoryDistanceFromPlayer * playerInventory.Count / 3 + 1.5f;
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
            if (playerInventory[i].GetComponent<TowerStats>().attachedToPlayer)
            {
                if (playerInputControl.movementEnabled || Vector3.Distance(playerInventory[i].transform.position, transform.position) < inventoryDistanceFromPlayer - .5f)
                {
                    playerInventory[i].transform.position = Vector3.Lerp(playerInventory[i].transform.position, position, towerSnapSpeed * Time.deltaTime);
                }
                else
                {
                    playerInventory[i].transform.position = Vector3.Lerp(playerInventory[i].transform.position, position, towerSnapSpeed * Time.deltaTime * .3f);
                }

            }

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

    private IEnumerator buyTower()
    {
        GameObject newTower = Instantiate(getRandomTower(), transform.position, Quaternion.identity);
        newTower.GetComponent<TowerStats>().attachedToPlayer = true;
        playerInventory.Insert(Random.Range(0, playerInventory.Count), newTower);
        newTower.GetComponent<Rigidbody>().angularVelocity = Random.onUnitSphere * .8f;
        AudioSource.PlayClipAtPoint(getTowerSFX, Camera.main.transform.position);

        /*yield return new WaitForSeconds(.4f);

        checkRosterAndCombineTowers();

        yield return new WaitForSeconds(.5f);

        while (checkRosterAndCombineTowers())
        {
            yield return new WaitForSeconds(1f);
            checkRosterAndCombineTowers();
            yield return new WaitForSeconds(1f);
        }*/

        selectionEnabled = true;
        if (selectionDisplayEffectInstance == null)
        {
            selectionDisplayEffectInstance = Instantiate(selectionDisplayEffect, playerInventory[0].transform.position, Quaternion.identity);
        }
        
        yield break;
    }

    public IEnumerator destroyPlayerInventory()
    {
        while (playerInventory.Count > 0)
        {
            for (int i = 0; i < playerInventory.Count; i++)
            {
                playerInventory[i].GetComponent<TowerStats>().attachedToPlayer = false;
                playerInventory[i].GetComponent<Rigidbody>().AddForce((transform.position - playerInventory[i].transform.position).normalized * 100);
                if (Vector3.Distance(playerInventory[i].transform.position, transform.position) < .4f)
                {
                    Instantiate(towerDestroyEffect, Vector3.Lerp(playerInventory[i].transform.position, transform.position, .5f), Quaternion.identity);
                    Destroy(playerInventory[i]);
                    playerInventory.RemoveAt(i);
                    i--;
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }

    private int pairListIndex(List<KeyValuePair<int, TowerStats.TowerName>> checkList, KeyValuePair<int, TowerStats.TowerName> value)
    {
        for (int i = 0; i < checkList.Count; i++)
        {
            KeyValuePair<int, TowerStats.TowerName> temp = checkList[i];
            if (value.Key == temp.Key && value.Value == temp.Value)
            {
                return i;
            }
        }
        return -1;
    }

    private bool checkRosterAndCombineTowers()
    {           
        for (int a = 0; a < playerInventory.Count; a++)
        {
            //check for special towers
            TowerStats checkTowerStats = playerInventory[a].GetComponent<TowerStats>();
            foreach (KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>> combination in combinations)
            {
                KeyValuePair<int, TowerStats.TowerName> checkTowerDesignation = new KeyValuePair<int, TowerStats.TowerName>(checkTowerStats.level, checkTowerStats.towerName);
                List<KeyValuePair<int, TowerStats.TowerName>> checkList = new List<KeyValuePair<int, TowerStats.TowerName>>(combination.Value);
                List<GameObject> foundTowers = new List<GameObject>();
                //Debug.Log(checkTowerDesignation);
                if (pairListIndex(checkList, checkTowerDesignation) != -1)
                {
                    foreach (GameObject tempTower in playerInventory)
                    {
                        if (tempTower != playerInventory[a])
                        {
                            TowerStats tempTowerStats = tempTower.GetComponent<TowerStats>();
                            KeyValuePair<int, TowerStats.TowerName> tempTowerDesignation = new KeyValuePair<int, TowerStats.TowerName>(tempTowerStats.level, tempTowerStats.towerName);
                            int checkListIndex = pairListIndex(checkList, tempTowerDesignation);
                            if (checkListIndex != -1)
                            {
                                //Debug.Log(tempTowerDesignation);
                                foundTowers.Add(tempTower);
                                checkList.RemoveAt(checkListIndex);
                            }
                        }
                    }

                    if (checkList.Count == 0)
                    {
                        Debug.Log("Special tower combination found");
                        //combine all the foundTowers into the combination.key
                        StartCoroutine(combineSpecialTower(combination.Key, playerInventory[a], foundTowers[0], foundTowers[1]));
                        return true;
                    }
                    //search the other towers for the two other towers in the combinations.
                }
            }


            /*for (int b = a; b < playerInventory.Count; b++)
            {
                if (a != b)
                {
                    GameObject towerA = playerInventory[a];
                    GameObject towerB = playerInventory[b];
                    TowerStats towerStatsA = towerA.GetComponent<TowerStats>();
                    TowerStats towerStatsB = towerB.GetComponent<TowerStats>();
                    if (canCombine(towerStatsA, towerStatsB))
                    {
                        StartCoroutine(combineTowers(towerA, towerB));
                        return true;
                    }
                }
            }*/
        }

        for (int a = 0; a < playerInventory.Count; a++)
        {
            for (int b = a; b < playerInventory.Count; b++)
            {
                if (a != b)
                {
                    GameObject towerA = playerInventory[a];
                    GameObject towerB = playerInventory[b];
                    TowerStats towerStatsA = towerA.GetComponent<TowerStats>();
                    TowerStats towerStatsB = towerB.GetComponent<TowerStats>();
                    if (canCombine(towerStatsA, towerStatsB))
                    {
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
        playerInventory.Remove(towerA);
        playerInventory.Remove(towerB);
        while ((towerA.transform.position - towerB.transform.position).sqrMagnitude > .25f)
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
        playerInventory.Insert(Random.Range(0, playerInventory.Count), towerA);
        yield return new WaitForSeconds(.5f);
    }

    public IEnumerator combineSpecialTower(TowerStats.TowerName towerName, GameObject towerA, GameObject towerB, GameObject towerC)
    {
        playerInventory.Remove(towerA);
        playerInventory.Remove(towerB);
        playerInventory.Remove(towerC);
        List<Rigidbody> rbList = new List<Rigidbody>();
        rbList.Add(towerA.GetComponent<Rigidbody>());
        rbList.Add(towerB.GetComponent<Rigidbody>());
        rbList.Add(towerC.GetComponent<Rigidbody>());
       
        while ((towerA.transform.position - towerB.transform.position).sqrMagnitude > .1f &&
            (towerA.transform.position - towerC.transform.position).sqrMagnitude > .1f &&
            (towerB.transform.position - towerC.transform.position).sqrMagnitude > .1f)
        {
            foreach (Rigidbody rb in rbList)
            {
                foreach (Rigidbody rb2 in rbList)
                {
                    if (rb != rb2)
                    {
                        rb.AddForce(rb2.transform.position - rb.transform.position);
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
        foreach (Rigidbody rb in rbList)
        {
            rb.velocity = Vector3.zero;
        }

        Vector3 inbetween = Vector3.Lerp(towerA.transform.position, towerB.transform.position, .5f);
        
        Instantiate(combineEffect, inbetween, Quaternion.identity);
        yield return new WaitForSeconds(.5f);
        Destroy(towerA);
        Destroy(towerB);
        Destroy(towerC);
        GameObject specialTower = Instantiate(specialTowerDictionary[towerName], inbetween, Quaternion.identity);
        playerInventory.Insert(Random.Range(0, playerInventory.Count), specialTower);

        yield break;
    }

    public bool canCombine(TowerStats towerStatsA, TowerStats towerStatsB)
    {
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

    /*public GameObject specialCanCombine(GameObject towerA, GameObject towerB, GameObject towerC)
    {
        TowerStats towerAstats = towerA.GetComponent<TowerStats>();
        TowerStats towerBstats = towerB.GetComponent<TowerStats>();
        TowerStats towerCstats = towerC.GetComponent<TowerStats>();
        foreach (KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>> combination in combinations)
        {
            KeyValuePair<int, TowerStats.TowerName> towerALevelName = new KeyValuePair<int, TowerStats.TowerName>(towerAstats.level, towerAstats.towerName);
            KeyValuePair<int, TowerStats.TowerName> towerBLevelName = new KeyValuePair<int, TowerStats.TowerName>(towerBstats.level, towerBstats.towerName);
            KeyValuePair<int, TowerStats.TowerName> towerCLevelName = new KeyValuePair<int, TowerStats.TowerName>(towerCstats.level, towerCstats.towerName);

            if (combination.Value.Contains(towerALevelName) && combination.Value.Contains(towerBLevelName) && combination.Value.Contains(towerCLevelName))
            {
                Debug.Log("special tower combination");
                return specialTowerDictionary[combination.Key];
            }
        }
        return null;
    }*/

    private GameObject getRandomTower()
    {
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
