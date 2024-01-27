using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectDefault : PlayerAffect
{
    public PlayerAffectDefault(List<PlayerAffectSystem.AffectName> affects): base(affects){
        this.moveSpeed = 3;
        this.jumpForce = 5;
        this.gravity   = 0;
        this.jumpCount = 1; 
    }
}
