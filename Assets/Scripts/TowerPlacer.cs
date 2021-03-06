using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{

    private WallStorage wallStorage;
    public GameObject shadowTower;
    public AudioClip placeTowerSFX;
    public AudioClip returnTowerSFX;
    private AudioSource source;
    public float placeTowerVol = 0.1f;
    private TowerInventory towerInventory;
    private TowerStorage towerStorage;

    public int priceIncreasePerPlacement;

    private int layerMask;

    public static TowerPlacer instance;

    int podiumLayerMask;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        source = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        layerMask = ~LayerMask.GetMask("Player", "Tower");
        towerStorage = TowerStorage.instance;
        wallStorage = WallStorage.instance;
        towerInventory = TowerInventory.instance;
        shadowTower = Instantiate(shadowTower, transform.position, Quaternion.identity);
        shadowTower.transform.position = new Vector3(100, 0, 0);
        podiumLayerMask = ~LayerMask.GetMask("Podium");
    }

    

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            if (hit.collider.gameObject.CompareTag("Podium"))
            {
                GameObject podium = hit.collider.gameObject;
                if (towerInventory.playerInventory.Count > 0)
                {
                    if (!wallStorage.podiumHasTower(podium))
                    {
                        shadowTower.transform.position = podium.transform.rotation * Vector3.forward * 1.5f + podium.transform.position;
                        shadowTower.transform.rotation = UtilityFunctions.getRotationTowardSide(podium.transform.position);
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            //towerInventory.selectionEnabled = false;
                            GameObject tempTower = towerInventory.playerInventory[0];
                            wallStorage.attachTowerToPodium(tempTower, podium);
                            source.PlayOneShot(placeTowerSFX, placeTowerVol);
                            StartCoroutine(placeTowerOnPodium(tempTower, tempTower.transform.position, shadowTower.transform.position, podium));
                            towerInventory.playerInventory.RemoveAt(0);
                            shadowTower.transform.position = new Vector3(100, 0, 0);
                            towerInventory.basePrice += 2;
                            if (towerInventory.playerInventory.Count > 0)
                            {
                                towerInventory.combinations = towerInventory.checkCurrentTowerForCombinations();
                            }
                            
                            towerInventory.price = towerInventory.basePrice;
                            towerInventory.priceText.text = "Tower Cost " + towerInventory.price.ToString();
                            //StartCoroutine(towerInventory.destroyPlayerInventory());
                            towerInventory.combinationLrIndex = 0;
                            if (towerInventory.combinations.Count > 0)
                            {
                                towerInventory.lr.positionCount = towerInventory.combinations[towerInventory.combinationLrIndex].Count;
                            }
                            else
                            {
                                towerInventory.lr.positionCount = 0;
                            }
                        }
                    }
                    else if (wallStorage.getTowerAttachedToPodium(podium) != null &&
                        towerInventory.playerInventory.Count > 0 &&
                        TowerInventory.canCombine(towerInventory.playerInventory[0].GetComponent<TowerStats>(),
                        wallStorage.getTowerAttachedToPodium(podium).GetComponent<TowerStats>()))
                    {
                        shadowTower.transform.position = podium.transform.rotation * Vector3.forward * 1.5f + podium.transform.position;
                        shadowTower.transform.rotation = UtilityFunctions.getRotationTowardSide(podium.transform.position);
                        if (Input.GetKeyDown(KeyCode.F))
                        {
                            towerInventory.price--;
                            towerInventory.priceText.text = "Tower Cost " + towerInventory.price.ToString();
                            source.PlayOneShot(placeTowerSFX, placeTowerVol);
                            StartCoroutine(towerInventory.combineTowerOnPodium(wallStorage.getTowerAttachedToPodium(podium), towerInventory.playerInventory[0]));
                            shadowTower.transform.position = new Vector3(100, 0, 0);
                        }

                    }
                }
                else
                {
                    shadowTower.transform.position = new Vector3(100, 0, 0);
                }
            }
            else
            {
                shadowTower.transform.position = new Vector3(100, 0, 0);
            }
        }
        else
        {
            shadowTower.transform.position = new Vector3(100, 0, 0);
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
        MeshRenderer mesh = attachPodium.GetComponent<MeshRenderer>();
        Color finalColor = tower.GetComponent<TowerStats>().trailRendererColor;
        //StartCoroutine(UtilityFunctions.changeScaleOfTransformOverTime(tower.transform, 1, 1));
        while (Vector3.Distance(tower.transform.position, end) > .05f)
        {
            tower.transform.position = Vector3.Slerp(tower.transform.position, end, 3 * Time.deltaTime);
            mesh.material.color = Color.Lerp(mesh.material.color, finalColor, 3 * Time.deltaTime);
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
            mesh.material.color = finalColor;
            towerStorage.addTower(end, tower);
            tower.transform.position = end;
            tower.GetComponent<TowerStats>().attachedToPlayer = false;
            ShootsBullets recoilSnapPositionTemp;
            if (tower.TryGetComponent<ShootsBullets>(out recoilSnapPositionTemp))
            {
                recoilSnapPositionTemp.snapPosition = end;
            }
        }
    }

    
}
