using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallStorage : MonoBehaviour
{
    private Dictionary<Vector3, GameObject> storage = new Dictionary<Vector3, GameObject>();
    public Dictionary<GameObject, GameObject> wallAndTowers = new Dictionary<GameObject, GameObject>();
    Dictionary<Vector3, int> duplicates = new Dictionary<Vector3, int>();
    public List<Pathfinder> pathfinders;
    private Stack<GameObject> wallStack;
    private TowerStorage towerStorage;
    private Dictionary<GameObject, Vector3> placementLocations;

    private TowerInventory towerInventory;

    public static WallStorage instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            //removeMostRecentWall();
        }
    }

    public void removeMostRecentWall()
    {
        if (wallStack.Count > 0)
        {
            GameObject tempWall = wallStack.Pop();
            placementLocations.Remove(tempWall);
            removeWall(tempWall);

            foreach (Pathfinder pathFinder in pathfinders)
            {
                if (pathFinder != null)
                {
                    pathFinder.findPath();
                }
            }
        }
    }

    void Start()
    {
        placementLocations = new Dictionary<GameObject, Vector3>();
        towerStorage = TowerStorage.instance;
        towerInventory = TowerInventory.instance;
        wallStack = new Stack<GameObject>();
    }

    public bool isWall(Vector3 checkVec)
    {
        return storage.ContainsKey(checkVec);
    }

    public GameObject getWall(Vector3 wallVec)
    {
        return storage[wallVec];
    }

    public void attachTowerToWall(GameObject towerIn, GameObject wallIn)
    {
        wallAndTowers.Add(wallIn, towerIn);
    }

    public void detectPathCollision()
    {
        HashSet<Pathfinder> redoBuffer = new HashSet<Pathfinder>();
        foreach (Vector3 checkVec in storage.Keys)
        {
            foreach (Pathfinder pathFinder in pathfinders)
            {
                if (!redoBuffer.Contains(pathFinder) && pathFinder.pathContainsVector(checkVec) && pathFinder != null)
                {
                    redoBuffer.Add(pathFinder);
                }
            }
        }
        foreach (Pathfinder pathFinder in redoBuffer)
        {
            if (pathFinder.enemyMovement == null)
            {
                pathFinder.detectAndRedoPathSegments();
            }
        }
    }

    private void checkAndCreateTowerPodiums(Vector3 checkVec)
    {

    }

    public bool validWallPosition(Vector3 checkVec)
    {
        if (isWall(checkVec))
        {
            return false;
        }
        //check for checkpoints
        Vector3 side = UtilityFunctions.getClosestSide(checkVec);
        for (float i = -1; i < 1.5f; i += .5f)
        {
            for (float j = -1; j < 1.5f; j += .5f)
            {
                for (float k = -1; k < 1.5f; k += .5f)
                {
                    Vector3 curVec = new Vector3(checkVec.x + i, checkVec.y + j, checkVec.z + k);
                    if (side == Vector3.forward || side == Vector3.back)
                    {
                        if (Mathf.Abs(curVec.y) > 17 || Mathf.Abs(curVec.x) > 17)
                        {
                            return false;
                        }
                    } 
                    else if (side == Vector3.left || side == Vector3.right)
                    {
                        if (Mathf.Abs(curVec.z) > 17 || Mathf.Abs(curVec.y) > 17)
                        {
                            return false;
                        }
                    }
                    else if (side == Vector3.up || side == Vector3.down)
                    {
                        if (Mathf.Abs(curVec.x) > 17 || Mathf.Abs(curVec.z) > 17)
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

    //add vec is the center of the wall
    public void addWall(Vector3 addVec, GameObject wallIn)
    {
        placementLocations.Add(wallIn, addVec);
        wallStack.Push(wallIn);
        Vector3 addSide = UtilityFunctions.getClosestSide(addVec);
        for (float i = -.5f; i < 1f; i += .5f)
        {
            for (float j = -.5f; j < 1f; j += .5f)
            {
                for (float k = -.5f; k < 1f; k += .5f)
                {
                    Vector3 curVec = new Vector3(addVec.x + i, addVec.y + j, addVec.z + k);
                    if (!storage.ContainsKey(curVec) &&
                        UtilityFunctions.validEnemyVector(curVec) &&
                        addSide == UtilityFunctions.getClosestSide(curVec))
                    {
                        storage.Add(curVec, wallIn);
                    }
                    else if (storage.ContainsKey(curVec))
                    {
                        if (duplicates.ContainsKey(curVec))
                        {
                            duplicates[curVec]++;
                        }
                        else
                        {
                            duplicates.Add(curVec, 2);
                        }
                    }
                }
            }
        }
                    /*
                    for (float i = -1; i < 1.5f; i += .5f)
                    {
                        for (float j = -1; j < 1.5f; j += .5f)
                        {
                            for (float k = -1; k < 1.5f; k += .5f)
                            {
                                Vector3 curVec = new Vector3(addVec.x + i, addVec.y + j, addVec.z + k);
                                if (!storage.ContainsKey(curVec) &&
                                    UtilityFunctions.validEnemyVector(curVec) &&
                                    addSide == UtilityFunctions.getClosestSide(curVec))
                                {
                                    storage.Add(curVec, wallIn);
                                }
                                else if (storage.ContainsKey(curVec))
                                {
                                    if (duplicates.ContainsKey(curVec))
                                    {
                                        duplicates[curVec]++;
                                    }
                                    else
                                    {
                                        duplicates.Add(curVec, 2);
                                    }
                                }
                            }
                        }
                    }*/
                    detectPathCollision();
    }

    public void removeWall(GameObject wallIn)
    {
        Vector3 removeVec = wallIn.transform.position;
        for (float i = -.5f; i < 1f; i += .5f)
        {
            for (float j = -.5f; j < 1f; j += .5f)
            {
                for (float k = -.5f; k < 1f; k += .5f)
                {
                    Vector3 curVec = new Vector3(removeVec.x + i, removeVec.y + j, removeVec.z + k);
                    if (duplicates.ContainsKey(curVec))
                    {
                        duplicates[curVec]--;
                        if (duplicates[curVec] <= 0)
                        {
                            duplicates.Remove(curVec);
                            storage.Remove(curVec);
                        }
                    }
                    else
                    {
                        storage.Remove(curVec);
                    }
                }
            }
        }
        if (!storage.ContainsValue(wallIn))
        {
            if (wallAndTowers.ContainsKey(wallIn))
            {
                detatchTowerAndReturn(wallIn);
            }
            Destroy(wallIn);
        }   
        else
        {
            Debug.Log("ERROR: STORAGE STILL CONTAINS WALL AFTER REMOVAL");
        }
    }

    public void detatchTowerAndReturn(GameObject wallIn)
    {
        GameObject tempTower = wallAndTowers[wallIn];
        towerInventory.playerInventory.Add(tempTower);
        tempTower.GetComponent<TowerStats>().attachedToPlayer = true;
        wallAndTowers.Remove(wallIn);
        towerStorage.removeTower(tempTower);
        ShootsBullets recoilSnapPositionTemp;
        if (tempTower.TryGetComponent<ShootsBullets>(out recoilSnapPositionTemp))
        {
            recoilSnapPositionTemp.snapPosition = Vector3.zero;
        }
    }

    public bool wallHasTower(GameObject wall)
    {
        return wallAndTowers.ContainsKey(wall);
    }
}
