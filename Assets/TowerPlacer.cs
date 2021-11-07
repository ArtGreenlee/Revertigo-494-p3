using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{

    private WallStorage wallStorage;
    public GameObject shadowTower;
    private TowerInventory towerInventory; 
    // Start is called before the first frame update
    void Start()
    {
        wallStorage = WallStorage.instance;
        towerInventory = TowerInventory.instance;
        shadowTower = Instantiate(shadowTower, transform.position, new Quaternion());
        shadowTower.transform.position = new Vector3(25, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Vector3 curPoint;
        if (Physics.Raycast(ray, out hit) && towerInventory.playerInventory.Count > 0)
        {
            curPoint = UtilityFunctions.snapVector(hit.point);
            if (wallStorage.isWall(curPoint) && !wallStorage.wallHasTower(wallStorage.getWall(curPoint)) && validTowerPlacement(curPoint))
            {
                GameObject tempWall = wallStorage.getWall(curPoint);
                shadowTower.transform.position = tempWall.transform.rotation * Vector3.forward * -1.5f + tempWall.transform.position;
                shadowTower.transform.rotation = UtilityFunctions.getRotationawayFromSide(UtilityFunctions.getClosestSide(curPoint));
                if (Input.GetMouseButtonDown(2))
                {
                    StartCoroutine(placeTowerOnBoard(towerInventory.playerInventory[0], towerInventory.playerInventory[0].transform.position, shadowTower.transform.position, tempWall));
                    towerInventory.playerInventory.RemoveAt(0);
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
        Vector3 side = UtilityFunctions.getClosestSide(checkVec);
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
        }
    }

    private IEnumerator placeTowerOnBoard(GameObject tower, Vector3 start, Vector3 end, GameObject attachWall)
    {
        while (Vector3.Distance(tower.transform.position, end) > .05f )
        {
            tower.transform.position = Vector3.Lerp(tower.transform.position, end, 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        if (attachWall == null)
        {
            towerInventory.playerInventory.Add(tower);
            yield break;
        }
        else
        {
            wallStorage.attachTowerToWall(tower, attachWall);
            tower.transform.position = end;
            tower.GetComponent<TowerStats>().automaticallyShoots = true;
        }
    }
}
