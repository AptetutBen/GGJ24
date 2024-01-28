using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAffectLilGuy : PlayerAffect
{
    public PlayerAffectLilGuy(List<PlayerAffectSystem.AffectName> affects): base(affects){
        jumpCount = 1;
        // dash duration up a little
    }
}
