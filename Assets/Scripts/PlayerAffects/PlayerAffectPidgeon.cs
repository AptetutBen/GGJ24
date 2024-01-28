using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectPidgeon : PlayerAffect
{
    public PlayerAffectPidgeon(List<PlayerAffectSystem.AffectName> affects): base(affects){
        gravityMultiplier = -0.3f;
        airMoveSpeed = 3f;
        jumpForce = 3f;
    }
}
