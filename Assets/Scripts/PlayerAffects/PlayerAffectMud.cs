using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectMud : PlayerAffect
{
    public PlayerAffectMud(List<PlayerAffectSystem.AffectName> affects): base(affects){
        moveSpeed = -4f;
        airMoveSpeed = -4f;
        dashSpeed = -10f;
        jumpForce = -13f;
    }
}
