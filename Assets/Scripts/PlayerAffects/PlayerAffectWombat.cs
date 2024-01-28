using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectWombat : PlayerAffect
{
    override public float _moveSpeed
    {
        get
        {
            if (activeAffects.Contains(PlayerAffectSystem.AffectName.Bushes))
                return 4f;
            else
                return 0f;
        }
    }

    public PlayerAffectWombat(List<PlayerAffectSystem.AffectName> affects): base(affects){
        jumpPadForce = -3f;
        jumpForce = -2;
    }
}
