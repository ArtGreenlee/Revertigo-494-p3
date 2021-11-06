using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bullet;
    private TowerStats towerStats;
    private float cooldownUtility;
    private ObjectPooler objectPooler;
    private Rigidbody rb;
    public float knockBackForce;
    private TowerInventory towerInventory;
    private PlayerInputControl playerInputControl;
    private void Awake()
    {
        playerInputControl = GetComponent<PlayerInputControl>();
        towerInventory = GetComponent<TowerInventory>();
        rb = GetComponent<Rigidbody>();
        
        towerStats = GetComponent<TowerStats>();
    }


    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        cooldownUtility = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (Time.time - cooldownUtility > towerStats.cooldown)
            {
                GameObject tempBullet = objectPooler.getObjectFromPool("Bullet", transform.position, new Quaternion());
                tempBullet.GetComponent<Rigidbody>().velocity = transform.forward * towerStats.bulletSpeed;
                tempBullet.GetComponent<BulletController>().towerStats = towerStats;
                rb.AddForce(transform.forward * -1 * knockBackForce, ForceMode.Impulse);
                cooldownUtility = Time.time;
            }

            foreach (GameObject tower in towerInventory.playerInventory)
            {
                ShootsBullets shootsBullets;
                ShootsFireBalls shootsFireballs;
                if (tower.TryGetComponent<ShootsBullets>(out shootsBullets))
                {
                    shootsBullets.shootBullet(playerInputControl.currentLookPoint - tower.transform.position + Random.insideUnitSphere / 1.5f);
                }
                else if (tower.TryGetComponent<ShootsFireBalls>(out shootsFireballs))
                {
                    shootsFireballs.ShootFireball(transform.forward);
                    shootsFireballs.controlledByPlayer = true;
                }
            }
        }
    }
}
