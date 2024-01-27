using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffect 
{
    List<PlayerAffect> activeAffects;
    public float moveSpeed;
    public float jumpForce;
    public float gravity;
    public float dashDistance;
    public float dashSpeed;
    public float airMoveSpeed;
    public int jumpCount;

    public float moveSpeedMultiplier;
    public float jumpForceMultiplier;
    public float gravityMultiplier;
    public int jumpCountMultiplier;

    public PlayerAffect(List<PlayerAffect> affects){
        activeAffects = affects;
        
        moveSpeed = 0;
        jumpForce = 0;
        gravity = 0;
        jumpCount = 0;
        
        moveSpeedMultiplier = 0;
        jumpForceMultiplier = 0;
        gravityMultiplier = 0;
        jumpCountMultiplier = 0;
    }
}
