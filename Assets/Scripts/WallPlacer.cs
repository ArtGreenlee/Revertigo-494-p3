using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacer : MonoBehaviour
{
    public GameObject shadowWall;
    public GameObject wall;
    private WallStorage wallStorage;
    private EnemyStorage enemyStorage;
    
    private HashSet<Vector3> checkPointVectors;
    public GameObject onPlacementEffect;
    MeshRenderer shadowRenderer;
    MeshRenderer wallRenderer;
    private GoldStorage goldStorage;
    
    // Start is called before the first frame update

    private void Awake()
    {
        
    }

    void Start()
    {
        goldStorage = GoldStorage.instance;
        wallStorage = WallStorage.instance;
        enemyStorage = EnemyStorage.instance;
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
        shadowWall = Instantiate(shadowWall, new Vector3(25,0,0), Quaternion.identity);
        //shadowTower = Instantiate(shadowTower, new Vector3(25, 0, 0), Quaternion.identity);
        //shadowRenderer = shadowTower.GetComponent<MeshRenderer>();
        wallRenderer = shadowWall.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Vector3 curPoint;
        if (Physics.Raycast(ray, out hit) && goldStorage.gold >= 1)
        {
            curPoint = hit.point;
            curPoint = UtilityFunctions.snapVector(curPoint);
            if (wallStorage.validWallPosition(curPoint) && 
                !isCheckpoint(curPoint) && 
                enemyStorage.validWallPosition(curPoint) &&
                hit.collider.gameObject.CompareTag("Playfield"))
            {
                shadowWall.transform.rotation = UtilityFunctions.getRotationTowardSide(curPoint);
                shadowWall.transform.position = shadowWall.transform.rotation * Vector3.forward * .5f + curPoint;
                
                /*if (validTowerPlacement(curPoint) && towerPlacementEnabled)
                {
                    //shadowTower.transform.position = shadowWall.transform.rotation * Vector3.forward * -1.5f + shadowWall.transform.position;
                    //shadowTower.transform.rotation = UtilityFunctions.getRotationawayFromSide(UtilityFunctions.getClosestSide(curPoint));
                }
                else
                {
                    //shadowTower.transform.position = new Vector3(25, 0, 0);
                }*/
                if (Input.GetMouseButtonDown(1))
                {
                    goldStorage.changeGoldAmount(-1);
                    GameObject newWall = Instantiate(wall, shadowWall.transform.position, shadowWall.transform.rotation);
                    //GameObject newTower = Instantiate(getRandomTower(), shadowTower.transform.position, shadowTower.transform.rotation);
                    //Instantiate(onPlacementEffect, shadowTower.transform.position, shadowTower.transform.rotation);
                    //wallStorage.attachTowerToWall(newTower, newWall);
                    //shadowTower.transform.position = new Vector3(25, 0, 0);
                    shadowWall.transform.position = new Vector3(25, 0, 0);
                    wallStorage.addWall(curPoint, newWall);
                }
            }
            else
            {
                //shadowTower.transform.position = new Vector3(25, 0, 0);
                shadowWall.transform.position = new Vector3(25, 0, 0);
            }
        }

        /*if (Input.GetKeyDown(KeyCode.R))
        {            wallStorage.popRecentWall();
            pathFinder.findPath();
        }*/
    }

    

    private bool isCheckpoint(Vector3 checkVec)
    {
        return checkPointVectors.Contains(checkVec);
    }

    
}
