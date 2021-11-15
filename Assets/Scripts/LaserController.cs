using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public TowerStats towerStats;
    private float lifeTime = 2;
    private float lifeTimeStart;
    private ObjectPooler objectPooler;
    public GameObject target;


    // Start is called before the first frame update
    private void Awake()
    {
        objectPooler = ObjectPooler.Instance;
    }

    IEnumerator Start()
    {
        lifeTimeStart = Time.time;
        yield return new WaitForEndOfFrame();
        transform.position = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lifeTimeStart > lifeTime)
        {
            Destroy(gameObject);
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject, 1);
    }
}
