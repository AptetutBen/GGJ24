using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectFox : PlayerAffect
{
    override public float _moveSpeed
    {
        get
        {
            if (activeAffects.Contains(PlayerAffectSystem.AffectName.Bushes))
                return 6f;
            else
                return 4f;
        }
    }

    public PlayerAffectFox(List<PlayerAffectSystem.AffectName> affects): base(affects){
        jumpForce = 5f;
        gravityMultiplier = 0.4f;
    }



}
