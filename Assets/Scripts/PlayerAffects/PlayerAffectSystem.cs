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

    
    private List<PlayerAffect> affects = new List<PlayerAffect>();

    private Dictionary<AffectName, PlayerAffect> nameToAffect;

    private Dictionary<string, PlayerAffect> IDToAffect;

    public PlayerAffectSystem(){
        nameToAffect = new Dictionary<AffectName, PlayerAffect>(){
            [AffectName.Default] = new PlayerAffectDefault(affects)
        };

        IDToAffect = new Dictionary<string, PlayerAffect>(){
            ["default"] = nameToAffect[AffectName.Default]
        };

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

    float gravityCache = 0;
    float gravityCacheMultiplier = 0;
    public float GetGravity(){
        gravityCache = 0;
        gravityCacheMultiplier = 0;

        for (int i = 0; i < affects.Count; i++)
        {
            gravityCache += affects[i].gravity;
            gravityCacheMultiplier += affects[i].gravityMultiplier;
        }

        return gravityCache * gravityCacheMultiplier;
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
