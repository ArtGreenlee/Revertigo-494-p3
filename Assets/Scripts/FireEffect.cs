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
        if (AreaSparksEffect != null)
        {
            AreaSparksEffect = Instantiate(AreaSparksEffect, transform.position, new Quaternion(), transform);

            float size = AreaSparksEffect.GetComponent<ParticleSystem>().shape.radius;
            Vector3 rescale = AreaSparksEffect.GetComponent<ParticleSystem>().shape.scale;
            rescale = GetComponent<TowerStats>().range * rescale / size;
            AreaSparksEffect.transform.localScale = rescale;
        }
        

        TowerFireEffect = Instantiate(TowerFireEffect, transform.position, new Quaternion(), transform);
    }
}
