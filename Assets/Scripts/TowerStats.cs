using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStats : MonoBehaviour
{
    public List<float> killsToUpgrade;

    public List<float> slowPercentageAtLevel;

    public List<float> slowDurationAtLevel;

    public List<float> poisonDotAtLevel;

    public List<float> poisonDurationAtLevel;

    public List<float> damageIncreaseOnLevelUp;

    public List<float> cooldownDecreaseOnLevelUp;

    public List<float> aoeRangeIncreaseOnLevelUp;

    public List<float> cooldownBuffDecreaseAtLevel;

    public List<Mesh> towerLevelMeshList;

    public List<float> rangeAtLevel;
    
    public enum TowerName
    {
        Blue,
        Green,
        Red,
        White,
        Purple,
        Fireball,
        ClusterFireball,
        Stun,
        Spirit,
        Player,
        Yellow,
        Laser,
        Tourmaline
    };

    public TowerName towerName;

    public enum buffTypes
    {
        damageBuff,
        cooldownBuff
    };

    public GameObject cooldownBuffEffect;
    private GameObject cooldownBuffEffectInstance;
    public void buffTower(buffTypes buffType, float value)
    {

        if (buffType == buffTypes.cooldownBuff && !cooldownBuffs.Contains(value))
        {
            Vector3 buffLocation = transform.position;
            ShootsBullets bulletTryComponent;
            if (TryGetComponent<ShootsBullets>(out bulletTryComponent))
            {
                buffLocation = bulletTryComponent.snapPosition;
            }
            if (cooldownBuffEffectInstance != null)
            {
                Destroy(cooldownBuffEffectInstance);
            }
            cooldownBuffEffectInstance = Instantiate(cooldownBuffEffect,
                buffLocation,
                UtilityFunctions.getRotationawayFromSide(transform.position));
            cooldownBuffs.Add(value);
        }
    }

    public void removeBuff(buffTypes buffType, float value)
    {
        if (buffType == buffTypes.cooldownBuff && cooldownBuffs.Contains(value) && value != 1)
        {
            cooldownBuffs.Remove(value);
            if (cooldownBuffs.Min == 1)
            {
                Destroy(cooldownBuffEffectInstance);
            }
        }
    }

    public int kills = 0;
    public int level = 0;
    public SortedSet<float> damageBuffs;
    public float damageBuff;
    public bool attachedToPlayer = true;

    public bool buffsTowers;
    public List<buffTypes> buffs;

    public bool slowsEnemy;
    public bool canCriticallyHit;
    public bool aoe;
    public bool poisons;
    public float poisonDPS;
    public float poisonDuration;
    public float range;
    public int numTargets;

    public float baseDamageMin;
    public float baseDamageMax;

    public float getDamageMin()
    {
        return baseDamageMin * damageBuffs.Max;
    }

    public float getDamageMax()
    {
        return baseDamageMax * damageBuffs.Max;
    }

    public float getDamage()
    {
        if (attachedToPlayer)
        {
            return Random.Range(getDamageMin(), getDamageMax()) / 2;
        }
        return Random.Range(getDamageMin(), getDamageMax());
    }

    public float critChance;
    public float critMult;
    public float slowPercent;
    public float slowDuration;
    public float aoe_range;

    [SerializeField]
    private float baseCooldown;
    
    public float getCooldown()
    {
        return baseCooldown * cooldownBuffs.Min;
    }
        
    public Color trailRendererColor;
    public GameObject upgradeEffect;
    public bool specialTower;
    public GameObject slowEffect;
    private SortedSet<float> cooldownBuffs;

    private void Start()
    {
        cooldownBuffs = new SortedSet<float>();
        damageBuffs = new SortedSet<float>();
        damageBuffs.Add(1);
        cooldownBuffs.Add(1);
        if (gameObject.name != "Player")
        {
            GetComponent<TrailRenderer>().startColor = trailRendererColor;
        }
        GetComponent<Rigidbody>().angularVelocity = Random.onUnitSphere * .8f;
    }

    public void increaseKills()
    {
        if (this != null && gameObject != null && gameObject.name != "Player")
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
            if (tempLevel > level)
            {
                level = tempLevel;
                levelUp();
            }
        }
    }

    public void levelUp()
    {
        if (buffsTowers)
        {
            GetComponent<BuffsOtherTowers>().resetBuffs();
        }
        if (!specialTower)
        {
            range = rangeAtLevel[level];
            baseDamageMin *= damageIncreaseOnLevelUp[level];
            baseDamageMax *= damageIncreaseOnLevelUp[level];
            baseCooldown *= cooldownDecreaseOnLevelUp[level];
            if (slowsEnemy)
            {
                slowPercent = slowPercentageAtLevel[level];
                slowDuration = slowDurationAtLevel[level];
            }
            if (aoe)
            {
                aoe_range += aoeRangeIncreaseOnLevelUp[level];
            }
            if (poisons)
            {
                poisonDuration = poisonDurationAtLevel[level];
                poisonDPS = poisonDotAtLevel[level];
            }
            UtilityFunctions.changeScaleOfTransform(transform, transform.localScale.x + .1f);
            Instantiate(upgradeEffect, transform.position, Quaternion.identity);
            GetComponent<MeshFilter>().mesh = towerLevelMeshList[level];
        }
    }
}
