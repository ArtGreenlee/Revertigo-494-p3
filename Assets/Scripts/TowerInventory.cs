using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
public class TowerInventory : MonoBehaviour
{
    public AudioClip getTowerSFX;
    public float getTowerVol = 0.2f;
    private AudioSource source;

    public static TowerInventory instance;

    public bool towerPlacementEnabled;
    public float inventoryDistanceFromPlayer;
    public float towerSnapSpeed;
    public List<GameObject> playerInventory;

    public List<GameObject> towerRoster;
    public List<GameObject> specialTowerRoster;
    private GoldStorage goldStorage;
    public float basePrice;
    public float price;
    public TextMeshProUGUI priceText;

    public GameObject selectionDisplayEffect;
    private GameObject selectionDisplayEffectInstance;
    private Transform cameraTransform;
    public GameObject combineEffect;
    public int maxTowerInventory;
    public float combineCooldown;
    private float combineCooldownUtility;
    public LineRenderer lr;
    public int combinationLrIndex;
    public List<List<GameObject>> combinations;
    public GameObject removeEffect;
    private bool purchaseEnabled;

    public Dictionary<TowerStats.TowerName, GameObject> specialTowerDictionary;

    private PlayerInputControl playerInputControl;

    public bool selectionEnabled;

    public List<KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>> specialTowerCombinations;

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
        price = basePrice;
        purchaseEnabled = true;
        combinationLrIndex = 0;
        source = GetComponent<AudioSource>();
        lr = GetComponent<LineRenderer>();
        combinations = new List<List<GameObject>>();
        combineCooldownUtility = 0;
        playerInputControl = PlayerInputControl.instance;
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        priceText.text = "Tower Cost " + price.ToString();
        goldStorage = GoldStorage.instance;
        playerInventory = new List<GameObject>();
        selectionEnabled = false;
        specialTowerDictionary = new Dictionary<TowerStats.TowerName, GameObject>();
        foreach (GameObject tower in specialTowerRoster)
        {
            TowerStats tempStats = tower.GetComponent<TowerStats>();
            if (tempStats.specialTower && !specialTowerDictionary.ContainsKey(tempStats.towerName))
            {
                specialTowerDictionary.Add(tempStats.towerName, tower);
            }
        }

        specialTowerCombinations = new List<KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>>();

