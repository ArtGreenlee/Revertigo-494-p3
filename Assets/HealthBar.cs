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
    void Start()
    {
        HealthBarImage = GetComponent<Image>();
        maxWidth = HealthBarImage.rectTransform.sizeDelta.x;
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {

        transform.position = (enemyTransform.position + (enemyTransform.position - cameraTransform.position).normalized * -2) + new Vector3(0, .3f, 0);
        transform.LookAt(cameraTransform);
        Debug.Log(HealthBarImage.rectTransform.sizeDelta);
        HealthBarImage.rectTransform.sizeDelta = new Vector2((enemyHealth.currentHealth / enemyHealth.maxHealth) * maxWidth, HealthBarImage.rectTransform.sizeDelta.y);
    }
}
