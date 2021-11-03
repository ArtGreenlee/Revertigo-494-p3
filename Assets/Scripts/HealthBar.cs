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
    private void Awake()
    {
        HealthBarImage = GetComponent<Image>();
        cameraTransform = Camera.main.transform;
    }
    void Start()
    {
        maxWidth = HealthBarImage.rectTransform.sizeDelta.x;
    }

    private void Update()
    {
        if (enemyTransform == null)
        {
            Destroy(gameObject);
            return;
        }
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
    }

    public void showDamage()
    {
        if (enemyHealth != null)
        {
            HealthBarImage.rectTransform.sizeDelta = new Vector2(enemyHealth.currentHealth / enemyHealth.getMaxHealth() * maxWidth, HealthBarImage.rectTransform.sizeDelta.y);
        }
    }
}
