using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public GameObject shadowWall;
    public GameObject wall;
    private WallStorage wallStorage;
    private EnemyStorage enemyStorage;
    public GameObject shadowTower;
    private Pathfinder pathFinder;
    private HashSet<Vector3> checkPointVectors;
    public GameObject onPlacementEffect;
    SnapToPosition CameraMovement;
    MeshRenderer shadowRenderer;
    MeshRenderer wallRenderer;
    public bool debugMode;
    public bool towerPlacementEnabled;

    public List<GameObject> debugRoster;

    public List<GameObject> gemRoster;
    // Start is called before the first frame update

    private void Awake()
    {
        pathFinder = GetComponent<Pathfinder>();
        wallStorage = GetComponent<WallStorage>();
        enemyStorage = GetComponent<EnemyStorage>();
    }

    void Start()
    {
        checkPointVectors = new HashSet<Vector3>();
        foreach (GameObject checkPoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            Vector3 curVec = UtilityFunctions.snapVector(checkPoint.transform.position);
            for (float i = -1; i < 1.5f; i += .5f)
            {
                for (float j = -1; j < 1.5f; j += .5f)
                {
                    for (float k = -1; k < 1.5f; k += .5f)
                    {
                        checkPointVectors.Add(new Vector3(curVec.x + i, curVec.y + j, curVec.z + k));
                    }
                }
            }
        }
        shadowWall = Instantiate(shadowWall, new Vector3(25,0,0), new Quaternion());
        shadowTower = Instantiate(shadowTower, new Vector3(25, 0, 0), new Quaternion());
        CameraMovement = Camera.main.GetComponent<SnapToPosition>();
        shadowRenderer = shadowTower.GetComponent<MeshRenderer>();
        wallRenderer = shadowWall.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        shadowRenderer.enabled = !CameraMovement.isLerping;
        wallRenderer.enabled = !CameraMovement.isLerping;
        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Vector3 curPoint;
        if (Physics.Raycast(ray, out hit))
        {
            curPoint = hit.point;
            curPoint = UtilityFunctions.snapVector(curPoint);
            if (wallStorage.validWallPosition(curPoint) && 
                !isCheckpoint(curPoint) && 
                enemyStorage.validWallPosition(curPoint) &&
                hit.collider.gameObject.CompareTag("Playfield"))
            {
                shadowWall.transform.rotation = UtilityFunctions.getRotationawayFromSide(UtilityFunctions.getClosestSide(curPoint));
                shadowWall.transform.position = shadowWall.transform.rotation * Vector3.forward * .5f + curPoint;
                
                if (validTowerPlacement(curPoint) && towerPlacementEnabled)
                {
                    shadowTower.transform.position = shadowWall.transform.rotation * Vector3.forward * -1.5f + shadowWall.transform.position;
                    shadowTower.transform.rotation = UtilityFunctions.getRotationawayFromSide(UtilityFunctions.getClosestSide(curPoint));
                }
                else
                {
                    shadowTower.transform.position = new Vector3(25, 0, 0);
                }
                if (Input.GetMouseButtonDown(1))
                {
                    GameObject newWall = Instantiate(wall, shadowWall.transform.position, shadowWall.transform.rotation);
                    GameObject newTower = Instantiate(getRandomTower(), shadowTower.transform.position, shadowTower.transform.rotation);
                    Instantiate(onPlacementEffect, shadowTower.transform.position, shadowTower.transform.rotation);
                    wallStorage.attachTowerToWall(newTower, newWall);
                    shadowTower.transform.position = new Vector3(25, 0, 0);
                    shadowWall.transform.position = new Vector3(25, 0, 0);
                    wallStorage.addWall(curPoint, newWall);
                }
            }
            else
            {
                shadowTower.transform.position = new Vector3(25, 0, 0);
                shadowWall.transform.position = new Vector3(25, 0, 0);
            }
        }

        /*if (Input.GetKeyDown(KeyCode.R))
        {            wallStorage.popRecentWall();
            pathFinder.findPath();
        }*/
    }

    private GameObject getRandomTower()
    {
        if (debugMode)
        {
            return debugRoster[Random.Range(0, debugRoster.Count)];
        }
        return gemRoster[Random.Range(0, gemRoster.Count)];
    }

    private bool isCheckpoint(Vector3 checkVec)
    {
        return checkPointVectors.Contains(checkVec);
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
}
