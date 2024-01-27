using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectDefault : PlayerAffect
{
    public PlayerAffectDefault(List<PlayerAffectSystem.AffectName> affects): base(affects){
        moveSpeed       = 3;
        jumpForce       = 5;
        gravity         = 0;
        dashTime        = 0;
        dashSpeed       = 0;
        airMoveSpeed    = 0;
        jumpCount       = 1;
        jumpPadForce    = 0;
        
        moveSpeedMultiplier     = 0;
        jumpForceMultiplier     = 0;
        gravityMultiplier       = 0;
        dashTimeMultiplier      = 0;
        dashSpeedMultiplier     = 0;
        airMoveSpeedMultiplier  = 0;
        jumpCountMultiplier     = 0;
        jumpPadForceMultiplier  = 0;
    }
}
