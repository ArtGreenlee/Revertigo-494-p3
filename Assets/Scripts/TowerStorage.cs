using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStorage : MonoBehaviour
{
    

    public static TowerStorage instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private Dictionary<Vector3, GameObject> towers; // ONLY APPLIES TO THE TOWERS ON THE FIELD


    // Start is called before the first frame update
    void Start()
    {
        towers = new Dictionary<Vector3, GameObject>();
    }

    public void addTower(Vector3 placementPosition, GameObject tower)
    {
        if (towers.ContainsKey(placementPosition))
        {
            Debug.LogError("TOWER ALREADY IN INVENTORY");
        }
        towers.Add(placementPosition, tower);
    }

    public void removeTower(Vector3 placementPosition, GameObject tower)
    {
        if (!towers.ContainsValue(tower))
        {
            Debug.LogError("TOWER REMOVED NOT INVENTORY");
        }
        towers.Remove(placementPosition);
    }

    public List<GameObject> getAllTowersWithinRangeAtPoint(Vector3 point, float range)
    {
        List<GameObject> tempList = new List<GameObject>();
        float rangeSquared = range * range;
        foreach (Vector3 towerPosition in towers.Keys)
        {
            if ((towerPosition - point).sqrMagnitude <= rangeSquared)
            {
                tempList.Add(towers[towerPosition]);
            }
        }
        return tempList;
    }

    public List<GameObject> getAllTowersAdjacentToPoint(Vector3 point)
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (Vector3 direction in UtilityFunctions.sideVectors)
        {
            Vector3 checkPos = point + direction * 2;
            if (towers.ContainsKey(checkPos))
            {
                tempList.Add(towers[checkPos]);
            }
        }
        return tempList;
    }
}
