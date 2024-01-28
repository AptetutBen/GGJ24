using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectCrab : PlayerAffect
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

    public PlayerAffectCrab(List<PlayerAffectSystem.AffectName> affects): base(affects){
        // dash speed up
        // dash duration up
    }
}
