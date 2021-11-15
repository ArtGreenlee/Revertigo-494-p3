using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public class pos
    {

        public pos instance;


        public pos(Vector3 vectorIn)
        {
            v = vectorIn;
            instance = this;
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
    private GameObject collisionWall;
    private WallPlacer wallPlacer;

    private ObjectPooler objectPooler;

    public EnemyMovement enemyMovement;

    int numCoroutinesRunning = 0;

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Start()
    {
        wallPlacer = WallPlacer.instance;
        wallStorage = WallStorage.instance;
        wallStorage.pathfinders.Add(this);
        
        checkPointVectors = new List<Vector3>();
        checkPointList.Insert(0, gameObject);
        objectPooler = ObjectPooler.Instance;
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
            Debug.LogError("curent: " + numCoroutinesRunning + " max: " + checkPointVectors.Count);
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
                        visualizers[pathIndex].Add(Instantiate(pathFindingVisualizerSphere, curPosOne.v, Quaternion.identity));
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
                        visualizers[pathIndex].Add(Instantiate(pathFindingVisualizerSphere, curPosTwo.v, Quaternion.identity));
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
                Instantiate(pathFindingVisualizerSphere, curPosOne.v, Quaternion.identity);
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
                Instantiate(pathFindingVisualizerSphere, curPosTwo.v, Quaternion.identity);
                yield return new WaitForSeconds(.1f);
                if (!closedPathTwo.Contains(newVec) && !activePathVectorsTwo.Contains(newVec))
                {
                    pos newPos = new pos(newVec);
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
        wallPlacer.shadowTower.transform.position = new Vector3(25, 0, 0);
        numCoroutinesRunning++;
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
        start.v = startVec;
        start.GCost = 0;

        SortedList<float, pos> activePath = new SortedList<float, pos>();
        Dictionary<Vector3, pos> activePathVectors = new Dictionary<Vector3, pos>();
        activePath.Add(start.FCost, start);
        activePathVectors.Add(start.v, start);
        HashSet<Vector3> closedPath = new HashSet<Vector3>();

        while (activePath.Count > 0)
        {
            /*while (Input.GetMouseButton(1))
            {
                yield return new WaitForEndOfFrame();
            }*/
            pos curPos = activePath.Values[0];
            activePath.RemoveAt(0);
            activePathVectors.Remove(curPos.v);
            closedPath.Add(curPos.v);

            foreach (Vector3 newVec in walkableTiles(curPos.v))
            {
                //Instantiate(pathFindingVisualizerSphere, newVec, Quaternion.identity).GetComponent<PathVisualizerEffects>().fadeIn();
                //yield return new WaitForSeconds(.1f);
                
                if (newVec == endVec)
                {
                    Vector3 traverseVec = endVec;
                    bool showPath = enemyMovement == null;
                    List<Vector3> tempPath = new List<Vector3>();
                    tempPath.Add(traverseVec);
                    Vector3 previousVec;

                    while (traverseVec != startVec)
                    {
                        pathVectors[pathIndex].Add(traverseVec);
                        tempPath.Add(traverseVec);
                        previousVec = traverseVec;
                        traverseVec = curPos.parent.v;
                        curPos = curPos.parent;

                        if (showPath && walkableTiles(traverseVec).Count == 2 && (
                            !wallStorage.forbiddenVectors.Contains(traverseVec) ||
                            !wallStorage.forbiddenVectors.Contains(previousVec) ||
                            !wallStorage.forbiddenVectors.Contains(curPos.parent.v)))
                        {
                            StartCoroutine(testForInvalidPath(startVec, previousVec, traverseVec, curPos.parent.v, pathIndex));
                        }

                        //only show visualizer if this is a main path
                        if (showPath)
                        {
                            GameObject visualizer = Instantiate(pathFindingVisualizerSphere, traverseVec, Quaternion.identity);
                            visualizer.GetComponent<PathVisualizerEffects>().fadeIn();
                            visualizers[pathIndex].Add(visualizer);
                        }

                        if (wallStorage.isWall(traverseVec))
                        {
                            //wallStorage.removeWall(wallStorage.getWall(traverseVec));
                            //rerun this coroutine
                            
                            removeSegmentFromDictionary(pathIndex);
                            removeVisualizerSegment(pathIndex);
                            /*while (Input.GetMouseButton(1))
                            {
                                yield return new WaitForEndOfFrame();
                            }*/
                            collisionWall = wallStorage.getWall(traverseVec);
                            StartCoroutine(findPathBetweenPointsLyft(startVec, endVec, pathIndex));
                            yield break;
                        }

                        //
                    }
                    tempPath.Add(traverseVec);
                    tempPath.Reverse(); // O(n) bad bad
                    path[pathIndex] = tempPath;
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
                    float HCost = (newVec - end.v).sqrMagnitude;
                    float Fcost = GCost + HCost;
                    newPos.HCost = HCost;
                    newPos.GCost = GCost;
                    newPos.FCost = Fcost;
                    newPos.parent = curPos;

                    if (!activePathVectors.ContainsKey(newVec))
                    {
                        activePathVectors.Add(newPos.v, newPos);
                        while (activePath.ContainsKey(newPos.FCost))
                        {
                            newPos.FCost -= .01f;
                        }
                        activePath.Add(newPos.FCost, newPos);
                        yield return new WaitForEndOfFrame();
                    }
                    else if (activePathVectors[newVec].FCost > newPos.FCost)
                    {
                        activePathVectors[newVec].FCost = newPos.FCost;
                        activePathVectors[newVec].parent = curPos;
                        foreach (pos tempPos in activePath.Values)
                        {
                            if (tempPos.v == newVec)
                            {
                                pos replacePos = new pos(newVec);
                                replacePos.FCost = newPos.FCost;
                                replacePos.GCost = newPos.GCost;
                                replacePos.parent = curPos;
                                activePath[Fcost] = replacePos;
                                /*activePath.Remove(tempPos.FCost);
                                activePath.Add(newPos.FCost, replacePos);
                                //activePath[tempPos.FCost].FCost = newPos.FCost;
                                //activePath[tempPos.FCost].parent = curPos;*/
                                break;
                            }
                        }
                    }
                }
            }
        }
        //path is not found
        if (enemyMovement == null)
        {
            /*if (collisionWall != null)
            {
                wallStorage.removeWall(collisionWall);
            }
            else
            {
                wallStorage.removeMostRecentWall();
            }*/
            wallStorage.removeMostRecentWall();
            collisionWall = null;
        }
        else
        {
            yield return new WaitForSeconds(.5f);
        }

        //wallPlacer.shadowTower.transform.position = new Vector3(25, 0, 0);
        StartCoroutine(findPathBetweenPointsLyft(startVec, endVec, pathIndex));
        yield break;
    }

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

    public IEnumerator testForInvalidPath(Vector3 start, Vector3 end, Vector3 psuedoWall, Vector3 parentVec, int pathIndex)
    {
        // if there is no path between start and end, return true 
        Stack<Vector3> activePath = new Stack<Vector3>();
        activePath.Push(start);
        HashSet<Vector3> activePathVectors = new HashSet<Vector3>();
        HashSet<Vector3> closedPath = new HashSet<Vector3>();
        closedPath.Add(psuedoWall);
        while (activePath.Count > 0)
        {
            closedPath.Add(activePath.Peek());
            activePathVectors.Remove(activePath.Peek());
            foreach (Vector3 addVec in walkableTiles(activePath.Pop()))
            {
                if (addVec == end)
                {
                    yield break;
                }
                if (!closedPath.Contains(addVec) && !activePathVectors.Contains(addVec))
                {
                    activePath.Push(addVec);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        if (!wallStorage.forbiddenVectors.Contains(parentVec))
        {
            wallStorage.forbiddenVectors.Add(parentVec);
        }
        if (!wallStorage.forbiddenVectors.Contains(end))
        {
            wallStorage.forbiddenVectors.Add(end);
        }
        if (!wallStorage.forbiddenVectors.Contains(psuedoWall))
        {
            wallStorage.forbiddenVectors.Add(psuedoWall);
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
                Instantiate(pathFindingVisualizerSphere, visualizer.transform.position, Quaternion.identity).GetComponent<PathVisualizerEffects>().fadeOut();
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
        /*foreach (Vector3 sideVec in UtilityFunctions.sideVectors)
        {
            Vector3 checkVec = vec + (sideVec / 2);
            if (!wallStorage.isWall(checkVec))
            {
                for (float i = -.5f; i < 1; i += .5f)
                {
                    Vector3 addVec = checkVec + sideVec * i;
                    if (UtilityFunctions.validEnemyVector(addVec) && !wallStorage.isWall(checkVec))
                    {
                        walkable.Add(addVec);
                    }
                }
            }
        }

        
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
        /*
        for (float x = -.5f; x < 1; x += .5f)
        {
            for (float y = -.5f; y < 1; y += .5f)
            {
                for (float z = -.5f; z < 1; z += .5f)
                {
                    Vector3 checkVec = new Vector3(vec.x + x, vec.y + y, vec.z + z);
                    if (UtilityFunctions.validEnemyVector(checkVec) && !wallStorage.isWall(checkVec))
                    {
                        walkable.Add(checkVec);
                    }
                    if (y == 0)

                }
            }
        }*/

        foreach (Vector3 sideVec in UtilityFunctions.sideVectors)
        {
            Vector3 checkVec = vec + (sideVec / 2);
            if (!wallStorage.isWall(checkVec) && UtilityFunctions.validEnemyVector(checkVec))
            {
                walkable.Add(checkVec);
            }
        }

        


        /*Vector3 checkVec = vec + Vector3.up / 2;
        if (!wallStorage.isWall(checkVec))
        {

            for (float i = -.5f; i < 1; i += .5f)
            {
                Vector3 addVec = new Vector3(checkVec.x + i, checkVec.y, checkVec.z);
                if (UtilityFunctions.validEnemyVector(addVec) && !wallStorage.isWall(addVec))
                {
                    walkable.Add(addVec);
                }
            }
        }
        checkVec = vec + Vector3.down / 2;
        if (!wallStorage.isWall(checkVec))
        {

            for (float i = -.5f; i < 1; i += .5f)
            {
                Vector3 addVec = new Vector3(checkVec.x + i, checkVec.y, checkVec.z);
                if (UtilityFunctions.validEnemyVector(addVec) && !wallStorage.isWall(addVec))
                {
                    walkable.Add(addVec);
                }
            }
        }
        checkVec = vec + Vector3.left / 2;
        if (!wallStorage.isWall(checkVec))
        {

            for (float i = -.5f; i < 1; i += .5f)
            {
                Vector3 addVec = new Vector3(checkVec.x, checkVec.y, checkVec.z + i);
                if (UtilityFunctions.validEnemyVector(addVec) && !wallStorage.isWall(addVec))
                {
                    walkable.Add(addVec);
                }
            }
        }
        checkVec = vec + Vector3.right / 2;
        if (!wallStorage.isWall(checkVec))
        {

            for (float i = -.5f; i < 1; i += .5f)
            {
                Vector3 addVec = new Vector3(checkVec.x, checkVec.y, checkVec.z + i);
                if (UtilityFunctions.validEnemyVector(addVec) && !wallStorage.isWall(addVec))
                {
                    walkable.Add(addVec);
                }
            }
        }
        checkVec = vec + Vector3.forward / 2;
        if (!wallStorage.isWall(checkVec))
        {

            for (float i = -.5f; i < 1; i += .5f)
            {
                Vector3 addVec = new Vector3(checkVec.x, checkVec.y + i, checkVec.z);
                if (UtilityFunctions.validEnemyVector(addVec) && !wallStorage.isWall(addVec))
                {
                    walkable.Add(addVec);
                }
            }
        }
        checkVec = vec + Vector3.back / 2;
        if (!wallStorage.isWall(checkVec))
        {

            for (float i = -.5f; i < 1; i += .5f)
            {
                Vector3 addVec = new Vector3(checkVec.x, checkVec.y + i, checkVec.z);
                if (UtilityFunctions.validEnemyVector(addVec) && !wallStorage.isWall(addVec))
                {
                    walkable.Add(addVec);
                }
            }
        }*/
        return walkable;
    }

}
