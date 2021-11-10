using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuraBubbleAutodestruct : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        UtilityFunctions.changeScaleOfTransformOverTime(transform, 0, 10);
        Destroy(gameObject, 4);
    }
}
