using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectMud : PlayerAffect
{
    public PlayerAffectMud(List<PlayerAffectSystem.AffectName> affects): base(affects){
        this.moveSpeed = -2;
    }
}
