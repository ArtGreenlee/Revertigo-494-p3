using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffsOtherTowers : MonoBehaviour
{
    private TowerStats towerStats;
    private TowerStorage towerStorage;
    private List<GameObject> buffTowers;

    public enum buffSelection
    {
        adjacent,
        inRange
    };

    public buffSelection buffSelectionType;
    private void Start()
    {
        buffTowers = new List<GameObject>();
        towerStorage = TowerStorage.instance;
        towerStats = GetComponent<TowerStats>();
    }
    private void Update()
    {
        if (Time.frameCount % 15 == 0)
        {
            if (!towerStats.attachedToPlayer)
            {
                updateBuffs();
            }
            else
            {
                resetBuffs();
            }
        }

    }

    public void resetBuffs()
    {
        for (int i = 0; i < buffTowers.Count; i++)
        {
            foreach (TowerStats.buffTypes buff in towerStats.buffs)
            {
                buffTowers[i].GetComponent<TowerStats>().removeBuff(buff, towerStats.cooldownBuffDecreaseAtLevel[towerStats.level]);
            }
        }
        buffTowers = new List<GameObject>();
    }
    private void updateBuffs()
    {
        Vector3 searchPoint = GetComponent<ShootsBullets>().snapPosition;
        ShootsBullets recoilCheck;
        if (TryGetComponent<ShootsBullets>(out recoilCheck))
        {
            if (recoilCheck.snapPosition != Vector3.zero)
            {
                searchPoint = recoilCheck.snapPosition;
            }
        }
        List<GameObject> towersInRange;
        if (buffSelectionType == buffSelection.adjacent)
        {
            towersInRange = towerStorage.getAllTowersAdjacentToPoint(searchPoint);
        }
        else
        {
            towersInRange = towerStorage.getAllTowersWithinRangeAtPoint(searchPoint, towerStats.range);
        }
        for (int i = 0; i < buffTowers.Count; i++)
        {
            if (!towersInRange.Contains(buffTowers[i]))
            {
                //remove tower;
                foreach (TowerStats.buffTypes buff in towerStats.buffs)
                {
                    buffTowers[i].GetComponent<TowerStats>().removeBuff(buff, towerStats.cooldownBuffDecreaseAtLevel[towerStats.level]);
                }
                buffTowers.RemoveAt(i);
                i--;
            }
        }
        foreach (GameObject tower in towersInRange)
        {
            TowerStats tempStats = tower.GetComponent<TowerStats>();
            if (!buffTowers.Contains(tower) && tower != gameObject && !tempStats.buffsTowers)
            {
                buffTowers.Add(tower);
            }
        }
        foreach (GameObject tower in buffTowers)
        {
            TowerStats tempStats = tower.GetComponent<TowerStats>();
            if (towerStats.buffs.Contains(TowerStats.buffTypes.cooldownBuff))
            {
                tempStats.buffTower(TowerStats.buffTypes.cooldownBuff, towerStats.cooldownBuffDecreaseAtLevel[towerStats.level]);
            }
            if (towerStats.buffs.Contains(TowerStats.buffTypes.damageBuff))
            {
                tempStats.buffTower(TowerStats.buffTypes.damageBuff, towerStats.damageBuff);
            }
        }
    }
}
