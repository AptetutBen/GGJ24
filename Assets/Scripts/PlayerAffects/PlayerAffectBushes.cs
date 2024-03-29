using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectBushes : PlayerAffect
{
    public PlayerAffectBushes(List<PlayerAffectSystem.AffectName> affects): base(affects){
        moveSpeed = -4f;
        dashTime = -1f;
        jumpForce = -3f;
    }
}
