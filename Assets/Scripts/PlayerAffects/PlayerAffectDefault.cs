using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectDefault : PlayerAffect
{
    public PlayerAffectDefault(List<PlayerAffectSystem.AffectName> affects): base(affects){
        moveSpeed       = 9.5f;
        jumpForce       = 14;
        gravity         = 0.4f;
        dashTime        = 0.2f;
        dashSpeed       = 32f;
        dashCooldown    = 0.5f;
        airMoveSpeed    = 9.5f;
        jumpCount       = 1;
        jumpPadForce    = 30;
        
        moveSpeedMultiplier     = 1;
        jumpForceMultiplier     = 1;
        gravityMultiplier       = 1;
        dashTimeMultiplier      = 1;
        dashSpeedMultiplier     = 1;
        dashCooldownMultiplier  = 1;
        airMoveSpeedMultiplier  = 1;
        jumpCountMultiplier     = 1;
        jumpPadForceMultiplier  = 1;
    }
}
