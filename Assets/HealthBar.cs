using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform enemyTransform;
    private Transform cameraTransform;
    public EnemyHealth enemyHealth;
    private float maxWidth;
    private Image HealthBarImage;
    public float healthBarOffset;
    void Start()
    {
        HealthBarImage = GetComponent<Image>();
        maxWidth = HealthBarImage.rectTransform.sizeDelta.x;
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        Vector3 offsetVec = Vector3.zero;
        if (Mathf.Abs(enemyTransform.position.y) > 9)
        {
            offsetVec.z = healthBarOffset;
        }
        else
        {
            offsetVec.y = healthBarOffset;
        }
        transform.position = (enemyTransform.position + (enemyTransform.position - cameraTransform.position).normalized * -2) + offsetVec;
        transform.LookAt(cameraTransform);  
        HealthBarImage.rectTransform.sizeDelta = new Vector2((enemyHealth.currentHealth / enemyHealth.maxHealth) * maxWidth, HealthBarImage.rectTransform.sizeDelta.y);
    }
}
