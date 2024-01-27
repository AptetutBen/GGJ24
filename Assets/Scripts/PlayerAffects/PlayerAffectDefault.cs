using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectDefault : PlayerAffect
{
    public PlayerAffectDefault(List<PlayerAffectSystem.AffectName> affects): base(affects){
        moveSpeed       = 10;
        jumpForce       = 6;
        gravity         = 0;
        dashTime        = 0;
        dashSpeed       = 0;
        airMoveSpeed    = 5f;
        jumpCount       = 1;
        jumpPadForce    = 0;
        
        moveSpeedMultiplier     = 1;
        jumpForceMultiplier     = 1;
        gravityMultiplier       = 1;
        dashTimeMultiplier      = 1;
        dashSpeedMultiplier     = 1;
        airMoveSpeedMultiplier  = 1;
        jumpCountMultiplier     = 1;
        jumpPadForceMultiplier  = 1;
    }
}
