using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectDog : PlayerAffect
{
    public PlayerAffectDog(List<PlayerAffectSystem.AffectName> affects): base(affects){
        moveSpeed = 8f;
        //dash duration up
    }

}
