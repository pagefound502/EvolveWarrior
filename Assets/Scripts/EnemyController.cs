using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.Rendering.DebugUI;


public class BaseEnemy
{
    public float maxHealthState ;
    public float armorState ;
    public float moveSpeedState ;
    public float attackDamageState ;
    public float attackRangeState ;
    public float attackSpeedState ;
    public float expRewardState ;

    public BaseEnemy(float maxHealthState, float armorState, float moveSpeedState, float attackDamageState, float attackRangeState, float attackSpeedState, float expRewardState)
    {
        this.maxHealthState = maxHealthState;
        this.armorState = armorState;
        this.moveSpeedState = moveSpeedState;
        this.attackDamageState = attackDamageState;
        this.attackRangeState = attackRangeState;
        this.attackSpeedState = attackSpeedState;
        this.expRewardState = expRewardState;
    }
    public BaseEnemy()
    {

    }
    public override string ToString()
    {
        return $"maxHealthState={maxHealthState}, armorState={armorState}, moveSpeedState={moveSpeedState}, " +
               $"attackDamageState={attackDamageState}, attackRangeState={attackRangeState}, " +
               $"attackSpeedState={attackSpeedState}, expRewardState={expRewardState}";
    }
}



public class EnemyController : EnemyPublicController
{


}
