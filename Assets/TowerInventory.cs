using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerInventory : MonoBehaviour
{
    public int maxGemInventory;

    public static TowerInventory instance;

    public bool debugMode;
    public bool towerPlacementEnabled;
    public float inventoryDistanceFromPlayer;
    public float towerSnapSpeed;
    public List<GameObject> playerInventory;

    public List<GameObject> debugRoster;

    public List<GameObject> towerRoster;

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
        playerInventory = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && playerInventory.Count < maxGemInventory)
        {
            GameObject newTower = Instantiate(getRandomTower(), transform.position, new Quaternion());
            newTower.transform.localScale = new Vector3(newTower.transform.localScale.x / 2, newTower.transform.localScale.y / 2, newTower.transform.localScale.z / 2);
            playerInventory.Add(newTower);
            newTower.GetComponent<Rigidbody>().angularVelocity = Random.onUnitSphere * .5f;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameObject rotateTemp = playerInventory[playerInventory.Count - 1];
            playerInventory.RemoveAt(playerInventory.Count - 1);
            playerInventory.Insert(0, rotateTemp);
        }
        else if (Input.GetKeyDown(KeyCode.E))
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
            position.x += inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween) * Mathf.Cos(vecRotation.y * Mathf.Deg2Rad);
            position.z -= inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween) * Mathf.Sin(vecRotation.y * Mathf.Deg2Rad);
            
            //position.z += inventoryDistanceFromPlayer * Mathf.Sin(i * degreesBetween) * Mathf.Sin(vecRotation.x * Mathf.Deg2Rad);


            position.y += inventoryDistanceFromPlayer * Mathf.Sin(i * degreesBetween) * Mathf.Cos(vecRotation.x * Mathf.Deg2Rad);
            //position.z += inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween) * Mathf.Cos(vecRotation.x * Mathf.Deg2Rad);
            playerInventory[i].transform.position = Vector3.Slerp(playerInventory[i].transform.position, position, towerSnapSpeed * Time.deltaTime);
            //PlayerInventory[i].transform.position = position;
        }
    }

    private GameObject getRandomTower()
    {
        if (debugMode)
        {
            return debugRoster[Random.Range(0, debugRoster.Count)];
        }
        return towerRoster[Random.Range(0, towerRoster.Count)];
    }
}
