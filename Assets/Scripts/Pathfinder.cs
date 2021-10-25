﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public class pos
    {
        public pos(Vector3 vectorIn)
        {
            v = vectorIn;
        }
        /*public pos(Vector3 vectorIn, float HCostIn, float GCostIn, float FCostIn) {
            v = vectorIn;
            HCost = HCostIn;
            Gcost = GCostIn;
            FCost = FCostIn;
        }*/

        public Vector3 v;
        public pos parent;
        public float GCost;//distance from start node, USING PARENT DISTANCe
        public float FCost;//gcost + hcost
        //when choosing pos from walkable tiles, get lowerst hcost
    }
    public GameObject pathFindingVisualizerSphere;
    public GameObject pathFindingVisualizerBar;
    public List<GameObject> checkPointList;
    public List<List<Vector3>> path;
    private List<HashSet<GameObject>> visualizers;
    private List<HashSet<Vector3>> pathVectors;
    private List<bool> finishedPaths;
    private WallStorage wallStorage;
    private TowerPlacer towerPlacer;
    private int speedThreshold;
    private const int speedThresholdTrigger = 1000;
    private int currentCount;

    int numCoroutinesRunning = 0;

    private void Start()
    {
        speedThreshold = 5;
        currentCount = 0;
        visualizers = new List<HashSet<GameObject>>();
        pathVectors = new List<HashSet<Vector3>>();
        for (int i = 0; i < checkPointList.Count - 1; i++)
        {
            visualizers.Add(new HashSet<GameObject>());
            pathVectors.Add(new HashSet<Vector3>());
        }

        towerPlacer = GetComponent<TowerPlacer>();
        wallStorage = GetComponent<WallStorage>();
        finishedPaths = new List<bool>();
        path = new List<List<Vector3>>();
        for (int i = 0; i < checkPointList.Count - 1; i++)
        {
            finishedPaths.Add(false);
            path.Add(new List<Vector3>());
        }

        findPath();
    }

    public List<List<Vector3> > getPath()
    {
        speedThreshold = 100;

        if (finishedPaths.Contains(false))
        {
            return new List<List<Vector3>>();
        }
        else return path;
    }

    private void Update()
    {
        if (numCoroutinesRunning > 6)
        {
            Debug.Log("ERROR: TOO MANY PATHFINIDNG COROUTINES RUNNING");
            findPath();
        }
    }

    public void resetCoroutineSpeed()
    {
        //maybe???
    }

    public IEnumerator findPathBetweenPoints(Vector3 startVec, Vector3 endVec, int pathIndex)
    {
        currentCount = 0;
        speedThreshold = 1;

        pos start = new pos(startVec);
        pos end = new pos(endVec);
        start.FCost = Vector3.Distance(start.v, end.v);
        start.parent = start;
        start.GCost = 0;
        start.FCost = Vector3.Distance(start.v, end.v);
        start.parent = start;
        start.GCost = 0;

        List<pos> activePath = new List<pos>();
        activePath.Add(start);
        List<pos> closedPath = new List<pos>();

        bool pathFound = false;
        int speedSwitch = 0;
        while (activePath.Count > 0)
        {
            currentCount++;
            speedSwitch++;
            if (currentCount > speedThresholdTrigger)
            {
                currentCount = 0;
                speedThreshold++;
            }
            if (speedSwitch > speedThreshold)
            {
                yield return new WaitForEndOfFrame();
                speedSwitch = 0;
            }

            float min = 10000;
            int minIndex = 0;
            for (int j = 0; j < activePath.Count; j++)
            {
                if (activePath[j].FCost < min)
                {
                    minIndex = j;
                    min = activePath[j].FCost;
                }
            }
            pos curPos = activePath[minIndex];
            activePath.RemoveAt(minIndex);
            closedPath.Add(curPos);

            foreach (Vector3 newVec in walkableTiles(curPos.v))
            {
                if (newVec == end.v)
                {
                    end.parent = curPos;
                    List<Vector3> temp = new List<Vector3>();
                    temp.Add(end.v);
                    while (end.parent.v != start.v)
                    {
                        end = end.parent;
                        temp.Add(end.v);
                        pathVectors[pathIndex].Add(end.v);
                        visualizers[pathIndex].Add(Instantiate(pathFindingVisualizerSphere, end.v, new Quaternion()));
                        if (wallStorage.isWall(end.v))
                        {
                            removeSegmentFromDictionary(pathIndex);
                            removeVisualizerSegment(pathIndex);
                            numCoroutinesRunning++;
                            StartCoroutine(findPathBetweenPoints(startVec, endVec, pathIndex));
                            numCoroutinesRunning--;
                            yield break;
                        }
                    }
                    temp.Reverse();
                    path[pathIndex] = temp;
                    numCoroutinesRunning--;
                    finishedPaths[pathIndex] = true;
                    yield break;
                }
                bool found = false;
                foreach (pos checkPos in closedPath)
                {
                    if (checkPos.v == newVec)
                    {
                        found = true;
                    }
                }
                if (!found)
                {
                    pos newPos = new pos(newVec);
                    newPos.parent = curPos;

                    float GCost = curPos.GCost + Vector3.Distance(newVec, curPos.v);
                    float Fcost = Vector3.Distance(end.v, newVec) + GCost;
                    newPos.GCost = GCost;
                    newPos.FCost = Fcost;
                    newPos.parent = curPos;
                    found = false;
                    foreach (pos checkPos in activePath)
                    {
                        if (checkPos.v == newVec)
                        {
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        activePath.Add(newPos);
                    }
                }
            }
        }
        if (activePath.Count == 0 && !pathFound)
        {
            towerPlacer.shadowTower.transform.position = new Vector3(25, 0, 0);
            wallStorage.popRecentWall();
            numCoroutinesRunning++;
            StartCoroutine(findPathBetweenPoints(startVec, endVec, pathIndex));
            numCoroutinesRunning--;
            yield break;
        }
    }

    public void findPath()
    {
        StopAllCoroutines();
        for (int i = 0; i < checkPointList.Count - 1; i++)
        {
            numCoroutinesRunning++;
            StartCoroutine(findPathBetweenPoints(checkPointList[i].transform.position, checkPointList[i + 1].transform.position, i));
        }
    }

    public void detectAndRedoPathSegments()
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (finishedPaths[i])
            {
                for (int j = 0; j < path[i].Count; j++)
                {
                    if (wallStorage.isWall(path[i][j]))
                    {
                        removeSegmentFromDictionary(i);
                        removeVisualizerSegment(i);
                        numCoroutinesRunning++;
                        //idea: only start coroutine if path is finished
                        finishedPaths[i] = false;
                        StartCoroutine(findPathBetweenPoints(checkPointList[i].transform.position, checkPointList[i + 1].transform.position, i));
                        j = path[i].Count;
                    }
                }
            }
        }
    }

    private void removeSegmentFromDictionary(int index)
    {
        foreach (Vector3 checkVec in path[index])
        {
            if (pathVectors[index].Contains(checkVec))
            {
                pathVectors[index].Remove(checkVec);
            }
        }
    }

    private void removeVisualizerSegment(int index)
    {
        foreach (GameObject visualizer in visualizers[index])
        {
            Destroy(visualizer);
        }
    }

    public bool pathContainsVector(Vector3 checkVec)
    {
        foreach (HashSet<Vector3> curSet in pathVectors)
        {
            if (curSet.Contains(checkVec))
            {
                return true;
            }
        }
        return false;
    }

    private List<Vector3> walkableTiles(Vector3 vec)
    {
        List<Vector3> walkable = new List<Vector3>();
        for (float i = -.5f; i < 1; i += .5f)
        {
            for (float j = -.5f; j < 1; j += .5f)    
            {
                for (float k = -.5f; k < 1; k += .5f)
                {
                    Vector3 checkVec = new Vector3(vec.x + i, vec.y + j, vec.z + k);
                    if (UtilityFunctions.validEnemyVector(checkVec) && !wallStorage.isWall(checkVec))
                    {
                        walkable.Add(checkVec);
                    }
                }
            }
        }
        return walkable;
    }
}
