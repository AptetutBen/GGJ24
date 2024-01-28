using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectMud : PlayerAffect
{
    public PlayerAffectMud(List<PlayerAffectSystem.AffectName> affects): base(affects){
        moveSpeed = -4f;
        airMoveSpeed = -4f;
        // dash speed down
        // dash duration down
        jumpForce = -3f;
    }
}
