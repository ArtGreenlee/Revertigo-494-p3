using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerStats : MonoBehaviour
{
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

    public int numberOfBulletsPerShot;
    public enum shotSpread { Standard, Cone, SideBySide}
}
