using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectDuck : PlayerAffect
{
    override public float _moveSpeed
    {
        get {
            if(activeAffects.Contains(PlayerAffectSystem.AffectName.Mud))
                return 4f;
            else
                return 0f;
        }
    }

    public PlayerAffectDuck(List<PlayerAffectSystem.AffectName> affects): base(affects){
        airMoveSpeed = 2f;
        gravityMultiplier = -0.2f;
    }

}
