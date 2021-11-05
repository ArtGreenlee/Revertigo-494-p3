using System.Collections;
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
        public float HCost;
        public float FCost;//gcost + hcost
        //when choosing pos from walkable tiles, get lowerst hcost
    }
    public GameObject pathFindingVisualizerSphere;
    public List<GameObject> checkPointList;
    public List<Vector3> checkPointVectors;
    public List<List<Vector3>> path;
    private List<HashSet<GameObject>> visualizers;
    private List<HashSet<Vector3>> pathVectors;
    private List<bool> finishedPaths;
    private WallStorage wallStorage;
    private WallPlacer towerPlacer;
    private int speedThreshold;
    private const int speedThresholdTrigger = 1000;
    private int currentCount;

    public EnemyMovement enemyMovement;

    int numCoroutinesRunning = 0;

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Awake()
    {
        towerPlacer = GameObject.Find("GameController").GetComponent<WallPlacer>();
        wallStorage = GameObject.Find("GameController").GetComponent<WallStorage>();
        wallStorage.pathfinders.Add(this);
        speedThreshold = 5;
        currentCount = 0;

        checkPointList.Insert(0, gameObject);
        checkPointVectors = new List<Vector3>();

        foreach (GameObject temp in checkPointList)
        {
            checkPointVectors.Add(temp.transform.position); 
        }

        if (!TryGetComponent(out enemyMovement))
        {
            findPath();
        }
        else
        {
            enemyMovement = GetComponent<EnemyMovement>();
        }
    }

    private void FixedUpdate()
    {
        if (numCoroutinesRunning >= checkPointVectors.Count)
        {
            Debug.Log("curent: " + numCoroutinesRunning + " max: " + checkPointVectors.Count);
            Debug.LogError("ERROR: TOO MANY COROUTINES RUNNING");
        }
    }

    public List<List<Vector3> > getPath()
    {
        if (finishedPaths.Contains(false))
        {
            return new List<List<Vector3>>();
        }
        else return path;
    }

    /*public IEnumerator findPathBetweenPointsUber(Vector3 startVec, Vector3 endVec, int pathIndex)
    {
        finishedPaths[pathIndex] = false;

        pos startOne = new pos(startVec);
        pos endOne = new pos(endVec);
        startOne.FCost = Vector3.Distance(startOne.v, endOne.v);
        startOne.parent = startOne;
        endOne.GCost = 0;

        pos startTwo = new pos(endVec);
        pos endTwo = new pos(startVec);


        SortedList<float, pos> activePathOne = new SortedList<float, pos>();
        HashSet<Vector3> activePathVectorsOne = new HashSet<Vector3>();
        activePathOne.Add(startOne.FCost, startOne);
        activePathVectorsOne.Add(startVec);
        HashSet<Vector3> closedPathOne = new HashSet<Vector3>();
        Dictionary<float, SortedList<float, pos>> FCostDuplicatesOne = new Dictionary<float, SortedList<float, pos>>();

        SortedList<float, pos> activePathTwo = new SortedList<float, pos>();
        HashSet<Vector3> activePathVectorsTwo = new HashSet<Vector3>();
        activePathVectorsTwo.Add(endVec);
        activePathTwo.Add(startTwo.FCost, startTwo);
        HashSet<Vector3> closedPathTwo = new HashSet<Vector3>();
        Dictionary<float, SortedList<float, pos>> FCostDuplicatesTwo = new Dictionary<float, SortedList<float, pos>>();


        while (activePathOne.Count > 0 && activePathTwo.Count > 0)
        {
            pos curPosOne = activePathOne.Values[0];
            if (FCostDuplicatesOne.ContainsKey(curPosOne.FCost))
            {
                curPosOne = FCostDuplicatesOne[curPosOne.FCost].Values[0];
                FCostDuplicatesOne[curPosOne.FCost].RemoveAt(0);
            }
            activePathOne.RemoveAt(0);
            activePathVectorsOne.Remove(curPosOne.v);
            closedPathOne.Add(curPosOne.v);

            pos curPosTwo = activePathTwo.Values[0];
            if (FCostDuplicatesTwo.ContainsKey(curPosTwo.FCost))
            {
                curPosTwo = FCostDuplicatesTwo[curPosTwo.FCost].Values[0];
                FCostDuplicatesTwo[curPosTwo.FCost].RemoveAt(0);
            }
            activePathTwo.RemoveAt(0);
            activePathVectorsTwo.Remove(curPosTwo.v);
            closedPathTwo.Add(curPosTwo.v);


            if (curPosOne.v == curPosTwo.v)
            {
                path[pathIndex].Clear();
                while (curPosOne.v != startVec)
                {
                    if (wallStorage.isWall(curPosOne.v))
                    {
                        //RERUN
                        removeSegmentFromDictionary(pathIndex);
                        removeVisualizerSegment(pathIndex);
                        StartCoroutine(findPathBetweenPointsUber(startVec, endVec, pathIndex));
                        yield break;
                    }

                    path[pathIndex].Insert(0, curPosOne.v);
                    pathVectors[pathIndex].Add(curPosOne.v);
                    if (enemyMovement == null)
                    {
                        visualizers[pathIndex].Add(Instantiate(pathFindingVisualizerSphere, curPosOne.v, new Quaternion()));
                    }
                    curPosOne = curPosOne.parent;
                }

                while (curPosTwo.v != endVec)
                {
                    if (wallStorage.isWall(curPosTwo.v))
                    {
                        //RERUN
                        removeSegmentFromDictionary(pathIndex);
                        removeVisualizerSegment(pathIndex);
                        StartCoroutine(findPathBetweenPointsUber(startVec, endVec, pathIndex));
                        yield break;
                    }

                    path[pathIndex].Add(curPosTwo.v);
                    pathVectors[pathIndex].Add(curPosTwo.v);
                    if (enemyMovement == null)
                    {
                        visualizers[pathIndex].Add(Instantiate(pathFindingVisualizerSphere, curPosTwo.v, new Quaternion()));
                    }
                    curPosTwo = curPosTwo.parent;
                }
                path[pathIndex].Add(endVec);

                finishedPaths[pathIndex] = true;
                numCoroutinesRunning--;
                yield break;
            }

            foreach (Vector3 newVec in walkableTiles(curPosOne.v))
            {
                Instantiate(pathFindingVisualizerSphere, curPosOne.v, new Quaternion());
                yield return new WaitForSeconds(.1f);
                if (!closedPathOne.Contains(newVec) && !activePathVectorsOne.Contains(newVec))
                {
                    pos newPos = new pos(newVec);
                    newPos.parent = curPosOne;
                    float HCostTemp = Vector3.Distance(endVec, newVec);
                    float GCostTemp = curPosOne.GCost + Vector3.Distance(newVec, curPosOne.v);
                    float FCostTemp = GCostTemp + HCostTemp;
                    newPos.HCost = HCostTemp;
                    newPos.GCost = GCostTemp;
                    newPos.FCost = FCostTemp;

                    if (activePathOne.ContainsKey(newPos.FCost))
                    {
                        //FCOST DUPLICATE
                        if (!FCostDuplicatesOne.ContainsKey(newPos.FCost))
                        {
                            SortedList<float, pos> tempSlist = new SortedList<float, pos>();
                            tempSlist.Add(newPos.HCost, newPos);
                            FCostDuplicatesOne.Add(newPos.FCost, tempSlist);
                        }
                        else
                        {
                            while (FCostDuplicatesOne[newPos.FCost].ContainsKey(newPos.HCost))
                            {
                                newPos.HCost -= .01f;
                            }

                            FCostDuplicatesOne[newPos.FCost].Add(newPos.HCost, newPos);

                        }
                    }
                    else
                    {
                        activePathVectorsOne.Add(newVec);
                        activePathOne.Add(newPos.FCost, newPos);
                    }
                }
            }

            foreach (Vector3 newVec in walkableTiles(curPosTwo.v))
            {
                Instantiate(pathFindingVisualizerSphere, curPosTwo.v, new Quaternion());
                yield return new WaitForSeconds(.1f);
                if (!closedPathTwo.Contains(newVec) && !activePathVectorsTwo.Contains(newVec))
                {
                    pos newPos = new pos(newVec);
                    newPos.parent = curPosTwo;
                    float HCostTemp = Vector3.Distance(startVec, newVec);
                    float GCostTemp = curPosTwo.GCost + Vector3.Distance(newVec, curPosTwo.v);
                    float FCostTemp = GCostTemp + HCostTemp;
                    newPos.HCost = HCostTemp;
                    newPos.GCost = GCostTemp;
                    newPos.FCost = FCostTemp;

                    if (activePathTwo.ContainsKey(newPos.FCost))
                    {
                        //FCOST DUPLICATE
                        if (!FCostDuplicatesTwo.ContainsKey(newPos.FCost))
                        {
                            SortedList<float, pos> tempSlist = new SortedList<float, pos>();
                            tempSlist.Add(newPos.HCost, newPos);
                            FCostDuplicatesTwo.Add(newPos.FCost, tempSlist);
                        }
                        else
                        {
                            while (FCostDuplicatesTwo[newPos.FCost].ContainsKey(newPos.HCost))
                            {
                                newPos.HCost -= .01f;
                            }
                            FCostDuplicatesTwo[newPos.FCost].Add(newPos.HCost, newPos);
                        }
                    }
                    else
                    {
                        activePathVectorsTwo.Add(newVec);
                        activePathTwo.Add(newPos.FCost, newPos);
                    }
                }
            }
        }
        if (enemyMovement == null)
        {
            wallStorage.removeMostRecentWall();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
        }
        towerPlacer.shadowTower.transform.position = new Vector3(25, 0, 0);
        numCoroutinesRunning++;
        StartCoroutine(findPathBetweenPointsUber(startVec, endVec, pathIndex));
        numCoroutinesRunning--;
        yield break;    
    }*/

    public IEnumerator findPathBetweenPointsLyft(Vector3 startVec, Vector3 endVec, int pathIndex)
    {
        finishedPaths[pathIndex] = false;
        
        pos start = new pos(startVec);
        pos end = new pos(endVec);
        start.FCost = (start.v - end.v).sqrMagnitude; 
        start.parent = start;
        start.GCost = 0;

        SortedList<float, pos> activePath = new SortedList<float, pos>();
        HashSet<Vector3> activePathVectors = new HashSet<Vector3>();
        activePath.Add(start.FCost, start);
        activePathVectors.Add(start.v);
        HashSet<Vector3> closedPath = new HashSet<Vector3>();
        Dictionary<float, List<pos>> FCostDuplicates = new Dictionary<float, List<pos>>();

        int speedSwitch = 5;
        currentCount = 0;
        speedThreshold = 1000;
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
            pos curPos = activePath.Values[0];
            if (FCostDuplicates.ContainsKey(curPos.FCost))
            {
                float minHCost = 1000;
                //int minHCostIndex = -1;
                foreach (pos checkPos in FCostDuplicates[curPos.FCost])
                {
                    if (checkPos.HCost < minHCost) {
                        
                    }
                }
            }
            activePath.RemoveAt(0);
            activePathVectors.Remove(curPos.v);
            closedPath.Add(curPos.v);

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
                        //only show visualizer if this is a main path
                        if (enemyMovement == null)
                        {
                            GameObject newVisualizer = Instantiate(pathFindingVisualizerSphere, end.v, new Quaternion());
                            newVisualizer.GetComponent<PathVisualizerEffects>().fadeIn();
                            visualizers[pathIndex].Add(newVisualizer);
                        }
                        if (wallStorage.isWall(end.v))
                        {
                            //rerun this coroutine
                            removeSegmentFromDictionary(pathIndex);
                            StartCoroutine(findPathBetweenPointsLyft(startVec, endVec, pathIndex));
                            yield break;
                        }
                    }
                    temp.Reverse();
                    path[pathIndex] = temp;
                    finishedPaths[pathIndex] = true;
                    numCoroutinesRunning--;
                    yield break;
                }


                if (!closedPath.Contains(newVec))
                {
                    pos newPos = new pos(newVec);
                    newPos.parent = curPos;
                    //this distance calculation could be optimized
                    float GCost = curPos.GCost + (newVec - curPos.v).sqrMagnitude;
                    float HCost = (end.v - newVec).sqrMagnitude; 
                    float Fcost = GCost + HCost;
                    newPos.HCost = HCost;
                    newPos.GCost = GCost;
                    newPos.FCost = Fcost;
                    newPos.parent = curPos;

                    if (!activePathVectors.Contains(newVec))
                    {
                        while (activePath.ContainsKey(newPos.FCost))
                        {
                            if (FCostDuplicates.ContainsKey(newPos.FCost))
                            {
                                FCostDuplicates[newPos.FCost].Add(newPos);
                            }
                            else
                            {
                                List<pos> tempNewList = new List<pos>();
                                tempNewList.Add(newPos);
                                FCostDuplicates.Add(newPos.FCost, tempNewList);
                            }
                            //holy moly this is bad my god this needs to be fixed TODO TODO TODO
                            newPos.FCost += .01f;
                        }
                        activePathVectors.Add(newPos.v);
                        activePath.Add(newPos.FCost, newPos);
                    }
                }
            }
        }
        if (activePath.Count == 0)
        {
            Debug.Log("path not found");
            if (enemyMovement == null)
            {
                wallStorage.removeMostRecentWall();
            }
            else
            {
                yield return new WaitForSeconds(.5f);
            }

            //towerPlacer.shadowTower.transform.position = new Vector3(25, 0, 0);
            StartCoroutine(findPathBetweenPointsLyft(startVec, endVec, pathIndex));
            yield break;
        }
    }

    /*public IEnumerator findPathBetweenPointsFast(Vector3 startVec, Vector3 endVec, int pathIndex)
    {
        currentCount = 0;
        speedThreshold = 1;
        finishedPaths[pathIndex] = false;

        pos start = new pos(startVec);
        pos end = new pos(endVec);
        start.FCost = Vector3.Distance(start.v, end.v);
        start.parent = start;
        start.GCost = 0;

        SortedList<float, pos> activePath = new SortedList<float, pos>();
        activePath.Add(start.FCost, start);
        Dictionary<Vector3, pos> closedPath = new Dictionary<Vector3, pos>();

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

            pos curPos = activePath.Values[0];
            activePath.RemoveAt(0);
            closedPath.Add(curPos.v, curPos);

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
                        //only show visualizer if this is a main path
                        if (enemyMovement == null)
                        {
                            visualizers[pathIndex].Add(Instantiate(pathFindingVisualizerSphere, end.v, new Quaternion()));
                        }
                        if (wallStorage.isWall(end.v))
                        {
                            //rerun this coroutine
                            removeSegmentFromDictionary(pathIndex);
                            removeVisualizerSegment(pathIndex);
                            numCoroutinesRunning++;
                            StartCoroutine(findPathBetweenPointsFast(startVec, endVec, pathIndex));
                            numCoroutinesRunning--;
                            yield break;
                        }
                    }
                    temp.Reverse();
                    path[pathIndex] = temp;
                    finishedPaths[pathIndex] = true;
                    numCoroutinesRunning--;
                    yield break;
                }


                if (!closedPath.ContainsKey(newVec))
                {
                    pos newPos = new pos(newVec);
                    newPos.parent = curPos;
                    float GCost = curPos.GCost + Vector3.Distance(newVec, curPos.v);
                    float Fcost = Vector3.Distance(end.v, newVec) + GCost;
                    newPos.GCost = GCost;
                    newPos.FCost = Fcost;
                    newPos.parent = curPos;

                    bool found = false;
                    foreach (pos checkPos in activePath.Values)
                    {
                        if (checkPos.v == newVec)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        while (activePath.ContainsKey(newPos.FCost))
                        {
                            //holy moly this is bad my god this needs to be fixed TODO TODO TODO
                            newPos.FCost -= .01f;
                        }
                        activePath.Add(newPos.FCost, newPos);
                    }
                }
            }
        }
        if (activePath.Count == 0 && !pathFound)
        {
            Debug.Log("path not found");
            if (enemyMovement == null) {
                wallStorage.removeMostRecentWall();
            }
            else
            {
                yield return new WaitForSeconds(.5f);
            }
            
            towerPlacer.shadowTower.transform.position = new Vector3(25, 0, 0);
            numCoroutinesRunning++;
            StartCoroutine(findPathBetweenPointsFast(startVec, endVec, pathIndex));
            numCoroutinesRunning--;
            yield break;
        }
    }*/

    public void findPath()
    {
        StopAllCoroutines(); //only applies to this script instance
        numCoroutinesRunning = 0;
        if (visualizers != null)
        {
            for (int i = 0; i < visualizers.Count; i++)
            {
                removeVisualizerSegment(i);
            }
        }
        visualizers = new List<HashSet<GameObject>>();
        pathVectors = new List<HashSet<Vector3>>();
        for (int i = 0; i < checkPointVectors.Count - 1; i++)
        {
            visualizers.Add(new HashSet<GameObject>());
            pathVectors.Add(new HashSet<Vector3>());
        }
        path = new List<List<Vector3>>();
        finishedPaths = new List<bool>();
        for (int i = 0; i < checkPointVectors.Count - 1; i++)
        {
            finishedPaths.Add(false);
            path.Add(new List<Vector3>());
        }
        for (int i = 0; i < checkPointVectors.Count - 1; i++)
        {
            numCoroutinesRunning++;
            StartCoroutine(findPathBetweenPointsLyft(checkPointVectors[i], checkPointVectors[i + 1], i));
        }
    }

    public void detectAndRedoPathSegments()
    {
        for (int i = 0; i < path.Count; i++)
        {
            if (finishedPaths[i])
            {
                //could use the dictionary
                for (int j = 0; j < path[i].Count; j++)
                {
                    if (wallStorage.isWall(path[i][j]))
                    {
                        removeSegmentFromDictionary(i);
                        removeVisualizerSegment(i);
                        finishedPaths[i] = false;
                        numCoroutinesRunning++;
                        if (this != null)
                        {
                            StartCoroutine(findPathBetweenPointsLyft(checkPointVectors[i], checkPointVectors[i + 1], i));
                        }
                        break;
                    }
                }
            }
        }
    }

    private void removeSegmentFromDictionary(int index)
    {
        pathVectors[index].Clear();
    }

    private void removeVisualizerSegment(int index)
    {
        if (enemyMovement == null)
        {
            foreach (GameObject visualizer in visualizers[index])
            {
                Instantiate(pathFindingVisualizerSphere, visualizer.transform.position, new Quaternion()).GetComponent<PathVisualizerEffects>().fadeOut();
                Destroy(visualizer);
            }
            visualizers[index].Clear();
        }
    }

    public bool pathFinished()
    {
        if (finishedPaths != null)
        {
            return !finishedPaths.Contains(false);
        }
        return false;
    }

    public bool pathContainsVector(Vector3 checkVec)
    {
        if (pathVectors != null)
        {
            foreach (HashSet<Vector3> curSet in pathVectors)
            {
                if (curSet != null)
                {
                    if (curSet.Contains(checkVec))
                    {
                        return true;
                    }
                }
                
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
