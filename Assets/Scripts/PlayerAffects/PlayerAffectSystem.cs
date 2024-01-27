using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAffectSystem
{

    public enum AffectName{
        Default
    }

    private Dictionary<AffectName, PlayerAffect> nameToAffect = new Dictionary<AffectName, PlayerAffect>(){
        [AffectName.Default] = new PlayerAffectDefault()
    };
    
    private List<PlayerAffect> affects = new List<PlayerAffect>();

    public PlayerAffectSystem(){
        AddAffect(AffectName.Default);
    }

    public float GetMoveSpeed(){
        float moveSpeed = 0;

        for (int i = 0; i < affects.Count; i++)
        {
            moveSpeed += affects[i].moveSpeed;
        }

        return Mathf.Max(moveSpeed, 0);
    }

    public float GetJumpForce(){
        float jumpForce = 0;

        for (int i = 0; i < affects.Count; i++)
        {
            jumpForce += affects[i].jumpForce;
        }

        return Mathf.Max(jumpForce, 0);
    }

    public float GetGravity(){
        float gravity = 0;

        for (int i = 0; i < affects.Count; i++)
        {
            gravity += affects[i].gravity;
        }

        return Mathf.Max(gravity, 0);
    }

    public int GetjumpCount(){
        int jumpCount = 0;

        for (int i = 0; i < affects.Count; i++)
        {
            jumpCount += affects[i].jumpCount;
        }

        return Mathf.Max(jumpCount, 0);
    }

    public void AddAffect(AffectName affect){
        affects.Add(nameToAffect[affect]);
    }

    public void RemoveAffect(AffectName affect){
        if(affects.Contains(nameToAffect[affect])){
            affects.Remove(nameToAffect[affect]);
        }
    }
}
