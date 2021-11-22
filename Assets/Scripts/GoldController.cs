using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldController : MonoBehaviour
{
    private PlayerInputControl player;
    private GoldStorage goldStorage;
    private Rigidbody rb;
    private MeshRenderer mesh;
    public float atttraction;
    public AudioClip goldCollectSFX;
    public float goldCollectLow = 0.1f;
    public float goldCollectHigh = 0.5f;
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        goldStorage = GoldStorage.instance;
        player = PlayerInputControl.instance;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mesh = GetComponent<MeshRenderer>();
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        rb.angularVelocity = Random.insideUnitSphere * 5;   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce((player.transform.position - transform.position).normalized * atttraction);
    }

    private void OnTriggerEnter(Collider other)
    {
        mesh.enabled = false;
        goldStorage.changeGoldAmount(1);
        StartCoroutine(AudioDelay());
    }

    IEnumerator AudioDelay() {
        source.PlayOneShot(goldCollectSFX, Random.Range(goldCollectLow, goldCollectHigh));
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);

    }
}
