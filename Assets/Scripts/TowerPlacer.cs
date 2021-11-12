using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{

    private WallStorage wallStorage;
    public GameObject shadowTower;
    private TowerInventory towerInventory;
    private TowerStorage towerStorage;
    // Start is called before the first frame update
    void Start()
    {
        towerStorage = TowerStorage.instance;
        wallStorage = WallStorage.instance;
        towerInventory = TowerInventory.instance;
        shadowTower = Instantiate(shadowTower, transform.position, Quaternion.identity);
        shadowTower.transform.position = new Vector3(25, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Vector3 curPoint;
        if (Physics.Raycast(ray, out hit))
        {
            curPoint = UtilityFunctions.snapVector(hit.point);
            if (wallStorage.isWall(curPoint))
            {
                GameObject tempWall = wallStorage.getWall(curPoint);
                if (!wallStorage.wallHasTower(tempWall) && validTowerPlacement(curPoint) && towerInventory.playerInventory.Count > 0)
                {
                    shadowTower.transform.position = tempWall.transform.rotation * Vector3.forward * 1.5f + tempWall.transform.position;
                    shadowTower.transform.rotation = UtilityFunctions.getRotationTowardSide(curPoint);
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        GameObject tempTower = towerInventory.playerInventory[0];
                        wallStorage.attachTowerToWall(tempTower, tempWall);
                        
                        StartCoroutine(placeTowerOnBoard(tempTower, tempTower.transform.position, shadowTower.transform.position, tempWall));
                        towerInventory.playerInventory.RemoveAt(0);
                        shadowTower.transform.position = new Vector3(25, 0, 0);
                    }

                }
                else if (wallStorage.wallHasTower(tempWall) && Input.GetKeyDown(KeyCode.T))
                {

                    wallStorage.detatchTowerAndReturn(tempWall);
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

    private IEnumerator placeTowerOnBoard(GameObject tower, Vector3 start, Vector3 end, GameObject attachWall)
    {


        //StartCoroutine(UtilityFunctions.changeScaleOfTransformOverTime(tower.transform, 1, 1));
        while (Vector3.Distance(tower.transform.position, end) > .05f )
        {
            tower.transform.position = Vector3.Lerp(tower.transform.position, end, 3 * Time.deltaTime);
            if (attachWall == null || !wallStorage.wallHasTower(attachWall))
            {
                //UtilityFunctions.changeScaleOfTransform(tower.transform, .5f);
                //StopAllCoroutines();
                
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
        if (attachWall == null)
        {
            //UtilityFunctions.changeScaleOfTransform(tower.transform, .5f);
            StopAllCoroutines();
            
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
}
