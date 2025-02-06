using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
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
    public BaseEnemyStatsInfo enemyStatsScriptableObject;

    public Animator _anim;
    public Transform playerTransform;
    public BaseEnemy currentEnemyInfo = new BaseEnemy();

    //[Header("Enemy StatsDefault")]


    public float currentHealth;
    public bool currentStun = false;
    public float armor;
    public float moveSpeed;
    public float attackDamage;
    public float attackRange;
    public float attackSpeed;
    public float expReward;

    private bool attackAviable = true;
    private bool isDead = false;
    private float nextAttackTime = 0f;
    public Rigidbody2D rb; // Rigidbody referansý


    public enum EnemyState
    {
        Empty,
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

    void Start()
    {
        //First value initialize
       
    }

    void FixedUpdate()
    {

        if ((enemyStateCurrent==EnemyState.Empty)) return;
        switch (enemyStateCurrent)
        {
            case EnemyState.Create:
                Console.WriteLine("Düþam statlarý yeniden oluþturuldu ama dünyaya eklenmedi");
                Create(enemyStatsScriptableObject);
                gameObject.SetActive(false);
                enemyStateCurrent = EnemyState.Empty;
                break;

            case EnemyState.Spawn:
                Console.WriteLine("Düþman spawnlandý");
                break;
            case EnemyState.DeSpawn:
                DeSpawn();
                break;
            case EnemyState.Die:
                if (isDead == true)
                {
                    enemyStateCurrent = EnemyState.DeSpawn;
                }
                break;

            case EnemyState.Stun:
                if (currentStun)
                {
                    break;
                }
                else
                {
                    enemyStateCurrent = EnemyState.Idle;
                }
                break;

            case EnemyState.Knockback:
                // Knockback kuvveti uygulanýyor.
                break;
            case EnemyState.Idle:
                //yaþýyorsa takip et
                if(!GameManagerCustom.playerIsDeath)
                {
                    enemyStateCurrent=EnemyState.Follow;
                    break;
                }
                break;
            case EnemyState.Follow:
                //yaþamýyorsa dur
                if (GameManagerCustom.playerIsDeath)
                {
                    enemyStateCurrent = EnemyState.Idle;
                    break;
                }
                if (PlayerDistance(playerTransform.position) > attackRange)
                {
                    FollowPlayer(PlayerDirection());
                }
                else
                {
                    enemyStateCurrent = EnemyState.Attack;
                }


                break;

            case EnemyState.Attack:
                //yaþamýyorsa saldýrdma
                if (GameManagerCustom.playerIsDeath)
                {
                    enemyStateCurrent = EnemyState.Idle;
                    break;
                }
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
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        return direction;
    }

    public void Create(BaseEnemyStatsInfo enemyScriptableObject)
    {
        currentEnemyInfo = new BaseEnemy(enemyStatsScriptableObject.maxHealthState, enemyStatsScriptableObject.armorState, enemyStatsScriptableObject.moveSpeedState, enemyStatsScriptableObject.attackDamageState, enemyStatsScriptableObject.attackRangeState, enemyStatsScriptableObject.attackSpeedState, enemyStatsScriptableObject.expRewardState);
        //rb=gameObject.GetComponent<Rigidbody2D>();
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
        playerTransform = GameManagerCustom.playerController.transform;
         gameObject.SetActive(true);
         isDead= false;
         enemyStateCurrent=EnemyState.Idle;
         ShowHp();
    }

    public void DeSpawn()
    {
        enemyStateCurrent= enemyStateCurrent = EnemyState.Empty;
        gameObject.SetActive(false);
    }

    public void AwardExp()
    {
        GameManagerCustom.playerController.GainExperience(expReward);
    }

    public void KnowBack()
    {
        throw new System.NotImplementedException();
    }

    public void Stun()
    {
        if (enemyStateCurrent == EnemyState.Die) return;
        StartCoroutine(StunEffect(3));
    }

    public void Walk()
    {
        rb.velocity = PlayerDirection().normalized * moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        if (enemyStateCurrent == EnemyState.Die) return;
        if(currentStun)
        {
            damage = damage * 2;
        }
        CalculateHealth(damage);

    }
    public void CalculateHealth(float value, bool isHeal = false)
    {
        if(isHeal)
        {
            value = value * -1;
        }
        currentHealth = Mathf.Clamp(currentHealth - value, 0, currentEnemyInfo.maxHealthState);
        if(currentHealth<=0)
        {
            Die();
        }
    }
    public void TryAttack()
    {
        if(PlayerDistance(playerTransform.position)<attackRange)
        {
            rb.velocity = Vector2.zero;
            AttackDamage();
            return;
        }
        else
        {
            enemyStateCurrent = EnemyState.Follow;
        }
    }
    public void AttackDamage()
    {
        if(attackAviable)
        {
            StartCoroutine(AttackEffect(1));
        }
        else
        {
            rb.velocity = Vector2.zero;
            _anim.SetFloat("RunState", 0);
        }
    }


    public void Die()
    {
        if (isDead==false)
        { 
            StartCoroutine(DieEffect(1.5f));
        }
    }

    public float PlayerDistance(Vector2 playerPos)
    {
       return Vector2.Distance(playerPos, transform.position);
    }

    public void FollowPlayer(Vector2 targetPosNormal)
    {
        rb.velocity = targetPosNormal * moveSpeed;
        _anim.SetFloat("RunState", 0.5f);
    }

    public void ShowHp()
    {
        Console.WriteLine(currentHealth);
    }


    //
    public IEnumerator StunEffect(float duration)
    {
        enemyStateCurrent=(EnemyState.Stun);
        rb.velocity = Vector2.zero; // Hareketi durdur
        _anim.SetFloat("RunState", 1f); // Stun animasyonunu kapat
        currentStun = true;
        yield return new WaitForSeconds(duration); // Belirtilen süre kadar bekle

        _anim.SetFloat("RunState", 0f); // Stun animasyonunu kapat
        currentStun = false;
    }
    public IEnumerator DieEffect(float duration)
    {
        //enemyStateCurrent = (EnemyState.Stun);
        rb.velocity = Vector2.zero; // Hareketi durdur
        _anim.SetTrigger("Die"); // Stun animasyonu çalýþtýr
        isDead = true;
        yield return new WaitForSeconds(duration); // Belirtilen süre kadar bekle
        enemyStateCurrent= (EnemyState.Die);

    }
    public IEnumerator AttackEffect(float duration)
    {
        enemyStateCurrent = (EnemyState.Stun);
        _anim.SetTrigger("Attack"); // Stun animasyonunu kapat
        attackAviable = false;
        GameManagerCustom.playerController.TakeDamage(attackDamage);
        yield return new WaitForSeconds(duration); // Belirtilen süre kadar bekle
        attackAviable = true;
    }



}
