using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnView : MonoBehaviour {

    public GameObject view_prefab;
    public GameObject view_instance;

	// Use this for initialization
	void Start () {
        GameObject new_view = GameObject.Instantiate(view_prefab);
        new_view.GetComponent<FollowTarget>().target = transform;
        view_instance = new_view;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, transform.localScale.x * 0.5f);
    }
}
