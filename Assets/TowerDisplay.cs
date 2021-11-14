using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TowerDisplay : MonoBehaviour
{

    private Transform cameraTransform;
    private Transform towerPosition;
    private PlayerInputControl player;
    private Vector3 startPosition;
    public static TowerDisplay instance;
    //private RectTransform transform;

    public TextMeshProUGUI damageMinText;
    public TextMeshProUGUI damageMaxText;
    public TextMeshProUGUI towerNameText;
    public TextMeshProUGUI rateOfFireText;
    public TextMeshProUGUI descriptionText;
    Vector3 startScale;
    private bool active;
    // Start is called before the first frame update
    void Start()
    {
        active = false;
        startScale = transform.localScale;
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
                if (hit.collider.gameObject.TryGetComponent<TowerStats>(out tempStats) && !active) 
                {
                    active = true;
                    StopAllCoroutines();
                    StartCoroutine(enableDisplay());
                    towerNameText.text = getTowerName(tempStats);
                    towerNameText.color = tempStats.trailRendererColor;
                    startPosition = player.transform.position;
                }
            }
        }

        if (startPosition != Vector3.zero && 
            Vector3.Distance(startPosition, player.transform.position) > 4 && active)
        {
            active = false;
            StopAllCoroutines();
            StartCoroutine(disableDisplay());
        }
        
    }

    public IEnumerator disableDisplay()
    {
        
        Debug.Log("shrink display");
        transform.localScale = startScale;
        while (transform.localScale.x > 0.0001f)
        {
            transform.localScale = Vector3.Slerp(transform.localScale, Vector3.zero, Time.deltaTime * 10);
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator enableDisplay()
    {
        
        Debug.Log("enlarge display");
        transform.localScale = Vector3.zero;
        while (transform.localScale.x < startScale.x)
        {
            transform.localScale = Vector3.Slerp(transform.localScale, startScale, Time.deltaTime * 10);
            yield return new WaitForEndOfFrame();
        }
    }

    private string getTowerName(TowerStats towerStats)
    {
        string temp = "";
        if (towerStats.towerName == TowerStats.TowerName.Blue)
        {
            temp = "Blue ";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Red)
        {
            temp = "Red ";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Yellow)
        {
            temp = "Yellow ";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Blue)
        {
            temp = "Blue ";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Spirit)
        {
            temp = "Spirit Tower";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Laser)
        {
            temp = "Laser";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Fireball)
        {
            temp = "Fireball";
        }
        else if (towerStats.towerName == TowerStats.TowerName.ClusterFireball)
        {
            temp = "ClusterFireball";
        }

        if (!towerStats.specialTower)
        {
            if (towerStats.level == 0)
            {
                temp += "Chipped";
            }
        }
        return temp;
    }
}
