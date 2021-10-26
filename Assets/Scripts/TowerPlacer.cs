using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public GameObject shadowWall;
    public GameObject wall;
    private WallStorage wallStorage;
    public GameObject shadowTower;
    private Pathfinder pathFinder;

    public List<GameObject> gemRoster;
    // Start is called before the first frame update
    void Start()
    {

        pathFinder = GetComponent<Pathfinder>();
        wallStorage = GetComponent<WallStorage>();
        shadowWall = Instantiate(shadowWall, new Vector3(25,0,0), new Quaternion());
        shadowTower = Instantiate(shadowTower, new Vector3(25, 0, 0), new Quaternion());
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Vector3 curPoint;
        Vector3 prevPoint = Vector3.zero;
        if (Physics.Raycast(ray, out hit) &&
            !Input.GetMouseButton(1))
        {
            curPoint = hit.point;
            curPoint = UtilityFunctions.snapVector(curPoint);
            if (prevPoint != curPoint && !wallStorage.isWall(curPoint) && hit.collider.gameObject.tag != "Checkpoint")
            {
                if (validTowerPlacement(curPoint))
                {
                    shadowTower.transform.position = shadowWall.transform.rotation * Vector3.forward * 1 + shadowWall.transform.position;
                    shadowTower.transform.rotation = UtilityFunctions.getRotationawayFromSide(UtilityFunctions.getSide(curPoint));
                }
                else
                {
                    shadowTower.transform.position = new Vector3(25, 0, 0);
                }
                
                shadowWall.transform.position = curPoint;
                shadowWall.transform.rotation = UtilityFunctions.getRotationawayFromSide(UtilityFunctions.getSide(curPoint));
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject newWall = Instantiate(wall, curPoint, shadowWall.transform.rotation);
                    GameObject newTower = Instantiate(getRandomGem(), shadowTower.transform.position, shadowTower.transform.rotation);
                    wallStorage.attachTowerToWall(newTower, newWall);
                    shadowWall.transform.position = new Vector3(25, 0, 0);
                    wallStorage.addWall(curPoint, newWall);
                }
            }
            prevPoint = curPoint;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {            wallStorage.popRecentWall();
            pathFinder.findPath();
        }
    }

    private GameObject getRandomGem()
    {
        return gemRoster[0];
    }

    public static bool validTowerPlacement(Vector3 checkVec)
    {
        UtilityFunctions.Side side = UtilityFunctions.getSide(checkVec);
        if (side == UtilityFunctions.Side.front || side == UtilityFunctions.Side.back)
        {
            return Mathf.Abs(checkVec.x) < 9 && Mathf.Abs(checkVec.y) < 9;
        }
        else if (side == UtilityFunctions.Side.left || side == UtilityFunctions.Side.right)
        {
            return Mathf.Abs(checkVec.z) < 9 && Mathf.Abs(checkVec.y) < 9;
        }
        else if (side == UtilityFunctions.Side.top || side == UtilityFunctions.Side.bottom)
        {
            return Mathf.Abs(checkVec.x) < 9 && Mathf.Abs(checkVec.z) < 9;
        }
        else
        {
            Debug.Log("ERROR: SIDE DOES NOT EXIST");
            return false;
        }
    }
}
