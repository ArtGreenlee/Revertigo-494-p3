using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    public GameObject rangeIndicator;

    private void OnMouseDown()
    {
        rangeIndicator = Instantiate(rangeIndicator, transform.position, new Quaternion());
    }
}
