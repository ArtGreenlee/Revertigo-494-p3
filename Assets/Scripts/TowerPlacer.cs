using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{

    private WallStorage wallStorage;
    public GameObject shadowTower;
    public AudioClip placeTowerSFX;
    public AudioClip returnTowerSFX;
    private TowerInventory towerInventory;
    private TowerStorage towerStorage;
    public GameObject combineEffect;

    public static TowerPlacer instance;

    int podiumLayerMask;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        towerStorage = TowerStorage.instance;
        wallStorage = WallStorage.instance;
        towerInventory = TowerInventory.instance;
        shadowTower = Instantiate(shadowTower, transform.position, Quaternion.identity);
        shadowTower.transform.position = new Vector3(25, 0, 0);
        podiumLayerMask = ~LayerMask.GetMask("Podium");
    }

    private bool canCombine(GameObject towerA, GameObject towerB)
    {
        TowerStats towerStatsA = towerA.GetComponent<TowerStats>();
        TowerStats towerStatsB = towerB.GetComponent<TowerStats>();
        if (!towerStatsA.specialTower && 
            !towerStatsB.specialTower && 
            towerStatsA.level == towerStatsB.level &&
            towerStatsA.towerName == towerStatsB.towerName &&
            towerStatsA.level != TowerStats.damageIncreaseAtLevel.Count - 1)
        {
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.CompareTag("Podium"))
            {
                GameObject podium = hit.collider.gameObject;
                if (towerInventory.selectionEnabled)
                {
                    if (!wallStorage.podiumHasTower(podium))
                    {
                        shadowTower.transform.position = podium.transform.rotation * Vector3.forward * 1.5f + podium.transform.position;
                        shadowTower.transform.rotation = UtilityFunctions.getRotationTowardSide(podium.transform.position);
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            towerInventory.selectionEnabled = false;
                            GameObject tempTower = towerInventory.playerInventory[0];
                            wallStorage.attachTowerToPodium(tempTower, podium);
                            AudioSource.PlayClipAtPoint(placeTowerSFX, Camera.main.transform.position);

                            StartCoroutine(placeTowerOnPodium(tempTower, tempTower.transform.position, shadowTower.transform.position, podium));
                            towerInventory.playerInventory.RemoveAt(0);
                            shadowTower.transform.position = new Vector3(25, 0, 0);
                        }
                    }
                    else if (wallStorage.podiumHasTower(podium) && Input.GetKeyDown(KeyCode.F))
                    {
                        GameObject tower = wallStorage.getTowerAttachedToPodium(podium);
                        if (canCombine(tower, towerInventory.playerInventory[0]))
                        {
                            towerInventory.selectionEnabled = false;
                            StartCoroutine(combineTowers(tower, towerInventory.playerInventory[0]));
                            towerInventory.playerInventory.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    shadowTower.transform.position = new Vector3(25, 0, 0);
                }
            }
            else
            {
                shadowTower.transform.position = new Vector3(25, 0, 0);
            }
        }
        else
        {
            shadowTower.transform.position = new Vector3(25, 0, 0);
        }
    }

    public static bool validTowerPlacement(Vector3 checkVec)
    {
        /*Vector3 side = UtilityFunctions.getClosestSide(checkVec);
        if (side == Vector3.forward || side == Vector3.back)
        {
            return Mathf.Abs(checkVec.x) < 14 && Mathf.Abs(checkVec.y) < 14;
        }
        else if (side == Vector3.left || side == Vector3.right)
        {
            return Mathf.Abs(checkVec.z) < 14 && Mathf.Abs(checkVec.y) < 14;
        }
        else if (side == Vector3.up || side == Vector3.down)
        {
            return Mathf.Abs(checkVec.x) < 14 && Mathf.Abs(checkVec.z) < 14;
        }
        else
        {
            Debug.Log("ERROR: SIDE DOES NOT EXIST");
            return false;
        }*/
        return true;
    }

    private IEnumerator placeTowerOnPodium(GameObject tower, Vector3 start, Vector3 end, GameObject attachPodium)
    {


        //StartCoroutine(UtilityFunctions.changeScaleOfTransformOverTime(tower.transform, 1, 1));
        while (Vector3.Distance(tower.transform.position, end) > .05f)
        {
            tower.transform.position = Vector3.Slerp(tower.transform.position, end, 3 * Time.deltaTime);
            if (attachPodium == null)
            {
                //UtilityFunctions.changeScaleOfTransform(tower.transform, .5f);
                //StopAllCoroutines();
                
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
        if (attachPodium == null)
        {
            //UtilityFunctions.changeScaleOfTransform(tower.transform, .5f);
            //StopAllCoroutines();
            
            yield break;
        }
        else
        {
            towerStorage.addTower(tower);
            tower.transform.position = end;
            tower.GetComponent<TowerStats>().attachedToPlayer = false;
            ShootsBullets recoilSnapPositionTemp;
            if (tower.TryGetComponent<ShootsBullets>(out recoilSnapPositionTemp))
            {
                recoilSnapPositionTemp.snapPosition = end;
            }
        }
    }

    private IEnumerator combineTowers(GameObject podiumTower, GameObject towerB)
    {
        while (Vector3.Distance(podiumTower.transform.position, towerB.transform.position) > .05f)
        {
            towerB.transform.position = Vector3.Slerp(towerB.transform.position, podiumTower.transform.position, 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        TowerStats podiumTowerStats = podiumTower.GetComponent<TowerStats>();
        TowerStats towerBStats = towerB.GetComponent<TowerStats>();
        podiumTowerStats.kills += towerBStats.kills;
        podiumTowerStats.level++;
        Destroy(towerB);
    }
}
