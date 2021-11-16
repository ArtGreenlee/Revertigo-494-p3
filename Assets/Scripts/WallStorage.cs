using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class WallStorage : MonoBehaviour
{
    private Dictionary<Vector3, GameObject> storage;
    public Dictionary<GameObject, GameObject> wallAndTowers;
    Dictionary<Vector3, int> duplicates = new Dictionary<Vector3, int>();
    public List<Pathfinder> pathfinders;
    private Stack<GameObject> wallStack;
    private TowerStorage towerStorage;
    //private Dictionary<GameObject, List<Vector3> > placementLocations;
    public GameObject debugSphere;
    

    public HashSet<Vector3> forbiddenVectors;

    private TowerInventory towerInventory;

    public static WallStorage instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        storage = new Dictionary<Vector3, GameObject>();
        wallAndTowers = new Dictionary<GameObject, GameObject>();
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
        
        forbiddenVectors = new HashSet<Vector3>();
        //placementLocations = new Dictionary<GameObject, List<Vector3>>();
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

        return !forbiddenVectors.Contains(checkVec);
    }

    //add vec is the center of the wall
    public void addWall(Vector3 addVec, GameObject wallIn)
    {
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
                        /*if (!placementLocations.ContainsKey(wallIn))
                        {
                            placementLocations[wallIn] = new List<Vector3>();
                        }
                        placementLocations[wallIn].Add(curVec);*/
                        //Instantiate(debugSphere, curVec, Quaternion.identity);
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
                            duplicates.Add(curVec, 0);
                        }
                    }
                }
            }
        }

        /*for (float i = -1.5f; i < 2f; i += .5f)
        {
            for (float j = -1.5f; j < 2f; j += .5f)
            {
                for (float k = -1.5f; k < 2f; k += .5f)
                {
                    Vector3 testVec = new Vector3(addVec.x + i, addVec.y + j, addVec.z + k);
                    if (UtilityFunctions.validEnemyVector(testVec))
                    {
                        foreach (Pathfinder pathfinder in pathfinders)
                        {
                            if (pathfinder.pathContainsVector(testVec) && pathfinder.enemyMovement == null)
                            {
                                for (int x = 0; x < pathfinder.path.Count; x++)
                                {
                                    int vecIndex = pathfinder.path[x].IndexOf(testVec);
                                    if (vecIndex != -1)
                                    {
                                        if (!forbiddenVectors.Contains(pathfinder.path[x][vecIndex + 1]) ||
                                            !forbiddenVectors.Contains(pathfinder.path[x][vecIndex]) ||
                                            !forbiddenVectors.Contains(pathfinder.path[x][vecIndex - 1]))
                                            StartCoroutine(pathfinder.testForInvalidPath(
                                                pathfinder.checkPointVectors[x],
                                                pathfinder.path[x][vecIndex - 1],
                                                pathfinder.path[x][vecIndex],
                                                pathfinder.path[x][vecIndex + 1],
                                                x));
                                        //why do people let me do this
                                    }
                                }

                            }
                        }
                    }
                    

                }
            }
        }*/

        detectPathCollision();
    }

    public void removeWall(GameObject wallIn)
    {

        /*foreach (Vector3 removeVec in placementLocations[wallIn])
        {
            GameObject removeWall = storage[removeVec];
            if (duplicates.ContainsKey(removeVec))
            {
                duplicates[removeVec]--;
                if (duplicates[removeVec] == 0)
                {
                    duplicates.Remove(removeVec);
                    storage.Remove(removeVec);
                }
            }
            else
            {
                storage.Remove(removeVec);
            }
        }
        placementLocations.Remove(wallIn);
        */
        forbiddenVectors = new HashSet<Vector3>();
        if (storage.ContainsValue(wallIn))
        {
            foreach (KeyValuePair<Vector3, GameObject> wallVecPair in storage.Where(kvp => kvp.Value == wallIn).ToList())
            {
                /*if (duplicates.ContainsKey(wallVecPair.Key))
                {
                    duplicates[wallVecPair.Key]--;
                    if (duplicates[wallVecPair.Key] < 0)
                    {
                        duplicates.Remove(wallVecPair.Key);
                        storage.Remove(wallVecPair.Key);
                        Instantiate(debugSphere, wallVecPair.Key, Quaternion.identity);
                    }
                }
                else
                {
                    Instantiate(debugSphere, wallVecPair.Key, Quaternion.identity);
                    storage.Remove(wallVecPair.Key);
                }*/
                //Instantiate(debugSphere, wallVecPair.Key, Quaternion.identity);
                storage.Remove(wallVecPair.Key);
            }
        }
        Destroy(wallIn);



        /*Vector3 removeVec = wallIn.transform.position;
        placementLocations.Remove(wallIn);
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
        }*/
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