        List<KeyValuePair<int, TowerStats.TowerName>> temp = new List<KeyValuePair<int, TowerStats.TowerName>>();
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Blue));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Red));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.White));
        specialTowerCombinations.Add(new KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>(TowerStats.TowerName.Fireball,
            new List<KeyValuePair<int, TowerStats.TowerName>>(temp)));

        temp.Clear();
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Green));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Yellow));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Blue));
        specialTowerCombinations.Add(new KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>(TowerStats.TowerName.Spirit,
            new List<KeyValuePair<int, TowerStats.TowerName>>(temp)));

        temp.Clear();
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Fireball));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Fireball));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Fireball));
        specialTowerCombinations.Add(new KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>(TowerStats.TowerName.ClusterFireball,
            new List<KeyValuePair<int, TowerStats.TowerName>>(temp)));

        temp.Clear();
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Purple));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Yellow));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.White));
        specialTowerCombinations.Add(new KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>(TowerStats.TowerName.Tourmaline,
            new List<KeyValuePair<int, TowerStats.TowerName>>(temp)));

        temp.Clear();
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Blue));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Purple));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Green));
        specialTowerCombinations.Add(new KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>(TowerStats.TowerName.Stun,
            new List<KeyValuePair<int, TowerStats.TowerName>>(temp)));

        temp.Clear();
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Red));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Red));
        temp.Add(new KeyValuePair<int, TowerStats.TowerName>(0, TowerStats.TowerName.Yellow));
        specialTowerCombinations.Add(new KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>>(TowerStats.TowerName.AOE,
            new List<KeyValuePair<int, TowerStats.TowerName>>(temp)));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && goldStorage.gold >= price && playerInventory.Count < maxTowerInventory && purchaseEnabled)
        {
            combineCooldownUtility = Time.time;
            goldStorage.changeGoldAmount(-price);
            price++;
            priceText.text = "Tower Cost " + price.ToString();
            buyTower();

            combinations = checkCurrentTowerForCombinations();
            combinationLrIndex = 0;
            if (combinations.Count > 0)
            {
                lr.positionCount = combinations[combinationLrIndex].Count;
            }
            else
            {
                lr.positionCount = 0;
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Q) && selectionEnabled)
        {
            GameObject rotateTemp = playerInventory[playerInventory.Count - 1];
            playerInventory.RemoveAt(playerInventory.Count - 1);
            playerInventory.Insert(0, rotateTemp);
            combinations = checkCurrentTowerForCombinations();
            combinationLrIndex = 0;
            if (combinations.Count > 0)
            {
                lr.positionCount = combinations[combinationLrIndex].Count;
            }
            else
            {
                lr.positionCount = 0;
            }
        }
        else if (Input.GetKeyDown(KeyCode.E) && selectionEnabled)
        {
            GameObject rotateTemp = playerInventory[0];
            playerInventory.RemoveAt(0);
            playerInventory.Add(rotateTemp);
            combinations = checkCurrentTowerForCombinations();
            combinationLrIndex = 0;
            if (combinations.Count > 0)
            {
                lr.positionCount = combinations[combinationLrIndex].Count;
            }
            else
            {
                lr.positionCount = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && combinations.Count > 0)
        {
            if (combinationLrIndex == 0)
            {
                combinationLrIndex = combinations.Count - 1;
            }
            else
            {
                combinationLrIndex--;
            }
            lr.positionCount = combinations[combinationLrIndex].Count;
        }
        /*else if (Input.GetKey(KeyCode.D) && combinations.Count > 0)
        {
            if (combinationLrIndex == combinations.Count - 1)
            {
                combinationLrIndex = 0;
            }
            else
            {
                combinationLrIndex++;
            }
            lr.positionCount = combinations[combinationLrIndex].Count;
        }*/

        if (Time.frameCount % 15 == 0 && Time.time - combineCooldownUtility > combineCooldown)
        {
            combineCooldownUtility = Time.time;
            //checkRosterAndCombineTowers();
        }

        if (combinations != null && combinations.Count > 0 && combinations[combinationLrIndex].Count > 1)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (combinations[combinationLrIndex].Count == 2)
                {
                    price--;
                    priceText.text = "Tower Cost " + price.ToString();
                    StartCoroutine(combineTowers(combinations[combinationLrIndex][0], combinations[combinationLrIndex][1]));
                }
                else if (combinations[combinationLrIndex].Count == 3)
                {
                    price -= 2;
                    priceText.text = "Tower Cost " + price.ToString();
                    StartCoroutine(combineSpecialTower(combinations[combinationLrIndex][0], combinations[combinationLrIndex][1], combinations[combinationLrIndex][2]));
                }
                if (playerInventory.Count > 0)
                {
                    combinations = checkCurrentTowerForCombinations();
                }
                else
                {
                    combinations = new List<List<GameObject>>();
                    lr.positionCount = 0;
                }
                combinationLrIndex = 0;
                if (combinations.Count > 0)
                {
                    lr.positionCount = combinations[combinationLrIndex].Count;
                }
                else
                {
                    lr.positionCount = 0;
                }
            }
            else if (lr.positionCount > 2)
            {
                for (int i = 0; i < combinations[combinationLrIndex].Count - 1; i++)
                {
                    lr.SetPosition(i, combinations[combinationLrIndex][i].transform.position);
                    lr.SetPosition(i + 1, combinations[combinationLrIndex][i + 1].transform.position);
                }
            }
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
            if (playerInventory[i] != null && playerInventory[i].GetComponent<TowerStats>().attachedToPlayer)
            {
                if (playerInputControl.movementEnabled || Vector3.Distance(playerInventory[i].transform.position, transform.position) < inventoryDistanceFromPlayer - .5f)
                {
                    playerInventory[i].transform.position = Vector3.Lerp(playerInventory[i].transform.position, position, towerSnapSpeed * Time.deltaTime);
                }
                else
                {
                    playerInventory[i].transform.position = Vector3.Lerp(playerInventory[i].transform.position, position, towerSnapSpeed * Time.deltaTime);
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

    private void buyTower()
    {
        GameObject newTower = Instantiate(getRandomTower(), transform.position, Quaternion.identity);
        newTower.GetComponent<TowerStats>().attachedToPlayer = true;
        addTowerToInventory(newTower);
        newTower.GetComponent<Rigidbody>().angularVelocity = Random.onUnitSphere * .8f;
        source.PlayOneShot(getTowerSFX, getTowerVol);

        selectionEnabled = true;
        if (selectionDisplayEffectInstance == null)
        {
            selectionDisplayEffectInstance = Instantiate(selectionDisplayEffect, playerInventory[0].transform.position, Quaternion.identity);
        }
    }

    public IEnumerator destroyPlayerInventory()
    {
        purchaseEnabled = false;
        lr.positionCount = 0;
        if (selectionDisplayEffectInstance != null)
        {
            Destroy(selectionDisplayEffectInstance);
        }
        for (int i = 0; i < playerInventory.Count; i++)
        {
            Instantiate(removeEffect, playerInventory[i].transform.position, Quaternion.identity);
            Destroy(playerInventory[i]);
            yield return new WaitForSeconds(.2f);
        }
        playerInventory.Clear();
        purchaseEnabled = true;
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

    public List<List<GameObject>> checkCurrentTowerForCombinations()
    {
        List<List<GameObject>> combinations = new List<List<GameObject>>();
        TowerStats checkTowerStats = playerInventory[0].GetComponent<TowerStats>();
        foreach (KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>> combination in specialTowerCombinations)
        {
            KeyValuePair<int, TowerStats.TowerName> checkTowerDesignation = new KeyValuePair<int, TowerStats.TowerName>(checkTowerStats.level, checkTowerStats.towerName);
            List<KeyValuePair<int, TowerStats.TowerName>> checkList = new List<KeyValuePair<int, TowerStats.TowerName>>(combination.Value);
            List<GameObject> foundTowers = new List<GameObject>();
            //Debug.Log(checkTowerDesignation);
            if (pairListIndex(checkList, checkTowerDesignation) != -1)
            {
                foreach (GameObject tempTower in playerInventory)
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

                if (checkList.Count == 0 && foundTowers.Count == 3)
                {
                    combinations.Add(new List<GameObject>{ foundTowers[0], foundTowers[1], foundTowers[2] });
                }
            }
        }
        for (int b = 1; b < playerInventory.Count; b++)
        {
            GameObject towerB = playerInventory[b];
            TowerStats towerStatsB = towerB.GetComponent<TowerStats>();
            if (canCombine(checkTowerStats, towerStatsB))
            {
                combinations.Add(new List<GameObject> { playerInventory[0], towerB });
            }
        }
        return combinations;
    }

    private bool checkRosterAndCombineTowers()
    {           
        for (int a = 0; a < playerInventory.Count; a++)
        {
            TowerStats checkTowerStats = playerInventory[a].GetComponent<TowerStats>();
            foreach (KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>> combination in specialTowerCombinations)
            {
                KeyValuePair<int, TowerStats.TowerName> checkTowerDesignation = new KeyValuePair<int, TowerStats.TowerName>(checkTowerStats.level, checkTowerStats.towerName);
                List<KeyValuePair<int, TowerStats.TowerName>> checkList = new List<KeyValuePair<int, TowerStats.TowerName>>(combination.Value);
                List<GameObject> foundTowers = new List<GameObject>(); 
                if (pairListIndex(checkList, checkTowerDesignation) != -1)
                {
                    foreach (GameObject tempTower in playerInventory)
                    {
                        TowerStats tempTowerStats = tempTower.GetComponent<TowerStats>();
                        KeyValuePair<int, TowerStats.TowerName> tempTowerDesignation = new KeyValuePair<int, TowerStats.TowerName>(tempTowerStats.level, tempTowerStats.towerName);
                        int checkListIndex = pairListIndex(checkList, tempTowerDesignation);
                        if (checkListIndex != -1)
                        {
                            foundTowers.Add(tempTower);
                            checkList.RemoveAt(checkListIndex);
                        }   
                    }

                    if (checkList.Count == 0 && foundTowers.Count == 3)
                    {
                        //StartCoroutine(combineSpecialTower(combination.Key, foundTowers[2], foundTowers[0], foundTowers[1]));
                        return true;
                    }
                }
            }
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
            towerB.transform.position = Vector3.Lerp(towerB.transform.position, cameraTransform.transform.position, 1.5f * Time.deltaTime);
            towerA.transform.position = Vector3.Lerp(towerA.transform.position, towerB.transform.position, 3 * Time.deltaTime);
            towerA.transform.position = Vector3.Lerp(towerA.transform.position, cameraTransform.transform.position, 1.5f * Time.deltaTime);
            yield return new WaitForFixedUpdate();
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
        addTowerToInventory(towerA);
        //playerInventory.Insert(Random.Range(0, playerInventory.Count), towerA);
        yield return new WaitForSeconds(.5f);
    }

    public IEnumerator combineTowerOnPodium(GameObject podiumTower, GameObject inventoryTower)
    {
        playerInventory.Remove(inventoryTower);
        while ((podiumTower.transform.position - inventoryTower.transform.position).sqrMagnitude > .25f)
        {
            inventoryTower.transform.position = Vector3.Lerp(inventoryTower.transform.position, podiumTower.transform.position,3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        TowerStats towerAStats = podiumTower.GetComponent<TowerStats>();
        TowerStats towerBStats = inventoryTower.GetComponent<TowerStats>();
        towerAStats.kills += towerBStats.kills;
        towerAStats.level++;
        Vector3 inbetween = Vector3.Lerp(podiumTower.transform.position, inventoryTower.transform.position, .5f);
        Instantiate(combineEffect, inbetween, Quaternion.identity);
        yield return new WaitForSeconds(.5f);
        Destroy(inventoryTower);
        towerAStats.levelUp();
    }

    public IEnumerator combineSpecialTower(GameObject towerA, GameObject towerB, GameObject towerC)
    {
        List<TowerStats> towerList = new List<TowerStats> { towerA.GetComponent<TowerStats>() , towerB.GetComponent<TowerStats>(), towerC.GetComponent<TowerStats>() };
        TowerStats.TowerName towerName = TowerStats.TowerName.Blue;
        bool foundSpecialTower = false;
        foreach (KeyValuePair<TowerStats.TowerName, List<KeyValuePair<int, TowerStats.TowerName>>> combination in specialTowerCombinations)
        {
            bool match = true;
            for (int j = 0; j < towerList.Count; j++)
            {
                if (pairListIndex(combination.Value, new KeyValuePair<int, TowerStats.TowerName>(towerList[j].level, towerList[j].towerName)) == -1)
                {
                    match = false;
                }
            }
            if (match)
            {
                Debug.Log("found");
                foundSpecialTower = true;
                towerName = combination.Key;
                break;
            }
        }
        if (!foundSpecialTower)
        {
            Debug.LogError("special combination not found");
            yield break;
        }
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
                rb.AddForce(cameraTransform.position - rb.transform.position);
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
        addTowerToInventory(specialTower);
    }

    public static bool canCombine(TowerStats towerStatsA, TowerStats towerStatsB)
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

    private void addTowerToInventory(GameObject tower)
    {
        tower.GetComponent<TowerStats>().attachedToPlayer = true;
        int addIndex = 0;
        if (playerInventory.Count > 0)
        {
            do
            {
                addIndex = Random.Range(1, playerInventory.Count + 1);
            } while (addIndex == 0);
        }
        playerInventory.Insert(addIndex, tower);
    }

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
