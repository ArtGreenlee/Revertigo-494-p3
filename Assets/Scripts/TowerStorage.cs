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

    private List<GameObject> towers; // ONLY APPLIES TO THE TOWERS ON THE FIELD


    // Start is called before the first frame update
    void Start()
    {
        towers = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addTower(GameObject tower)
    {
        if (towers.Contains(tower))
        {
            Debug.LogError("TOWER ALREADY IN INVENTORY");
        }
        towers.Add(tower);
    }

    public void removeTower(GameObject tower)
    {
        if (!towers.Contains(tower))
        {
            Debug.LogError("TOWER REMOVED NOT INVENTORY");
        }
        towers.Remove(tower);
    }

    public List<GameObject> getAllTowersWithinRangeAtPoint(Vector3 point, float range)
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (GameObject tower in towers)
        {
            Vector3 towerPosition = tower.transform.position;
            ShootsBullets recoilCheck;
            if (TryGetComponent<ShootsBullets>(out recoilCheck))
            {
                if (recoilCheck.snapPosition != Vector3.zero)
                {
                    towerPosition = recoilCheck.snapPosition;
                }
            }

            if ((tower.transform.position - point).sqrMagnitude <= range * range)
            {
                tempList.Add(tower);
            }
        }
        return tempList;
    }

}
