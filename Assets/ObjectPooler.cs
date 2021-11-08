using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    // Start is called before the first frame update
    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject tempObject = Instantiate(pool.prefab);
                tempObject.SetActive(false);
                objectPool.Enqueue(tempObject);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject getObjectFromPool(string tag, Vector3 position, Quaternion rotation)
    {

        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.Log("TAG DOESNT EXIST IN OBJECT POOL");
            return null;
        }

        GameObject returnObject = poolDictionary[tag].Dequeue();

        if (returnObject.activeInHierarchy)
        {
            Debug.LogError("object pool overflow: " + tag);
        }

        returnObject.SetActive(true);
        returnObject.transform.position = position;
        returnObject.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(returnObject);
        if (returnObject == null)
        {
            Debug.LogError("ERROR: OBJECT RETURNED BY POOL IS NULL");
        }
        return returnObject;
    }
}
