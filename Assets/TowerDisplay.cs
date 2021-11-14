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
    public TextMeshProUGUI killsText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI killsToUpgradeText;
    Vector3 startScale;
    private bool active;
    // Start is called before the first frame update
    void Start()
    {
        active = false;
        startScale = transform.localScale;
        transform.localScale = Vector3.zero;
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
                    //towerNameText.color = tempStats.trailRendererColor;
                    startPosition = player.transform.position;
                    damageMinText.text = "Damage min " + tempStats.damageMin.ToString();
                    damageMaxText.text = "Damage max " + tempStats.damageMax.ToString();
                    rateOfFireText.text = "Cooldown " + tempStats.getCooldown().ToString();
                    killsText.text = "Kills " + tempStats.kills;
                    if (!tempStats.specialTower)
                    {
                        levelText.text = "Level " + tempStats.level;
                    }
                    else
                    {
                        levelText.text = "";
                    }
                    

                    descriptionText.text = getDescription(tempStats);
                }
                else if (active)
                {
                    active = false;
                    StopAllCoroutines();
                    StartCoroutine(disableDisplay());
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

    private string getDescription(TowerStats towerStats)
    {
        string temp = "";
        if (towerStats.towerName == TowerStats.TowerName.Blue)
        {
            temp = "Slows enemy by " + towerStats.slowPercent.ToString() + " for " + towerStats.slowDuration.ToString() + " seconds";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Red)
        {
            temp = "Deals AoE damage within " + towerStats.aoe_range.ToString() + " range";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Yellow)
        {
            temp = "Attacks " + towerStats.numTargets.ToString() + " targets";
        }
        else if (towerStats.towerName == TowerStats.TowerName.White)
        {
            temp = "Has a " + towerStats.critChance.ToString() + " chance to deal " + towerStats.critMult.ToString() + " times damage";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Purple)
        {
            temp = "Buffs towers within range reducing cooldown by " + TowerStats.cooldownBuffDecreasePerLevel[towerStats.level].ToString();
        }
        else if (towerStats.towerName == TowerStats.TowerName.Green)
        {
            temp = "Poisons towers hit dealing " + towerStats.poisonDPS.ToString() + " DPS for " + towerStats.poisonDuration.ToString() + " seconds";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Fireball)
        {
            temp = "Shoots a fireball at the nearest enemy but can miss if placed incorrectly (try placing it on a different face then the enemy its targeting";
        }
        else if (towerStats.towerName == TowerStats.TowerName.ClusterFireball)
        {
            temp = "Shoots a fireball cluster";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Spirit)
        {
            temp = "Emenates spirits that search for enemies, slowing them drastically";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Tourmaline)
        {
            temp = "Attacks all towers in range and has a " + towerStats.critChance.ToString() + " chance to slow them for " + towerStats.slowPercent.ToString() + " for " + towerStats.slowDuration.ToString() + " seconds";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Stun)
        {
            temp = "Has a " + towerStats.critChance.ToString() + " chance to stun for " + towerStats.slowDuration.ToString() + " second(s)";
        }
        return temp;
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
        else if (towerStats.towerName == TowerStats.TowerName.White)
        {
            temp = "White ";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Purple)
        {
            temp = "Purple ";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Green)
        {
            temp = "Green ";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Spirit)
        {
            temp = "Spirit Tower";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Laser)
        {
            temp = "Laser Tower";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Fireball)
        {
            temp = "Fireball Tower";
        }
        else if (towerStats.towerName == TowerStats.TowerName.ClusterFireball)
        {
            temp = "Cluster Fireball Tower";
        }
        else if (towerStats.towerName == TowerStats.TowerName.Stun)
        {
            temp = "Stun Tower";
        }

        if (!towerStats.specialTower)
        {
            if (towerStats.level == 0)
            {
                temp += "Chip";
            }
            else if (towerStats.level == 1)
            {
                temp += "Shard";
            }
            else if (towerStats.level == 2)
            {
                temp += "Gem";
            }
            else if (towerStats.level == 3)
            {
                temp += "Big Gem";
            }
            else if (towerStats.level == 4)
            {
                temp += "Perfect Gem";
            }

        }
        return temp;
    }
}
