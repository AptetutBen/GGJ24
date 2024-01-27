using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffect 
{
    public List<PlayerAffectSystem.AffectName> activeAffects;
    protected float moveSpeed;
    virtual public float _moveSpeed { get { return moveSpeed; } }

    protected float jumpForce;
    virtual public float _jumpForce { get { return jumpForce; } }
    
    protected float gravity;
    virtual public float _gravity { get { return gravity; } }
    

    // Not implemented
    protected float dashDistance;
    virtual public float _dashDistance { get { return dashDistance; } }
    protected float dashSpeed;
    virtual public float _dashSpeed { get { return dashSpeed; } }
    protected float airMoveSpeed;
    virtual public float _airMoveSpeed { get { return airMoveSpeed; } }
    protected int jumpCount;
    virtual public int _jumpCount { get { return jumpCount; } }
    protected float jumpPadForce;
    virtual public float _jumpPadForce { get { return jumpPadForce; } }
    // end not implemented

    public float moveSpeedMultiplier;
    virtual public float _moveSpeedMultiplier { get { return moveSpeedMultiplier; } }
    public float jumpForceMultiplier;
    virtual public float _jumpForceMultiplier { get { return jumpForceMultiplier; } }
    public float gravityMultiplier;
    virtual public float _gravityMultiplier { get { return gravityMultiplier; } }
    protected float dashDistanceMultiplier;
    virtual public float _dashDistanceMultiplier { get { return dashDistanceMultiplier; } }
    protected float dashSpeedMultiplier;
    virtual public float _dashSpeedMultiplier { get { return dashSpeedMultiplier; } }
    protected float airMoveSpeedMultiplier;
    virtual public float _airMoveSpeedMultiplier { get { return airMoveSpeedMultiplier; } }
    public int jumpCountMultiplier;
    virtual public int _jumpCountMultiplier { get { return jumpCountMultiplier; } }
    protected float jumpPadForceMultiplier;
    virtual public float _jumpPadForceMultiplier { get { return jumpPadForceMultiplier; } }

    public PlayerAffect(List<PlayerAffectSystem.AffectName> affects){
        activeAffects = affects;

        moveSpeed       = 0;
        jumpForce       = 0;
        gravity         = 0;
        dashDistance    = 0;
        dashSpeed       = 0;
        airMoveSpeed    = 0;
        jumpCount       = 0;
        jumpPadForce    = 0;
        
        moveSpeedMultiplier     = 0;
        jumpForceMultiplier     = 0;
        gravityMultiplier       = 0;
        dashDistanceMultiplier  = 0;
        dashSpeedMultiplier     = 0;
        airMoveSpeedMultiplier  = 0;
        jumpCountMultiplier     = 0;
        jumpPadForceMultiplier  = 0;
    }
}
