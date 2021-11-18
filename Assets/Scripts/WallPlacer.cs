using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacer : MonoBehaviour
{
    public GameObject shadowWall;
    public GameObject wall;
    public AudioClip wallPlaceSFX;
    private WallStorage wallStorage;
    private EnemyStorage enemyStorage;
    
    private HashSet<Vector3> checkPointVectors;
    public GameObject onPlacementEffect;
    MeshRenderer shadowRenderer;
    MeshRenderer wallRenderer;
    private GoldStorage goldStorage;

    public static WallPlacer instance;
    private Vector3 storageVector;

    private HashSet<Vector3> currentMouseDragWallPositions;

    private int layerMask;

    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        layerMask = ~LayerMask.GetMask("Tower", "Player");
        storageVector = new Vector3(25, 0, 0);
        currentMouseDragWallPositions = new HashSet<Vector3>();
        goldStorage = GoldStorage.instance;
        wallStorage = WallStorage.instance;
        enemyStorage = EnemyStorage.instance;
        checkPointVectors = new HashSet<Vector3>();
        foreach (GameObject checkPoint in GameObject.FindGameObjectsWithTag("Checkpoint"))
        {
            Vector3 curVec = UtilityFunctions.snapVector(checkPoint.transform.position);
            for (float i = -.5f; i < 1f; i += .5f)
            {
                for (float j = -.5f; j < 1f; j += .5f)
                {
                    for (float k = -.5f; k < 1f; k += .5f)
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
        mousePosition.x += .25f;
        mousePosition.y += .25f;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

       

        Vector3 curPoint;
        if (Physics.Raycast(ray, out hit, 100, layerMask) && 
            goldStorage.gold >= 1)
        {

            curPoint = hit.point;
            curPoint = UtilityFunctions.snapVector(curPoint);
            if (wallStorage.validWallPosition(curPoint) && 
                !isCheckpoint(curPoint) && 
                enemyStorage.validWallPosition(curPoint) &&
                hit.collider.gameObject.CompareTag("Playfield"))
            {
                /*shadowWall.transform.rotation = UtilityFunctions.getRotationawayFromSide(curPoint);
                //shadowWall.transform.position = shadowWall.transform.rotation * Vector3.forward * .5f + curPoint;
                shadowWall.transform.position = shadowWall.transform.rotation * Vector3.back * .75f + curPoint;
                Vector3 adjustment = shadowWall.transform.rotation.eulerAngles;
                adjustment.z += 22.5f;
                shadowWall.transform.rotation = Quaternion.Euler(adjustment);

                if (Input.GetMouseButton(1))
                {
                    goldStorage.changeGoldAmount(-1);
                    GameObject newWall = Instantiate(wall, shadowWall.transform.position, shadowWall.transform.rotation);
                    shadowWall.transform.position = storageVector;
                    wallStorage.addWall(curPoint, newWall);
                    AudioSource.PlayClipAtPoint(wallPlaceSFX, Camera.main.transform.position);
                }*/

                if (Input.GetMouseButton(1) && shadowWall.transform.position != storageVector && !wallStorage.placements.ContainsKey(shadowWall.transform.position))
                {
                    for (float i = -.5f; i < 1f; i += .5f)
                    {
                        for (float j = -.5f; j < 1f; j += .5f)
                        {
                            for (float k = -.5f; k < 1f; k += .5f)
                            {
                                Vector3 curVec = new Vector3(curPoint.x + i, curPoint.y + j, curPoint.z + k);
                                if (!currentMouseDragWallPositions.Contains(curVec) && UtilityFunctions.validEnemyVector(curVec))
                                {
                                    currentMouseDragWallPositions.Add(curVec);
                                }

                            }
                        }
                    }
                    goldStorage.changeGoldAmount(-1);
                    GameObject newWall = Instantiate(wall, shadowWall.transform.position, shadowWall.transform.rotation);
                    wallStorage.addWall(curPoint, newWall);
                    AudioSource.PlayClipAtPoint(wallPlaceSFX, Camera.main.transform.position);
                    shadowWall.transform.position = storageVector;
                }
                else if (!Input.GetMouseButton(1) || !wallNearby(curPoint))
                {
                    shadowWall.transform.rotation = UtilityFunctions.getRotationawayFromSide(curPoint);
                    //shadowWall.transform.position = shadowWall.transform.rotation * Vector3.forward * .5f + curPoint;
                    shadowWall.transform.position = shadowWall.transform.rotation * Vector3.back * .5f + curPoint;
                    Vector3 adjustment = shadowWall.transform.rotation.eulerAngles;
                    adjustment.z += 22.5f;
                    shadowWall.transform.rotation = Quaternion.Euler(adjustment);
                }
                else
                {
                    shadowWall.transform.position = storageVector;
                }
                

            }
            else
            {
                //shadowTower.transform.position = new Vector3(25, 0, 0);
                shadowWall.transform.position = storageVector;
            }
        }
        else
        {
            shadowWall.transform.position = storageVector;
        }

        if (Input.GetMouseButtonUp(1))
        {
            currentMouseDragWallPositions.Clear();
        }


        /*if (Input.GetKeyDown(KeyCode.R))
        {            wallStorage.popRecentWall();
            pathFinder.findPath();
        }*/
    }

    private bool wallNearby(Vector3 checkVec)
    {
        for (float x = -1; x < 1.5f; x += .5f)
        {
            for (float y = -1; y < 1.5f; y += .5f)
            {
                for (float z = -1; z < 1.5f; z += .5f)
                {
                    Vector3 tempVec = new Vector3(checkVec.x + x, checkVec.y + y, checkVec.z + z);
                    if (wallStorage.isWall(tempVec) && !currentMouseDragWallPositions.Contains(tempVec))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    

    private bool isCheckpoint(Vector3 checkVec)
    {
        return checkPointVectors.Contains(checkVec);
    }

    
}
