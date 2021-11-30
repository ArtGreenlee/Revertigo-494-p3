using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject bullet;
    public AudioClip shootSFX;
    public float shootLow = 0.3f;
    public float shootHigh = 0.4f;
    private AudioSource source;
    private TowerStats towerStats;
    private float cooldownUtility;
    private ObjectPooler objectPooler;
    private Rigidbody rb;
    public float knockBackForce;
    private TowerInventory towerInventory;
    private PlayerInputControl playerInputControl;
    //public GameObject playerShootEffect;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
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
                objectPooler.getObjectFromPool("OnShootEffect", transform.position + transform.forward, Quaternion.identity);
                //Instantiate(playerShootEffect, transform.position, Quaternion.identity);
                tempBullet.GetComponent<Rigidbody>().velocity = transform.forward * 30;
                tempBullet.GetComponent<BulletController>().towerStats = towerStats;
                rb.AddForce(transform.forward * -1 * knockBackForce, ForceMode.Impulse);
                float vol = Random.Range(shootLow, shootHigh);
                source.PlayOneShot(shootSFX, vol);
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
