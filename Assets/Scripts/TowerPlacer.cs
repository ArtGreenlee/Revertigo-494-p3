using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{
    public GameObject shadowTower;
    public GameObject wall;
    private WallStorage wallStorage;
    private Pathfinder pathFinder;

    public List<GameObject> gemRoster;
    // Start is called before the first frame update
    void Start()
    {
        pathFinder = GetComponent<Pathfinder>();
        wallStorage = GetComponent<WallStorage>();
        shadowTower = Instantiate(shadowTower, new Vector3(25,0,0), new Quaternion());
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Vector3 curPoint;
        Vector3 prevPoint = Vector3.zero;
        if (Physics.Raycast(ray, out hit) &&
            !Input.GetMouseButton(1))
        {
            curPoint = hit.point;
            curPoint = UtilityFunctions.snapVector(curPoint);
            if (!wallStorage.isWall(curPoint) && hit.collider.gameObject.tag != "Checkpoint")
            {
                shadowTower.transform.position = curPoint;
                shadowTower.transform.rotation = UtilityFunctions.getRotationawayFromSide(UtilityFunctions.getSide(curPoint));
                if (Input.GetMouseButtonDown(0))
                {
                    GameObject newWall = Instantiate(getRandomGem(), curPoint, shadowTower.transform.rotation);
                    shadowTower.transform.position = new Vector3(25, 0, 0);
                    wallStorage.addWall(curPoint, newWall);
                }
            }
        }
    }

    private GameObject getRandomGem()
    {
        return gemRoster[0];
    }
}
