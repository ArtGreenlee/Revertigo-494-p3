using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritTower : MonoBehaviour
{
    private EnemyStorage enemyStorage;
    private TowerStats towerStats;
    private ParticleSystem spiritSystem;
    private GameObject target;
    private ParticleSystem.Particle[] m_Particles;
    private PlayerInputControl playerControl;

    private float emissionRate;

    private void Start()
    {
        spiritSystem = GetComponent<ParticleSystem>();
        m_Particles = new ParticleSystem.Particle[spiritSystem.main.maxParticles];

        playerControl = PlayerInputControl.instance;
        towerStats = GetComponent<TowerStats>();
        enemyStorage = EnemyStorage.instance;

    }

    // Update is called once per frame
    void Update()
    {
        if (!towerStats.attachedToPlayer && (target == null || (transform.position - target.transform.position).sqrMagnitude > towerStats.range * towerStats.range))
        {
            target = enemyStorage.getClosestEnemyToPointWithinRange(transform.position, towerStats.range);
        }

        if (towerStats.attachedToPlayer)
        {
            transform.rotation.SetLookRotation(playerControl.currentLookPoint);
            if (Input.GetKeyDown(0))
            {
                enableParticles();
            }
            else
            {
                disableParticles();
            }
        }
        else
        {
            enableParticles();
            transform.rotation = UtilityFunctions.getRotationawayFromSide(transform.position);
        }

        if (target != null)
        {
            int numParticlesAlive = spiritSystem.GetParticles(m_Particles);

            //Debug.Log(numParticlesAlive);
            // Change only the particles that are alive
            for (int i = 0; i < numParticlesAlive; i++)
            {
                m_Particles[i].position = Vector3.Slerp(m_Particles[i].position, target.transform.position, Time.deltaTime * 5 * (1 / (m_Particles[i].position - target.transform.position).sqrMagnitude));
            }

            // Apply the particle changes to the Particle System
            spiritSystem.SetParticles(m_Particles, numParticlesAlive);
        }
    }

    public void enableParticles()
    {
        if (!spiritSystem.emission.enabled)
        {
            spiritSystem.Play();
        }
    }

    public void disableParticles()
    {
        if (spiritSystem.emission.enabled)
        {
            spiritSystem.Play();
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log("test");
        other.gameObject.GetComponent<EnemyHealth>().takeDamage(.5f, true);
    }
}
