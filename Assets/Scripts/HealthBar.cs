using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform enemyTransform;
    private Transform cameraTransform;
    public EnemyHealth enemyHealth;
    private float maxWidth;
    public float healthBarOffset;
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }
    void Start()
    {
        maxWidth = transform.localScale.x;
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
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x * -1, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    public void showDamage()
    {
        if (enemyHealth != null)
        {
            transform.localScale = new Vector3((enemyHealth.currentHealth / enemyHealth.getMaxHealth()) * maxWidth, transform.localScale.y, transform.localScale.z);
        }
    }
}
