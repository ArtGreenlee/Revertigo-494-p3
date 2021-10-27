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
        if (Physics.Raycast(ray, out hit) &&
            !Input.GetMouseButton(1))
        {
            curPoint = hit.point;
            curPoint = UtilityFunctions.snapVector(curPoint);
            if (wallStorage.validWallPosition(curPoint) && hit.collider.gameObject.tag != "Checkpoint")
            {
                shadowWall.transform.rotation = UtilityFunctions.getRotationawayFromSide(UtilityFunctions.getSide(curPoint));
                shadowWall.transform.position = shadowWall.transform.rotation * Vector3.forward * -.5f + curPoint;
                if (validTowerPlacement(curPoint))
                {
                    shadowTower.transform.position = shadowWall.transform.rotation * Vector3.forward * 1.5f + shadowWall.transform.position;
                    shadowTower.transform.rotation = UtilityFunctions.getRotationawayFromSide(UtilityFunctions.getSide(curPoint));
                }
                else
                {
                    shadowTower.transform.position = new Vector3(25, 0, 0);
                }
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject newWall = Instantiate(wall, shadowWall.transform.position, shadowWall.transform.rotation);
                    GameObject newTower = Instantiate(getRandomGem(), shadowTower.transform.position, shadowTower.transform.rotation);
                    wallStorage.attachTowerToWall(newTower, newWall);
                    shadowTower.transform.position = new Vector3(25, 0, 0);
                    shadowWall.transform.position = new Vector3(25, 0, 0);
                    wallStorage.addWall(curPoint, newWall);
                }
            }
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
            return Mathf.Abs(checkVec.x) < 8 && Mathf.Abs(checkVec.y) < 8;
        }
        else if (side == UtilityFunctions.Side.left || side == UtilityFunctions.Side.right)
        {
            return Mathf.Abs(checkVec.z) < 8 && Mathf.Abs(checkVec.y) < 8;
        }
        else if (side == UtilityFunctions.Side.top || side == UtilityFunctions.Side.bottom)
        {
            return Mathf.Abs(checkVec.x) < 8 && Mathf.Abs(checkVec.z) < 8;
        }
        else
        {
            Debug.Log("ERROR: SIDE DOES NOT EXIST");
            return false;
        }
    }
}
