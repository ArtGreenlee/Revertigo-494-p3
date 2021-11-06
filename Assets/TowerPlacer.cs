using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacer : MonoBehaviour
{

    private WallStorage wallStorage;
    private TowerInventory towerInventory; 
    // Start is called before the first frame update
    void Start()
    {
        wallStorage = WallStorage.instance;
        towerInventory = TowerInventory.instance;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        Vector3 curPoint;
        if (Physics.Raycast(ray, out hit))
        {
            curPoint = hit.point;
            curPoint = UtilityFunctions.snapVector(curPoint);
            if (wallStorage.isWall(curPoint))
            {
                Debug.Log("valid tower spot");
            }
        }
    }

    public static bool validTowerPlacement(Vector3 checkVec)
    {
        Vector3 side = UtilityFunctions.getClosestSide(checkVec);
        if (side == Vector3.forward || side == Vector3.back)
        {
            return Mathf.Abs(checkVec.x) < 14 && Mathf.Abs(checkVec.y) < 14;
        }
        else if (side == Vector3.left || side == Vector3.right)
        {
            return Mathf.Abs(checkVec.z) < 14 && Mathf.Abs(checkVec.y) < 14;
        }
        else if (side == Vector3.up || side == Vector3.down)
        {
            return Mathf.Abs(checkVec.x) < 14 && Mathf.Abs(checkVec.z) < 14;
        }
        else
        {
            Debug.Log("ERROR: SIDE DOES NOT EXIST");
            return false;
        }
    }
}
