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

    public void enableRangeDisplay()
    {
        if (!rangeDisplayed)
        {
            rangeDisplayed = true;
            float range = towerStats.range;
            rangeIndicatorInstance = Instantiate(rangeIndicator, transform.position, Quaternion.identity);
            Vector3 currentSize = rangeIndicatorInstance.GetComponent<MeshRenderer>().bounds.size;
            Vector3 newScale = new Vector3(2 * range / currentSize.x, 2 * range / currentSize.y, 2 * range / currentSize.z);
            rangeIndicatorInstance.transform.localScale = newScale;
        }
       
    }

    public void disableRangeDisplay()
    {
        if (rangeDisplayed && rangeIndicatorInstance != null)
        {
            rangeDisplayed = false;
            Destroy(rangeIndicatorInstance);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.LeftShift))
        {
            RaycastHit hit;
            Vector2 mousePosition = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject && !towerStats.attachedToPlayer)
                {
                    enableRangeDisplay();
                }
                else
                {
                    disableRangeDisplay();
                }
            }
        }
    }
}
