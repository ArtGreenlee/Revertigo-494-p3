using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    public GameObject rangeIndicator;
    private GameObject rangeIndicatorInstance;
    private TowerStats towerStats;
    private bool rangeDisplayed;
    private void Awake()
    {
        towerStats = GetComponent<TowerStats>();
    }

    private void Start()
    {
        rangeDisplayed = false;
    }

    private void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(ray, out hit) &&
            hit.collider.gameObject == gameObject && !rangeDisplayed && towerStats.range != 0)
            {
               // Debug.Log("range indicator");
                rangeDisplayed = true;
                float range = towerStats.range;
                rangeIndicatorInstance = Instantiate(rangeIndicator, transform.position, new Quaternion());
                /*Mesh m = gameObject.GetComponent<MeshFilter>().sharedMesh;
         Bounds meshBounds = m.bounds;
         Vector3 meshSize = meshBounds.size;
         float xScale = targetSize.x / meshSize.x;
         float yScale = targetSize.y / meshSize.y;
         float zScale = targetSize.z / meshSize.z;
         transform.localScale = new Vector3(xScale, yScale, zScale);*/
                Vector3 currentSize = rangeIndicatorInstance.GetComponent<MeshRenderer>().bounds.size;
                Vector3 newScale = new Vector3(2 * range / currentSize.x, 2 * range / currentSize.y, 2 * range / currentSize.z);
                rangeIndicatorInstance.transform.localScale = newScale;
            }
            else if(rangeDisplayed && rangeIndicatorInstance != null)
            {
                rangeDisplayed = false;
                Destroy(rangeIndicatorInstance);
            }
           
        }
        
    }
}
