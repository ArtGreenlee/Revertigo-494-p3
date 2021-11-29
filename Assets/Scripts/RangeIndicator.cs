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
            if (rangeIndicatorInstance != null)
            {
                Destroy(rangeIndicatorInstance);
            }
            rangeIndicatorInstance = Instantiate(rangeIndicator, transform.position, Quaternion.identity);
            Vector3 currentSize = rangeIndicatorInstance.GetComponent<MeshRenderer>().bounds.size;
            Vector3 newScale = new Vector3(2 * range / currentSize.x, 2 * range / currentSize.y, 2 * range / currentSize.z);
            StopAllCoroutines();
            StartCoroutine(UtilityFunctions.changeScaleOfTransformOverTime(rangeIndicatorInstance.transform, newScale.z, 10));
            //rangeIndicatorInstance.transform.localScale = newScale;
        }
       
    }

    public IEnumerator disableRangeDisplay()
    {
        if (rangeDisplayed && rangeIndicatorInstance != null)
        {
            rangeDisplayed = false;
            StopAllCoroutines();
            StartCoroutine(UtilityFunctions.changeScaleOfTransformOverTime(rangeIndicatorInstance.transform, 0, 10));
            yield return new WaitForSeconds(.5f);
            Destroy(rangeIndicatorInstance);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
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
                    StartCoroutine(disableRangeDisplay());
                }
            }
        }
    }
}
