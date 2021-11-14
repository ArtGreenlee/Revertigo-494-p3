using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpawnView))]
public class EntityPhysicsInteractions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision coll)
    {
        GetComponent<SpawnView>().view_instance.GetComponent<AestheticEffects>().Bump(coll.relativeVelocity.magnitude * 0.1f);

        ScreenShakeController.RequestScreenShake(coll.relativeVelocity.magnitude * 0.05f, 0.1f);

        ParticleManager.RequestBurst(coll.contacts[0].point);
    }
}
