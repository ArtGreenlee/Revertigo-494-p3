using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLighter : MonoBehaviour
{
    public GameObject lightprefab;
    private Pathfinder pf;
    private IEnumerator coroutine;
    private GameObject[] light;
    // Start is called before the first frame update
    void Start()
    {
        pf = GetComponent<Pathfinder>();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(Time.time/5.0f-Mathf.Round(Time.time/5.0f))<0.0001) 
        {
            if (pf != null)
            {
                Debug.Log(pf.path.Count);
                light = new GameObject[pf.path.Count];
                for (int j = 0; j < pf.path.Count; j++)
                {
                    light[j] = GameObject.Instantiate(lightprefab, pf.path[j][0], Quaternion.identity);
                    coroutine = LightEffect(pf.path[j],j);
                    StartCoroutine(coroutine);
                   
                }
            }
        }
    }

    private IEnumerator LightEffect(List<Vector3> path,int t)
    {        
        Debug.Log("Light initialized!");
        for (int i = 0; i < path.Count; i++) {
            yield return new WaitForSeconds(0.1f);
            light[t].transform.position = path[i];
            Debug.Log(path[i]);
            Debug.Log(t);
        }
        GameObject.Destroy(light[t]);
    }
}
