using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class EnemyController : MonoBehaviour
{
    public Animator _anim;
    public Transform player;
    public EnemyType enemyType = EnemyType.Melee;

    [Header("Enemy Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float armor = 5f;
    public float moveSpeed = 3f;
    public float attackDamage = 5f;
    public float attackRange = 2f;
    public float attackSpeed = 1f;
    public float expReward = 50f;
    public bool isBoss = false;


    private bool isDead = false;
    private float nextAttackTime = 0f;
    Rigidbody2D rb; // Rigidbody referans�


    public bool isUsable = true;
    public enum EnemyType
    {
        Melee,
        Ranged,
        Magic,
        Fireball,
        Poison
    }

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        if (_anim == null) _anim = GetComponent<Animator>();
    }


    public enum EnemyState
    {
        Idle,
        Walk,
        Dead,
        Wait,
        Freeze,
        Stun,
        Target,
        Follow,
        Attack

    }

   public EnemyState enemyStateCurrent;

    void Update()
    {
        if (enemyStateCurrent == EnemyState.Dead||isDead)
        {
            return;
        }
        if (GameManagerCustom.playerIsDeath==true||GameManagerCustom.isPaused)
        {
            enemyStateCurrent = EnemyState.Idle;
        }
        if(enemyStateCurrent==EnemyState.Wait)
        {
            return;
        }
        if(enemyStateCurrent==EnemyState.Idle)
        {
            IdleAnim();
            enemyStateCurrent = EnemyState.Wait;
            return;
        }
        if(enemyStateCurrent==EnemyState.Stun)
        {
            return;
        }


        float distanceToPlayer = Vector3.Distance(player.position, transform.position);


        if (distanceToPlayer >= attackRange)
        {
            enemyStateCurrent = EnemyState.Walk;
        }
        else
        {
            enemyStateCurrent = EnemyState.Attack;
        }



        if (enemyStateCurrent==EnemyState.Walk)
        {
            FacePlayer();
            ChasePlayer();
        }


        if (enemyStateCurrent==EnemyState.Attack)
        {
            FacePlayer();
            AttemptAttack();
        }
    }

    public void EnemySpawned()
    {
        isDead = false;
        currentHealth = maxHealth;
        isUsable = false;
    }
    public void KnockBack(Vector3 sourcePosition, float knockbackForce, float knockbackDuration)
    {
        // Knockback s�ras�nda hareketi engelle
        enemyStateCurrent = EnemyState.Stun;
        rb.velocity = Vector2.zero; // Mevcut hareketi s�f�rla

        // Geri itme y�n�n� hesapla
        Vector2 difference = (transform.position - sourcePosition).normalized;
        Vector2 forceDirection = difference * knockbackForce;

        // Geri itilme ba�lat
        StartCoroutine(KnockbackCoroutine(forceDirection, knockbackDuration));
    }


    public void TakeDamage(float damage)
    {
        if(CalculateHealth(damage))
        {
            //DamageTaken
        }
        else
        {
            Die();
        }
    }


    private IEnumerator KnockbackCoroutine(Vector2 forceDirection, float duration)
    {
        float timer = 0f;
        rb.velocity = Vector2.zero; // Hareketi durdur
        enemyStateCurrent = EnemyState.Stun; // Knockback bitti, hareket serbest
        while (timer < duration)
        {
            float deceleration = Mathf.Lerp(1f, 0f, timer / duration); // Yava�lama efekti
            rb.velocity = forceDirection * deceleration;

            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        enemyStateCurrent = EnemyState.Walk; // Knockback bitti, hareket serbest
    }

    bool CalculateHealth(float damage,bool heal=false)
    {
        if (heal)
        {
            currentHealth = Mathf.Clamp(currentHealth + damage, 0, maxHealth);
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        }
        if (currentHealth <= 0f)
        {
            return false;
        }
        return true;
    }
    /*
    public void ApplyKnockback(Vector3 sourcePosition, float knockbackForce)
    {
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody2D bile�eni eksik!");
            return;
        }

        rb.velocity = Vector2.zero; // Mevcut hareketi s�f�rla

        // Geri itme y�n�n� hesapla (2D d�zleme uyarlanm�� �ekilde)
        Vector2 difference = (transform.position - sourcePosition).normalized;
        Vector2 forceDirection = difference * knockbackForce;

        // Kuvvet uygula
        rb.AddForce(forceDirection, ForceMode2D.Impulse);

        Debug.Log($"Knockback Applied: {forceDirection}");
    }
    */
    void FacePlayer()
    {
        // Oyuncunun konumuna g�re bak�� y�n� de�i�tir
        if (player.position.x < transform.position.x)
        {
            // Oyuncu solda
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            // Oyuncu sa�da
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void AttemptAttack()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + 1f / attackSpeed;
            rb.velocity = Vector2.zero;
            _anim.SetFloat("RunState", 0f);
            switch (enemyType)
            {
                case EnemyType.Melee:
                    MeeleDamage();
                    _anim.SetTrigger("Attack");
                    break;
                case EnemyType.Ranged:
                    _anim.SetTrigger("Attack");
                    break;
                case EnemyType.Magic:
                    _anim.SetTrigger("Attack");
                    break;
                    /*
                    case EnemyType.Melee:
                        _anim.SetTrigger("AttackMelee");
                        break;
                    case EnemyType.Ranged:
                        _anim.SetTrigger("AttackRanged");
                        break;
                    case EnemyType.Magic:
                        _anim.SetTrigger("AttackMagic");
                        break;
                    */
            }
        }
    }
    void MeeleDamage()
    {
        if (player == null || isDead) return;

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // Sadece oyuncu sald�r� menzilindeyse hasar uygula
        if (distanceToPlayer <= attackRange)
        {
            // Oyuncunun HealthController (veya benzeri) bile�enine eri�
            PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController != null)
            {
                float damageToDeal = attackDamage / attackSpeed; // Sald�r� h�z�na g�re hasar� �l�ekle
                playerController.TakeDamage(damageToDeal); // Hasar� uygula
            }
            else
            {
                Debug.LogWarning("PlayerHealth bile�eni bulunamad�!");
            }
        }
    }

    void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized; // Hedefe y�n
        rb.velocity = direction * moveSpeed; // Rigidbody2D'ye h�z uygula
        // Animasyonu g�ncelle
        if (direction.magnitude > 0)
        {
            _anim.SetFloat("RunState", 0.5f); // Ko�ma animasyonu
        }
        else
        {
            _anim.SetFloat("RunState", 0f); // Bekleme animasyonu
        }
    }
    void IdleAnim()
    {
        rb.velocity = Vector2.zero;
        _anim.SetFloat("RunState", 0f); // Bekleme animasyonu
    }

    void Die()
    {
        if (isDead) return;

        rb.velocity = Vector2.zero;
        enemyStateCurrent = EnemyState.Dead;
        isDead = true;
        _anim.SetTrigger("Die");
        // �ld���nde d��en deneyim puan�n� i�le
        AwardExp();
        StartCoroutine(HandleDeath());
    }
    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(0.75f);
        StopAllCoroutines();
        enemyStateCurrent = EnemyState.Dead;
        isUsable = true;
        gameObject.SetActive(false); // Karakteri devre d��� b�rak   
    }



    void AwardExp()
    {
        // Oyuncunun EXP sistemi burada tetiklenebilir.
        GameManagerCustom.GetPlayerController().GainExperience(expReward);
        Debug.Log($"+{expReward} EXP kazand�n!");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Sald�r� menzilini g�ster
    }
}
