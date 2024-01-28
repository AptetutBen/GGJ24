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

    private Dictionary<string, AffectName> IDToAffect;

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

        IDToAffect = new Dictionary<string, AffectName>(){
            ["default"]         = AffectName.Default,

            // Clothes
            ["duck_hat"]        = AffectName.Duck,
            ["duck_top"]        = AffectName.Duck,
            ["dog_hat"]         = AffectName.Dog,
            ["dog_top"]         = AffectName.Dog,
            ["fox_hat"]         = AffectName.Fox,
            ["fox_top"]         = AffectName.Fox,
            ["wombat_hat"]      = AffectName.Wombat,
            ["wombat_top"]      = AffectName.Wombat,
            ["crab_hat"]        = AffectName.Crab,
            ["crab_top"]        = AffectName.Crab,
            ["lilguy_hat"]      = AffectName.LilGuy,
            ["lilguy_top"]      = AffectName.LilGuy,
            ["pidgeon_hat"]     = AffectName.Pidgeon,
            ["pidgeon_top"]     = AffectName.Pidgeon,
            ["narwhal_hat"]     = AffectName.Narwhal,
            ["narwhal_top"]     = AffectName.Narwhal,

            // Environment
            ["mud"]             = AffectName.Mud,
            ["bushes"]          = AffectName.Bushes,
            ["sticky"]          = AffectName.Sticky,
            ["ice"]             = AffectName.Ice,
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

    // jumpPadForce
    float? jumpPadForceCache = null;
    public float GetjumpPadForce(){
        if(jumpPadForceCache == null){
            jumpPadForceCache = 0;
            float jumpPadForceCacheMultiplier = 0;

            for (int i = 0; i < affects.Count; i++)
            {
                jumpPadForceCache += affects[i]._jumpPadForce;
                jumpPadForceCacheMultiplier += affects[i]._jumpPadForceMultiplier;
            }

            jumpPadForceCache = jumpPadForceCache * jumpPadForceCacheMultiplier;
        }

        return (float) jumpPadForceCache;
    }
    
    // DashDuration
    float? dashDurationCache = null;
    public float GetDashDuration(){
        if(dashDurationCache == null){
            dashDurationCache = 0;
            float dashDurationCacheMultiplier = 0;

            for (int i = 0; i < affects.Count; i++)
            {
                dashDurationCache += affects[i]._dashTime;
                dashDurationCacheMultiplier += affects[i]._dashTimeMultiplier;
            }

            dashDurationCache = dashDurationCache * dashDurationCacheMultiplier;
        }

        return (float) dashDurationCache;
    }
    
    // DashSpped
    float? dashSpeedCache = null;
    public float GetDashSpeed(){
        if(dashSpeedCache == null){
            dashSpeedCache = 0;
            float dashSpeedCacheMultiplier = 0;

            for (int i = 0; i < affects.Count; i++)
            {
                dashSpeedCache += affects[i]._dashSpeed;
                dashSpeedCacheMultiplier += affects[i]._dashSpeedMultiplier;
            }

            dashSpeedCache = dashSpeedCache * dashSpeedCacheMultiplier;
        }

        return (float) dashSpeedCache;
    }

    // DashCooldown
    float? dashCooldownCache = null;
    public float GetDashCooldown(){
        if(dashCooldownCache == null){
            dashCooldownCache = 0;
            float dashCooldownCacheMultiplier = 0;

            for (int i = 0; i < affects.Count; i++)
            {
                dashCooldownCache += affects[i]._dashCooldown;
                dashCooldownCacheMultiplier += affects[i]._dashCooldownMultiplier;
            }

            dashCooldownCache = dashCooldownCache * dashCooldownCacheMultiplier;
        }

        return (float) dashCooldownCache;
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

    public void AddAffect(string affectName){
        if(IDToAffect.ContainsKey(affectName)){
            AddAffect(IDToAffect[affectName]);
        }
    }
    public void AddAffect(AffectName affect){
        WeekendLogger.Log($"AddAffect {affect.ToString()}");
        affects.Add(nameToAffect[affect]);
        affectNames.Add(affect);

        NullCaches();
    }

    public void RemoveAffect(string affectName){
        if(IDToAffect.ContainsKey(affectName)){
            RemoveAffect(IDToAffect[affectName]);
        }
    }
    public void RemoveAffect(AffectName affect){
        WeekendLogger.Log($"RemoveAffect {affect.ToString()}");
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
        jumpPadForceCache = null;
    }
}
