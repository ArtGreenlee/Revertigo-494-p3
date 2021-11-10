using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStats : MonoBehaviour
{
    public static List<float> killsToUpgrade = new List<float>
    {
        5,
        10,
        15,
        20,
        30
    };

    public static List<float> slowPercentageAtLevel = new List<float>
    {
        .9f,
        .85f,
        .8f,
        .7f,
        .6f
    };

    public static List<float> damageIncreaseAtLevel = new List<float>
    {
        1.1f,
        1.1f,
        1.1f,
        1.1f,
        1.1f
    };

    public static List<float> cooldownDecreaseAtLevel = new List<float>
    {
        .9f,
        .9f,
        .9f,
        .9f,
        .9f
    };

    public static List<float> aoeRangeIncreasePerLevel = new List<float>
    {
        .5f,
        .5f,
        .5f,
        .5f,
        .5f
    };

    public enum towerNames
    {
        Blue,
        Green,
        Red,
        White,
        Fireball,
        ClusterFireball,
        StunTower,
        SpiritTower
    };

    private int kills = 0;
    private int level = 0;
    public bool attachedToPlayer = true;
    public bool slowsEnemy;
    public bool canCriticallyHit;
    public bool aoe;
    public bool poisons;
    public float poisonDPS;
    public float poisonDuration;
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
    public GameObject slowEffect;

    public List<Mesh> towerLevelMeshList;

    public int numberOfBulletsPerShot;
    //public enum shotSpread { Standard, Cone, SideBySide }

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
            for (int i = 0; i < killsToUpgrade.Count; i++)
            {
                if (kills >= killsToUpgrade[i])
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
                    damageMin *= damageIncreaseAtLevel[level];
                    damageMax *= damageIncreaseAtLevel[level];
                    cooldown *= .9f;
                    if (slowsEnemy)
                    {
                        if (slowPercent > 0)
                        {
                            slowPercent = slowPercentageAtLevel[level];
                        }
                    }
                    if (aoe)
                    {
                        aoe_range += aoeRangeIncreasePerLevel[level];
                    }
                    UtilityFunctions.changeScaleOfTransform(transform, transform.localScale.x + .1f);
                    Instantiate(upgradeEffect, transform.position, Quaternion.identity);
                    GetComponent<MeshFilter>().mesh = towerLevelMeshList[level];
                }
                //upgrade the tower;
            }
        }
        
    } 
}
