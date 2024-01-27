using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAffectSystem
{
    private Dictionary<string, PlayerAffect> stringToAffect = new Dictionary<string, PlayerAffect>(){
        ["default"] = new PlayerAffectDefault()
    };
    
    private List<PlayerAffect> affects = new List<PlayerAffect>();

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

    public void AddAffect(string affectName){
        if(stringToAffect.ContainsKey(affectName)){
            AddAffect(stringToAffect[affectName]);
        }
    }

    public void RemoveAffect(string affectName){
        if(stringToAffect.ContainsKey(affectName)){
            RemoveAffect(stringToAffect[affectName]);
        }
    }

    public void AddAffect(PlayerAffect affect){
        affects.Add(affect);
    }

    public void RemoveAffect(PlayerAffect affect){
        if(affects.Contains(affect)){
            affects.Remove(affect);
        }
    }
}
