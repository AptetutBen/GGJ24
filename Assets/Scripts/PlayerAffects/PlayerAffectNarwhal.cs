using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectNarwhal : PlayerAffect
{
    override public float _moveSpeed
    {
        get
        {
            if (activeAffects.Contains(PlayerAffectSystem.AffectName.Mud))
                return 4f;
            else
                return 0f;
        }
    }

    public PlayerAffectNarwhal(List<PlayerAffectSystem.AffectName> affects): base(affects){
        dashSpeed = 18f;
        dashTime = -0.05f;
    }
}
