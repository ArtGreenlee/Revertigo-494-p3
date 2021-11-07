using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStats : MonoBehaviour
{
    private int kills = 0;
    private int level = 0;
    public bool automaticallyShoots = false;
    public bool slowsEnemy;
    public bool canCriticallyHit;
    public bool aoe;
    public float range;
    public int numTargets;
    public float damageMin;
    public float damageMax;
    public float critChance;
    public float critMult;
    public float slowPercent;
    public float slowDuration;
    public float aoe_range;
    public float cooldown;
    public float bulletSpeed;
    public Color trailRendererColor;
    public GameObject upgradeEffect;
    public bool specialTower;

    public List<Mesh> towerLevelMeshList;

    public int numberOfBulletsPerShot;
    public enum shotSpread { Standard, Cone, SideBySide }

    private void Start()
    {
        if (gameObject.name != "Player")
        {
            GetComponent<TrailRenderer>().startColor = trailRendererColor;
        }
    }

    public void increaseKills()
    {
        if (gameObject.name != "Player")
        {
            kills++;
            int tempLevel = level;
            for (int i = 0; i < UtilityFunctions.killsToUpgrade.Count; i++)
            {
                if (kills >= UtilityFunctions.killsToUpgrade[i])
                {
                    tempLevel = i + 1;
                }
            }
            //Debug.Log(tempLevel);
            if (tempLevel != level)
            {
                level = tempLevel;
                if (!specialTower)
                {
                    UtilityFunctions.changeScaleOfTransform(transform, transform.localScale.x + .1f);
                    Instantiate(upgradeEffect, transform.position, new Quaternion());
                    GetComponent<MeshFilter>().mesh = towerLevelMeshList[level];
                }
                //upgrade the tower;
            }
        }
        
    } 
}
