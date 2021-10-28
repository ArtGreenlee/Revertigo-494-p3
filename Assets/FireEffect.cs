using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffect : MonoBehaviour
{
    public GameObject AreaSparksEffect;
    public GameObject TowerFireEffect;
    // Start is called before the first frame update
    void Start()
    {
        AreaSparksEffect = Instantiate(AreaSparksEffect, transform.position, new Quaternion(), transform);
        TowerFireEffect = Instantiate(TowerFireEffect, transform.position, new Quaternion(), transform);
    }
}
