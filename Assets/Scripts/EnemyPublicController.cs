using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPublicController : MonoBehaviour,IBaseEnemy
{
    public BaseEnemyStatsInfo enemyStatsScriptableObject;

    public Animator _anim;
    public Transform playerTransform;
    public BaseEnemy currentEnemyInfo;


    //[Header("Enemy StatsDefault")]

    public Animator _HitAnim;



    protected float currentHealth;
    private bool isStun = false;
    protected float armor;
    protected float moveSpeed;
    protected float attackDamage;
    protected float attackRange;
    protected float attackSpeed;
    protected float expReward;

    private bool attackAviable = true;
    private bool isDead = false;
    private bool isKnockback = false;
    private bool isSlowEffect = false;
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



    public virtual void Start()
    {
    }

    public  virtual void FixedUpdate()
    {
        switch (enemyStateCurrent)
        {
            case EnemyState.Create:
                Debug.Log("Düþam statlarý yeniden oluþturuldu ama dünyaya eklenmedi");
                Create(enemyStatsScriptableObject);
                gameObject.SetActive(false);
                enemyStateCurrent = EnemyState.Empty;
                break;

            case EnemyState.Spawn:
                Debug.Log("Düþman spawnlandý");
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
                if (isDead) return;

                if (isStun)
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
                if (!GameManagerCustom.playerIsDeath)
                {
                    enemyStateCurrent = EnemyState.Follow;
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
                if (enemyStateCurrent == EnemyState.Stun || isDead == true) return;

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


    public virtual Vector2 PlayerDirection()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        return direction;
    }

    public virtual void Create(BaseEnemyStatsInfo enemyScriptableObject)
    {
        currentEnemyInfo = new BaseEnemy(enemyStatsScriptableObject.maxHealthState, enemyStatsScriptableObject.armorState, enemyStatsScriptableObject.moveSpeedState, enemyStatsScriptableObject.attackDamageState, enemyStatsScriptableObject.attackRangeState, enemyStatsScriptableObject.attackSpeedState, enemyStatsScriptableObject.expRewardState);
        //Debug.Log("Info"+currentEnemyInfo.ToString());
        gameObject.SetActive(false);
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

    public virtual void Spawn(Vector2 spawnPosition)
    {
        gameObject.SetActive(true);
        Debug.Log("Info" + currentEnemyInfo.ToString());
        currentHealth = currentEnemyInfo.maxHealthState;
        armor = currentEnemyInfo.armorState;
        moveSpeed = currentEnemyInfo.moveSpeedState;
        attackDamage = currentEnemyInfo.attackDamageState;
        attackRange = currentEnemyInfo.attackRangeState;
        attackSpeed = currentEnemyInfo.attackSpeedState;
        expReward = currentEnemyInfo.expRewardState;
        transform.position = spawnPosition;
        isDead = false;
        attackAviable = true;
        isStun = false;
        isKnockback = false;
        isSlowEffect = false;
        playerTransform = GameManagerCustom.playerController.transform;
        enemyStateCurrent = EnemyState.Idle;


        ShowHp();
    }

    public virtual void DeSpawn()
    {
        enemyStateCurrent = enemyStateCurrent = EnemyState.Empty;
        gameObject.SetActive(false);
    }

    public virtual void AwardExp()
    {
        GameManagerCustom.playerController.GainExperience(expReward);
    }

    public void KnockBack(Vector2 direction, float knocbackForce, float knockbackTime)
    {
        if (isDead)
        {
            return;
        }

        if (isKnockback == false)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(direction.normalized * knocbackForce, ForceMode2D.Impulse);
            StartCoroutine(KnockbackEffect(knockbackTime));
        }
    }

    public virtual void Stun()
    {
        if (isDead) return;
        if (isStun == false)
        {

            //stundayken tekrar stun atamazsýn
            StartCoroutine(StunEffect(3));
        }
    }

    public void Walk()
    {
        rb.velocity = PlayerDirection().normalized * moveSpeed;
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;
        if (isStun)
        {
            damage = damage * 2;
        }
        _HitAnim.SetTrigger("AttackEffectT");
        CalculateHealth(damage);

    }
    public virtual void CalculateHealth(float value, bool isHeal = false)
    {
        if (isHeal)
        {
            value = value * -1;
        }
        currentHealth = Mathf.Clamp(currentHealth - value, 0, currentEnemyInfo.maxHealthState);
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public virtual void TryAttack()
    {
        if (isDead)
        {
            return;
        }
        if (PlayerDistance(playerTransform.position) < attackRange)
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
    public virtual void AttackDamage()
    {
        if (attackAviable)
        {
            StartCoroutine(AttackEffect(1));
        }
        else
        {
            rb.velocity = Vector2.zero;
            _anim.SetFloat("RunState", 0);
        }
    }


    public virtual void Die()
    {
        if (isDead == false)
        {
            rb.velocity = Vector2.zero;
            StartCoroutine(DieEffect(1.5f));
        }
    }

    public virtual float PlayerDistance(Vector2 playerPos)
    {
        return Vector2.Distance(playerPos, transform.position);
    }

    public  virtual void FollowPlayer(Vector2 targetPosNormal)
    {
        rb.velocity = targetPosNormal * moveSpeed;
        _anim.SetFloat("RunState", 0.5f);
        ChasePlayer(PlayerDirection());
    }

    public  virtual void ChasePlayer(Vector2 targetPos)
    {
        if (targetPos.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            //transform.localScale = new Vector3(transform.localScale.x*-1, transform.localScale.y, transform.localScale.z);  // Saða bak
        }
        else if (targetPos.x <= 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            //transform.localScale = new Vector3(transform.localScale.x * 1, transform.localScale.y, transform.localScale.z);  // Saða bak
        }
    }

    public  virtual void ShowHp()
    {
        Console.WriteLine(currentHealth);
    }
    //


    public virtual void SlowAttack(float power, float duration)
    {
        if (!isSlowEffect && !isDead)
        {
            StartCoroutine(SlowEffect(power, duration));
        }
    }
    public virtual IEnumerator SlowEffect(float power, float duration)
    {
        if (power == 0)
        {
            power = 0.5f;
        }
        moveSpeed = Mathf.Clamp(moveSpeed / power, 0.1f, moveSpeed);

        isSlowEffect = true;
        yield return new WaitForSeconds(duration);
        moveSpeed = currentEnemyInfo.moveSpeedState;
        isSlowEffect = false;
    }


    public virtual IEnumerator DieEffect(float duration)
    {
        //enemyStateCurrent = (EnemyState.Stun);
        rb.velocity = Vector2.zero; // Hareketi durdur
        _anim.SetTrigger("Die"); // Stun animasyonu çalýþtýr
        isDead = true;
        attackAviable = false;
        yield return new WaitForSeconds(duration); // Belirtilen süre kadar bekle
        enemyStateCurrent = (EnemyState.Die);

    }
    public virtual IEnumerator AttackEffect(float duration)
    {
        enemyStateCurrent = (EnemyState.Stun);
        _anim.SetTrigger("Attack"); // Stun animasyonunu kapat
        attackAviable = false;
        GameManagerCustom.playerController.TakeDamage(attackDamage);
        yield return new WaitForSeconds(duration); // Belirtilen süre kadar bekle
        attackAviable = true;
    }

    public virtual IEnumerator KnockbackEffect(float duration)
    {
        enemyStateCurrent = (EnemyState.Knockback);
        isKnockback = true;
        _anim.SetFloat("RunState", 1f);
        yield return new WaitForSeconds(duration); // Belirtilen süre kadar bekle

        enemyStateCurrent = EnemyState.Idle;
        isKnockback = false;
    }
    public virtual IEnumerator StunEffect(float duration)
    {
        enemyStateCurrent = (EnemyState.Stun);
        rb.velocity = Vector2.zero; // Hareketi durdur
        _anim.SetFloat("RunState", 1f); // Stun animasyonunu kapat
        isStun = true;
        attackAviable = false;
        yield return new WaitForSeconds(duration); // Belirtilen süre kadar bekle
        attackAviable = true;
        isStun = false;
        _anim.SetFloat("RunState", 0f); // Stun animasyonunu kapat
    }
}
