using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerAffectSystem
{
    public enum AffectName{
        Default,
        
        // Clothes
        Duck,
        Dog,
        Dino,
        Fox,
        Wombat,
        Cat,
        Crab,
        LilGuy,
        Pidgeon,
        Narwhal,

        // Environment
        Mud,
        Bushes,
        Sticky,
        Ice
    }
    
    private List<PlayerAffect> affects = new List<PlayerAffect>();
    private List<AffectName> affectNames = new List<AffectName>();

    private Dictionary<AffectName, PlayerAffect> nameToAffect;

    private Dictionary<string, PlayerAffect> IDToAffect;

    public PlayerAffectSystem(){
        nameToAffect = new Dictionary<AffectName, PlayerAffect>(){
            [AffectName.Default] = new PlayerAffectDefault(affectNames),
            
            // Clothes
            [AffectName.Duck] = new PlayerAffectDuck(affectNames),
            [AffectName.Dog] = new PlayerAffectDog(affectNames),
            [AffectName.Dino] = new PlayerAffectDino(affectNames),
            [AffectName.Fox] = new PlayerAffectFox(affectNames),
            [AffectName.Wombat] = new PlayerAffectWombat(affectNames),
            [AffectName.Cat] = new PlayerAffectCat(affectNames),
            [AffectName.Crab] = new PlayerAffectCrab(affectNames),
            [AffectName.LilGuy] = new PlayerAffectLilGuy(affectNames),
            [AffectName.Pidgeon] = new PlayerAffectPidgeon(affectNames),
            [AffectName.Narwhal] = new PlayerAffectNarwhal(affectNames),

            // Environment
            [AffectName.Mud] = new PlayerAffectMud(affectNames),
            [AffectName.Bushes] = new PlayerAffectBushes(affectNames),
            [AffectName.Sticky] = new PlayerAffectSticky(affectNames),
            [AffectName.Ice] = new PlayerAffectIce(affectNames),
        };

        IDToAffect = new Dictionary<string, PlayerAffect>(){
            ["default"] = nameToAffect[AffectName.Default],

            // Clothes
            ["duck"] = nameToAffect[AffectName.Duck],
            ["dog"] = nameToAffect[AffectName.Dog],
            ["dino"] = nameToAffect[AffectName.Dino],
            ["fox"] = nameToAffect[AffectName.Fox],
            ["wombat"] = nameToAffect[AffectName.Wombat],
            ["cat"] = nameToAffect[AffectName.Cat],
            ["crab"] = nameToAffect[AffectName.Crab],
            ["lilguy"] = nameToAffect[AffectName.LilGuy],
            ["pidgeon"] = nameToAffect[AffectName.Pidgeon],
            ["narwhal"] = nameToAffect[AffectName.Narwhal],

            // Environment
            ["mud"] = nameToAffect[AffectName.Mud],
            ["bushes"] = nameToAffect[AffectName.Bushes],
            ["sticky"] = nameToAffect[AffectName.Sticky],
            ["ice"] = nameToAffect[AffectName.Ice],
        };

        AddAffect(AffectName.Default);
    }

    // Move Speed
    float? moveSpeedCache = null;
    public float GetMoveSpeed(){

        if(moveSpeedCache == null)
        {
            moveSpeedCache = 0;
            float moveSpeedMultiplier = 0;
            for (int i = 0; i < affects.Count; i++)
            {
                moveSpeedCache += affects[i]._moveSpeed;
                moveSpeedMultiplier += affects[i]._moveSpeedMultiplier;
            }

            moveSpeedCache *= moveSpeedMultiplier;
        }

        return Mathf.Max((float)moveSpeedCache, 0.1f);
    }

    // Jump Force
    float? jumpForceCache = null;
    public float GetJumpForce(){
       
        if (jumpForceCache == null)
        {
            jumpForceCache = 0;
            float jumpForceMultiplier= 0;

            for (int i = 0; i < affects.Count; i++)
            {
                jumpForceCache += affects[i]._jumpForce;
                jumpForceMultiplier += affects[i]._jumpForceMultiplier;
            }

            jumpForceCache *= jumpForceMultiplier;
        }
        return Mathf.Max((float)jumpForceCache, 0);
    }


    // Gravity
    float? gravityCache = null;
    public float GetGravity(){
        if(gravityCache == null){
            gravityCache = 0;
            float gravityCacheMultiplier = 0;

            for (int i = 0; i < affects.Count; i++)
            {
                gravityCache += affects[i]._gravity;
                gravityCacheMultiplier += affects[i].gravityMultiplier;
            }

            gravityCache = gravityCache * gravityCacheMultiplier;
        }

        return (float) gravityCache;
    }

    // AirControl
    float? airMoveSpeedCache = null;
    public float GetAirMoveSpeed()
    {
        if (airMoveSpeedCache == null)
        {
            airMoveSpeedCache = 0;
            float airMoveSpeedMultiplier = 0;

            for (int i = 0; i < affects.Count; i++)
            {
                airMoveSpeedCache += affects[i]._airMoveSpeed;
                airMoveSpeedMultiplier += affects[i]._airMoveSpeedMultiplier;
            }

            airMoveSpeedCache *= airMoveSpeedMultiplier;
        }

        return (float)airMoveSpeedCache;
    }


    // Jump count
    int? jumpCountCache = null;
    public int GetJumpCount(){
        if(jumpCountCache == null)
        {
            jumpCountCache = 0;
            for (int i = 0; i < affects.Count; i++)
            {
                jumpCountCache += affects[i]._jumpCount;
            }
        }

        return Mathf.Max((int)jumpCountCache, 0);
    }

    public void AddAffect(AffectName affect){
        affects.Add(nameToAffect[affect]);
        affectNames.Add(affect);

        NullCaches();
    }

    public void RemoveAffect(AffectName affect){
        if(affects.Contains(nameToAffect[affect])){
            affects.Remove(nameToAffect[affect]);
            affectNames.Remove(affect);
        }

        NullCaches();
    }

    public void NullCaches()
    {
        jumpCountCache = null;
        airMoveSpeedCache = null;
        gravityCache = null;
        jumpForceCache = null;
        moveSpeedCache = null;
    }
}
