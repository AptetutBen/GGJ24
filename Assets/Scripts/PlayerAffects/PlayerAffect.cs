using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffect 
{
    public float moveSpeed;
    public float jumpForce;
    public float gravity;
    public int jumpCount;

    public PlayerAffect(){
        moveSpeed = 0;
        jumpForce = 0;
        gravity = 0;
        jumpCount = 0;
    }
}
