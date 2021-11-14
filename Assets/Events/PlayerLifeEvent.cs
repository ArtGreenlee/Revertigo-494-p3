using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeEvent
{

    public int delta_life = 0;
    public PlayerLifeEvent(int _delta_life) {delta_life = _delta_life;}
    
    public override string ToString()
    {
        return "delta_life : " + delta_life;
    }
}
