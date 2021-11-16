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
        if (Input.GetKeyDown(KeyCode.G) && (playerInventory.Count == 0 && goldStorage.gold >= price) || debugMode)
        {
            goldStorage.changeGoldAmount(-price);
            price += 5;
            priceText.text = "Tower Cost " + price.ToString();
            StartCoroutine(buyRoundOfGems());
            selectionEnabled = true;
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
        for (int i = 0; i < playerInventory.Count; i++)
        {
            Vector3 vecRotation = transform.rotation.eulerAngles;
            Vector3 position = transform.position;

            float xDiff = inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween + vecRotation.z * Mathf.Deg2Rad) * Mathf.Cos(vecRotation.y * Mathf.Deg2Rad);
            float yDiff = inventoryDistanceFromPlayer * Mathf.Sin(i * degreesBetween + vecRotation.z * Mathf.Deg2Rad) * Mathf.Cos(vecRotation.x * Mathf.Deg2Rad);
            float zDiffHorizontal = inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween + vecRotation.z * Mathf.Deg2Rad) * Mathf.Sin(vecRotation.y * Mathf.Deg2Rad) * -1;
            float zDiffVertical = inventoryDistanceFromPlayer * Mathf.Sin(i * degreesBetween + vecRotation.z * Mathf.Deg2Rad) * Mathf.Sin(vecRotation.x * Mathf.Deg2Rad);


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
            if (Input.GetKey(KeyCode.Space))
            {
                playerInventory[i].transform.position = Vector3.Slerp(playerInventory[i].transform.position, position, towerSnapSpeed * Time.deltaTime * .1f);
            }
            else
            {
                playerInventory[i].transform.position = Vector3.Lerp(playerInventory[i].transform.position, position, towerSnapSpeed * Time.deltaTime);
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

    private IEnumerator buyRoundOfGems()
    {

        for (int i = 0; i < 6; i++)
        {
            GameObject newTower = Instantiate(getRandomTower(), transform.position, Quaternion.identity);
            newTower.GetComponent<TowerStats>().attachedToPlayer = true;
            playerInventory.Add(newTower);
            newTower.GetComponent<Rigidbody>().angularVelocity = Random.onUnitSphere * .5f;
            AudioSource.PlayClipAtPoint(getTowerSFX, Camera.main.transform.position);
            yield return new WaitForSeconds(.3f);
        }
        selectionDisplayEffectInstance = Instantiate(selectionDisplayEffect, playerInventory[0].transform.position, Quaternion.identity);
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
        return tower;
    }
}
