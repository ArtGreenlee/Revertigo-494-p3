using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPlacer : MonoBehaviour
{
    public GameObject shadowWall;
    public GameObject wall;
    public AudioClip wallPlaceSFX;
    public float wallPlaceVol = 0.1f;
    private AudioSource source;
    private WallStorage wallStorage;
    private EnemyStorage enemyStorage;
    
    private HashSet<Vector3> checkPointVectors;
    public GameObject onPlacementEffect;
    MeshRenderer shadowRenderer;
    MeshRenderer wallRenderer;
    private GoldStorage goldStorage;
    Vector3 dragDirection;
    GameObject firstWall;
    Vector3 previousPlacementPosition;

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
        source = GetComponent<AudioSource>();
    }

    void Start()
    {
        dragDirection = Vector3.zero;
        previousPlacementPosition = Vector3.zero;
        layerMask = ~LayerMask.GetMask("Player");
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

        if (Input.GetMouseButtonUp(1))
        {
            firstWall = null;
            dragDirection = Vector3.zero;
            previousPlacementPosition = Vector3.zero;
        }

        Vector3 curPoint;
        if (Physics.Raycast(ray, out hit, 100, layerMask) && 
            goldStorage.gold >= 1)
        {

            curPoint = hit.point;
            curPoint = UtilityFunctions.snapVector(curPoint);
            if (dragDirection != Vector3.zero)
            {
                if (Vector3.Distance(curPoint, firstWall.transform.position) > Vector3.Distance(previousPlacementPosition, firstWall.transform.position)) {
                    curPoint = previousPlacementPosition + dragDirection;
                }
                //we need to know if curpoint is greater previous placements position in the direction of the wall
                
            }
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
                    source.PlayOneShot(wallPlaceSFX, wallPlaceVol);
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
                    if (Input.GetMouseButtonDown(1))
                    {
                        firstWall = newWall;
                    }
                    else if (dragDirection == Vector3.zero && firstWall != null)
                    {
                        dragDirection = UtilityFunctions.getClosestSide(newWall.transform.position - firstWall.transform.position);
                    }
                    previousPlacementPosition = curPoint;
                    wallStorage.addWall(curPoint, newWall);
                    source.PlayOneShot(wallPlaceSFX, wallPlaceVol);
                    shadowWall.transform.position = storageVector;
                }
                else if ((!Input.GetMouseButton(1) || !wallNearby(curPoint)) &&
                    (dragDirection == Vector3.zero || UtilityFunctions.getClosestSide(curPoint - previousPlacementPosition) == dragDirection))
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
