using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldController : MonoBehaviour
{
    private PlayerInputControl player;
    private Rigidbody rb;
    public float atttraction;
    private GoldStorage goldStorage;

    // Start is called before the first frame update
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        goldStorage = GoldStorage.instance;
        player = PlayerInputControl.instance;
    }

    private void OnEnable()
    {
        rb.angularVelocity = Random.insideUnitSphere * 4;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce((player.transform.position - transform.position).normalized * atttraction);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("coin inactive");
        goldStorage.changeGoldAmount(1);

        gameObject.SetActive(false);
    }
}
