using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectDuck : PlayerAffect
{
    override public float _moveSpeed
    {
        get {
            if(activeAffects.Contains(PlayerAffectSystem.AffectName.Mud))
                return 1;
            else
                return 0;
        }
    }

    public PlayerAffectDuck(List<PlayerAffectSystem.AffectName> affects): base(affects){
        
    }

}
