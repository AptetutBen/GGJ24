using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectCrab : PlayerAffect
{
    public PlayerAffectCrab(List<PlayerAffectSystem.AffectName> affects): base(affects){
        this.moveSpeed = -2;
    }
}
