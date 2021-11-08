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
    private GoldStorage goldStorage;
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
        goldStorage = GoldStorage.instance;
        playerInventory = new List<GameObject>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && playerInventory.Count < maxGemInventory && goldStorage.gold >= 10)
        {
            goldStorage.changeGoldAmount(-10);
            GameObject newTower = Instantiate(getRandomTower(), transform.position, Quaternion.identity);
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

            float xDiff = inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween) * Mathf.Cos(vecRotation.y * Mathf.Deg2Rad);
            float yDiff = inventoryDistanceFromPlayer * Mathf.Sin(i * degreesBetween) * Mathf.Cos(vecRotation.x * Mathf.Deg2Rad);
            float zDiffHorizontal = inventoryDistanceFromPlayer * Mathf.Cos(i * degreesBetween) * Mathf.Sin(vecRotation.y * Mathf.Deg2Rad) * -1;
            float zDiffVertical = inventoryDistanceFromPlayer * Mathf.Sin(i * degreesBetween) * Mathf.Sin(vecRotation.x * Mathf.Deg2Rad);

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
