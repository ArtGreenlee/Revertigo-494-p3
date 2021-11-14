using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDisplay : MonoBehaviour
{

    private Transform cameraTransform;
    private Transform towerPosition;
    private PlayerInputControl player;
    private Vector3 startPosition;
    public static TowerDisplay instance;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = Vector3.zero;
        player = PlayerInputControl.instance;
        gameObject.SetActive(true);
    }

    private void Awake()
    {

        if (instance == null)
        {
            instance = null;
        }
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit; 
            Vector2 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                TowerStats tempStats;
                if (hit.collider.gameObject.TryGetComponent<TowerStats>(out tempStats)) 
                {
                    gameObject.SetActive(true);
                    startPosition = player.transform.position;
                }
            }
        }

        if (startPosition != Vector3.zero && 
            Vector3.Distance(startPosition, player.transform.position) > 6)
        {
            gameObject.SetActive(false);
        }
        
    }

    public void enableDisplay()
    {
        
    }
}
