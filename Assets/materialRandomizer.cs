using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialRandomizer : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Material> materials;
    void Start()
    {
        if (materials.Count > 0)
        {
            GetComponent<MeshRenderer>().material = materials[Random.Range(0, materials.Count)];
            GetComponent<FlashOnHit>().originalMaterial = GetComponent<MeshRenderer>().material;
        }
    }   
}
