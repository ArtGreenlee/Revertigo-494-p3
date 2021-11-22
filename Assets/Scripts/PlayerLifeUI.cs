using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeUI : MonoBehaviour
{

    Subscription<PlayerLifeEvent> player_life_event_subscription;

    public float k = 0.3f;
    public float amplitude = .01f;
    public float dampening_factor = 0.95f;
    Vector3 velocity = Vector3.zero;
    Vector3 origPosition;

    // Start is called before the first frame update
    void Start()
    {
        player_life_event_subscription = EventBus.Subscribe<PlayerLifeEvent>(_OnPlayerLifeUpdated);
        origPosition = transform.localPosition;
    }

    void Update(){
        Vector3 displacement = origPosition - transform.localPosition;
        Vector3 acceleration = k * displacement;
        velocity += acceleration;
        velocity *= dampening_factor;

        transform.localPosition += velocity;
    }

    void _OnPlayerLifeUpdated(PlayerLifeEvent e){
        Debug.Log("Player life updated: " + e.delta_life);
        if(e.delta_life < 0){
            float randX = UnityEngine.Random.Range(-.5f, .5f) * Mathf.Abs(e.delta_life / 4f) * .000001f;
            Vector3 randStart = new Vector3(randX, origPosition.y, origPosition.z);
            transform.localPosition = randStart; 
        }
        
    }

    
    private void OnDestroy()
    {
        EventBus.Unsubscribe(player_life_event_subscription);
    }
}
