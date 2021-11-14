using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLifeUI : MonoBehaviour
{

    Subscription<PlayerLifeEvent> player_life_event_subscription;

    // Start is called before the first frame update
    void Start()
    {
        player_life_event_subscription = EventBus.Subscribe<PlayerLifeEvent>(_OnPlayerLifeUpdated);
    }

    void _OnPlayerLifeUpdated(PlayerLifeEvent e){
        Debug.Log("Player life updated: " + e.delta_life);
    }

    
    private void OnDestroy()
    {
        EventBus.Unsubscribe(player_life_event_subscription);
    }
}
