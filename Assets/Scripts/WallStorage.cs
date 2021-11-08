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
            removeMostRecentWall();
        }
    }

    public void removeMostRecentWall()
    {
        if (wallStack.Count > 0)
        {
            removeWall(wallStack.Pop());
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
                        if (Mathf.Abs(curVec.y) > 16 || Mathf.Abs(curVec.x) > 16)
                        {
                            return false;
                        }
                    } 
                    else if (side == Vector3.left || side == Vector3.right)
                    {
                        if (Mathf.Abs(curVec.z) > 16 || Mathf.Abs(curVec.y) > 16)
                        {
                            return false;
                        }
                    }
                    else if (side == Vector3.up || side == Vector3.down)
                    {
                        if (Mathf.Abs(curVec.x) > 16 || Mathf.Abs(curVec.z) > 16)
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
        wallStack.Push(wallIn);
        Vector3 addSide = UtilityFunctions.getClosestSide(addVec);
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
        }
        detectPathCollision();
    }

    public void removeWall(GameObject wallIn)
    {
        Vector3 removeVec = wallIn.transform.position;
        for (float i = -1; i < 1.5f; i += .5f)
        {
            for (float j = -1; j < 1.5f; j += .5f)
            {
                for (float k = -1; k < 1.5f; k += .5f)
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
        towerInventory.playerInventory.Add(wallAndTowers[wallIn]);
        wallAndTowers[wallIn].GetComponent<TowerStats>().attachedToPlayer = true;
        wallAndTowers.Remove(wallIn);
    }

    public bool wallHasTower(GameObject wall)
    {
        return wallAndTowers.ContainsKey(wall);
    }
}
