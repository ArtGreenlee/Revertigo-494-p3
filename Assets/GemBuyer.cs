using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemBuyer : MonoBehaviour
{
    public int maxGemInventory;

    public bool debugMode;
    public bool towerPlacementEnabled;
    public float inventoryDistanceFromPlayer;

    private List<GameObject> PlayerInventory;

    public List<GameObject> debugRoster;

    public List<GameObject> towerRoster;
    // Start is called before the first frame update
    void Start()
    {
        PlayerInventory = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GameObject newTower = Instantiate(getRandomTower(), transform.position, new Quaternion());
            PlayerInventory.Add(newTower);
            newTower.GetComponent<Rigidbody>().angularVelocity = Random.onUnitSphere * .5f;
        }

        float degreesBetween = (2 * Mathf.PI) / PlayerInventory.Count;
        for (int i = 0; i < PlayerInventory.Count; i++)
        {
            PlayerInventory[i].transform.position = new Vector3(transform.position.x + Mathf.Cos(i * degreesBetween) * inventoryDistanceFromPlayer,
                transform.position.y + Mathf.Sin(i * degreesBetween) * inventoryDistanceFromPlayer, 
                transform.position.z);
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
