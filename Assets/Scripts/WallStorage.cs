using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallStorage : MonoBehaviour
{
    private Dictionary<Vector3, GameObject> storage = new Dictionary<Vector3, GameObject>();
    public Dictionary<GameObject, GameObject> wallAndTowers = new Dictionary<GameObject, GameObject>();
    Dictionary<Vector3, int> duplicates = new Dictionary<Vector3, int>();
    TowerPlacer towerPlacer;
    private Stack<GameObject> wallStack;

    public List<Pathfinder> pathfinders;
    void Start()
    {
        towerPlacer = GetComponent<TowerPlacer>();
        wallStack = new Stack<GameObject>();
    }

    public bool isWall(Vector3 checkVec)
    {
        return storage.ContainsKey(checkVec);
    }

    public void popRecentWall()
    {
        Debug.Log("wall popped");
        removeWall(wallStack.Pop());
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
                if (!redoBuffer.Contains(pathFinder) && pathFinder.pathContainsVector(checkVec))
                {
                    redoBuffer.Add(pathFinder);
                }
            }
        }
        foreach (Pathfinder pathFinder in redoBuffer)
        {
            pathFinder.detectAndRedoPathSegments();
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
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    Vector3 curVec = new Vector3(checkVec.x + i, checkVec.y + j, checkVec.z + k);
                    if (side == Vector3.forward || side == Vector3.back)
                    {
                        if (Mathf.Abs(curVec.y) > 10 || Mathf.Abs(curVec.x) > 10)
                        {
                            return false;
                        }
                    } 
                    else if (side == Vector3.left || side == Vector3.right)
                    {
                        if (Mathf.Abs(curVec.z) > 10 || Mathf.Abs(curVec.y) > 10)
                        {
                            return false;
                        }
                    }
                    else if (side == Vector3.up || side == Vector3.down)
                    {
                        if (Mathf.Abs(curVec.x) > 10 || Mathf.Abs(curVec.z) > 10)
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
        Debug.Log("wall pushed");
        Vector3 addSide = UtilityFunctions.getClosestSide(addVec);
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
                {
                    Vector3 curVec = new Vector3(addVec.x + i, addVec.y + j, addVec.z + k);
                    if (UtilityFunctions.validEnemyVector(curVec) && addSide == UtilityFunctions.getClosestSide(curVec))
                    {
                        if (!storage.ContainsKey(curVec))
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
        }
        detectPathCollision();
    }

    public void removeWall(GameObject wallIn)
    {
        Vector3 removeVec = wallIn.transform.position;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                for (int k = -1; k < 2; k++)
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
                Destroy(wallAndTowers[wallIn]);
            }
            Destroy(wallIn);
        }   
        else
        {
            Debug.Log("ERROR: STORAGE STILL CONTAINS WALL AFTER REMOVAL");
        }
    }
}
