using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterFireballController : MonoBehaviour
{

    private Rigidbody rb;
    public GameObject splitEffect;
    private EnemyStorage enemyStorage;
    public GameObject fireBall;
    public float speed;
    public float damage;
    public float aoeRange;

    private void Awake()
    {
        enemyStorage = GameObject.Find("GameController").GetComponent<EnemyStorage>();
        rb = GetComponent<Rigidbody>();
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        Vector3 side = UtilityFunctions.getClosestSide(transform.position);
        foreach (Vector3 sideDirection in UtilityFunctions.sideVectors)
        {
            GameObject tempRocket = Instantiate(fireBall, transform.position, Quaternion.LookRotation(sideDirection));
            yield break;
        }
    }
}
