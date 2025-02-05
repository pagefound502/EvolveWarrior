using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BaseEnemy
{


    public float maxHealthState = 100f;
    public float armorState = 5f;
    public float moveSpeedState = 3f;
    public float attackDamageState = 5f;
    public float attackRangeState = 2f;
    public float attackSpeedState = 1f;
    public float expRewardState = 50f;

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
}
public class EnemyController : MonoBehaviour,IBaseEnemy
{
    public Animator _anim;
    public Transform player;
    public BaseEnemy currentEnemyInfo = new BaseEnemy();

    //[Header("Enemy StatsDefault")]





    public float currentHealth;
    public float armor;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float expReward;


    private bool isDead = false;
    private float nextAttackTime = 0f;
    Rigidbody2D rb; // Rigidbody referansý


    public enum EnemyState
    {
        Create,
        Spawn,
        DeSpawn,
        Die,
        Stun,
        Knockback,
        Idle,
        Follow,
        Attack
    }

    public EnemyState enemyStateCurrent;


    void FixedUpdate()
    {

        if (enemyStateCurrent==EnemyState.DeSpawn) return;

        switch (enemyStateCurrent)
        {
            case EnemyState.Create:
                Console.WriteLine("Düþam statlarý yeniden oluþturuldu ama dünyaya eklenmedi");
                return;
                break;

            case EnemyState.Spawn:
                Console.WriteLine("Düþman spawnlandý");
                break;
            case EnemyState.DeSpawn:
                DeSpawn();
                break;
            case EnemyState.Die:
                Die();
                break;
            case EnemyState.Stun:
                // Stun animasyonu oynatýlýyor, süre sonunda baþka duruma geçecek.
                break;

            case EnemyState.Knockback:
                // Knockback kuvveti uygulanýyor.
                break;
            case EnemyState.Idle:
                
                break;
            case EnemyState.Follow:
                FollowPlayer(PlayerDirection());
                break;

            case EnemyState.Attack:
                TryAttack();
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); 
    }

    
    public Vector2 PlayerDirection()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        return direction;
    }

    public void Create(BaseEnemy baseEnemy)
    {
        currentEnemyInfo = baseEnemy;
        /*
        currentEnemyInfo.maxHealthState = maxHealthState;
        currentEnemyInfo.armorState = armorState;
        currentEnemyInfo.moveSpeedState = moveSpeedState;
        currentEnemyInfo.attackDamageState = attackDamageState;
        currentEnemyInfo.attackRangeState = attackRangeState;
        currentEnemyInfo.attackSpeedState = attackSpeedState;
        currentEnemyInfo.expRewardState = expRewardState;
        */
    }

    public void Spawn(Vector2 spawnPosition)
    {
         currentHealth= currentEnemyInfo.maxHealthState;
         armor= currentEnemyInfo.armorState;
         moveSpeed= currentEnemyInfo.moveSpeedState;
         attackDamage= currentEnemyInfo.attackDamageState;
         attackRange= currentEnemyInfo.attackRangeState;
         attackSpeed= currentEnemyInfo.attackSpeedState;
         expReward= currentEnemyInfo.expRewardState;
         transform.position =spawnPosition;
    }

    public void DeSpawn()
    {
        throw new System.NotImplementedException();
    }

    public void AwardExp()
    {
        throw new System.NotImplementedException();
    }

    public void KnowBack()
    {
        throw new System.NotImplementedException();
    }

    public void Walk()
    {
        rb.velocity = PlayerDirection().normalized * moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        throw new System.NotImplementedException();
    }
    public void CalculateHealth(float value, bool isHeal = false)
    {
        throw new System.NotImplementedException();
    }
    public void TryAttack()
    {

    }
    public void AttackDamage()
    {
        throw new System.NotImplementedException();
    }

    public void Die()
    {
        throw new System.NotImplementedException();
    }

    public float PlayerDistance(Vector2 playerPos)
    {
       return Vector2.Distance(playerPos, transform.position);
    }

    public void FollowPlayer(Vector2 targetPosNormal)
    {
        rb.velocity = targetPosNormal * moveSpeed;
        _anim.SetBool("isMoving", true);
    }



}
