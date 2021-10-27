using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallStorage : MonoBehaviour
{
    private Dictionary<Vector3, GameObject> storage = new Dictionary<Vector3, GameObject>();
    public Dictionary<GameObject, GameObject> wallAndTowers = new Dictionary<GameObject, GameObject>();
    private Pathfinder pathFinder;
    Dictionary<Vector3, int> duplicates = new Dictionary<Vector3, int>();
    TowerPlacer towerPlacer;
    private Stack<GameObject> wallStack;
    void Start()
    {
        towerPlacer = GetComponent<TowerPlacer>();
        pathFinder = GetComponent<Pathfinder>();
        wallStack = new Stack<GameObject>();
    }

    public bool isWall(Vector3 checkVec)
    {
        return storage.ContainsKey(checkVec);
    }

    public void popRecentWall()
    {
        removeWall(wallStack.Pop());
    }

    public void attachTowerToWall(GameObject towerIn, GameObject wallIn)
    {
        wallAndTowers.Add(wallIn, towerIn);
    }

    public bool validWallPosition(Vector3 checkVec)
    {
        if (isWall(checkVec))
        {
            return false;
        }
        UtilityFunctions.Side side = UtilityFunctions.getSide(checkVec);
        for (float i = -1; i < 1.5f; i += .5f)
        {
            for (float j = -1; j < 1.5f; j += .5f)
            {
                for (float k = -1; k < 1.5f; k += .5f)
                {
                    Vector3 curVec = new Vector3(checkVec.x + i, checkVec.y + j, checkVec.z + k);
                    Debug.Log(curVec.x);
                    if (side == UtilityFunctions.Side.front || side == UtilityFunctions.Side.back)
                    {
                        if (Mathf.Abs(curVec.y) > 10 || Mathf.Abs(curVec.x) > 10)
                        {
                            return false;
                        }
                    } 
                    else if (side == UtilityFunctions.Side.left || side == UtilityFunctions.Side.right)
                    {
                        if (Mathf.Abs(curVec.z) > 10 || Mathf.Abs(curVec.y) > 10)
                        {
                            return false;
                        }
                    }
                    else if (side == UtilityFunctions.Side.top || side == UtilityFunctions.Side.bottom)
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
        UtilityFunctions.Side addSide = UtilityFunctions.getSide(addVec);
        bool redo = false;
        for (float i = -1; i < 1.5f; i += .5f)
        {
            for (float j = -1; j < 1.5f; j += .5f)
            {
                for (float k = -1; k < 1.5f; k += .5f)
                {
                    Vector3 curVec = new Vector3(addVec.x + i, addVec.y + j, addVec.z + k);
                    if (!storage.ContainsKey(curVec) &&
                        UtilityFunctions.validEnemyVector(curVec) &&
                        addSide == UtilityFunctions.getSide(curVec))
                    {
                        if (pathFinder.pathContainsVector(curVec))
                        {
                            redo = true;
                        }
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
        if (redo)
        {
            pathFinder.detectAndRedoPathSegments();
        }
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
