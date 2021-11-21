using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bullet;
    public AudioClip shootSFX;
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
            if (Time.time - cooldownUtility > towerStats.getCooldown())
            {
                GameObject tempBullet = objectPooler.getObjectFromPool("Bullet", transform.position, Quaternion.LookRotation(transform.forward));
                tempBullet.GetComponent<Rigidbody>().velocity = transform.forward * 30;
                tempBullet.GetComponent<BulletController>().towerStats = towerStats;
                rb.AddForce(transform.forward * -1 * knockBackForce, ForceMode.Impulse);
                AudioSource.PlayClipAtPoint(shootSFX, transform.position, 2);
                cooldownUtility = Time.time;
            }

            foreach (GameObject tower in towerInventory.playerInventory)
            {
                if (tower.GetComponent<TowerStats>().attachedToPlayer)
                {
                    ShootsBullets shootsBullets;
                    ShootsFireBalls shootsFireballs;
                    if (tower.TryGetComponent<ShootsBullets>(out shootsBullets))
                    {
                        shootsBullets.shootBullet(playerInputControl.currentLookPoint - tower.transform.position + Random.insideUnitSphere / 2);
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
}
