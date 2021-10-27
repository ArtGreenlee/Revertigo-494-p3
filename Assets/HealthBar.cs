using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform enemyTransform;
    private Transform cameraTransform;
    void Start()
    {
        transform.SetParent(enemyTransform);
    }

    private void Update()
    {
        
        transform.LookAt(cameraTransform);
    }

    public void showDamage()
    {

    }
}
