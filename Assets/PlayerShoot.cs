using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bullet;
    private TowerStats towerStats;
    private float cooldownUtility;
    private void Awake()
    {
        towerStats = GetComponent<TowerStats>();
    }
    // Start is called before the first frame update
    void Start()
    {
        cooldownUtility = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time - cooldownUtility > towerStats.cooldown)
        {
            GameObject tempBullet = Instantiate(bullet, transform.position, new Quaternion());
            tempBullet.GetComponent<Rigidbody>().velocity = transform.forward * towerStats.bulletSpeed;
            tempBullet.GetComponent<BulletController>().towerStats = towerStats;
            cooldownUtility = Time.time;
        }
    }
}
