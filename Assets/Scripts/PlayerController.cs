using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerController : MonoBehaviour
{
    public Image healthImage;
    public Image expImage;

    // Temel �zellikler
    public float health;       // Can miktar�
    public float attackSpeed;// Sald�r� h�z�
    public int mana;         // Mana miktar�
    public int attackDamage;  // Sald�r� g�c�
    public float moveSpeed;   // Hareket h�z�
    
    // Ek �zellikler
    public int level = 1;          // Karakter seviyesi
    public float experience = 0;     // Deneyim puan�
    public float experienceToNextLevel = 100;
    public float expBost = 100;
    public float money = 0;          // Para miktar�

    private float nextAttackTime = 0f;

    public Animator _anim;        // Animator bile�eni referans�
    private bool isDead=false;
    private Vector2 lastDirection = Vector2.right; // Son hareket y�n�n� tut


    private float currentHealth;
    private float currentAttackSpeed; // Sald�r� h�z�
    private int currentMana;        // Mana miktar�
    private int currentAttackDamage;  // Sald�r� g�c�
    private float currentMoveSpeed;   // Hareket h�z�




    public float knockBackChance;
    public float knockBackForce; // Geri itme kuvveti
    public float knockBackTime; // Geri itme kuvveti
    public float attackRange; // Sald�r� menzili
    public LayerMask enemyLayer;  // D��manlar� belirlemek i�in Layer Mask

    //
    

    public void SpawnPlayer()
    {
        isDead = false;
        StatSettings();
        ShowHp();
    }
    void StatSettings()
    {
        currentHealth = health;
        currentAttackDamage = attackDamage;
        currentAttackSpeed = attackSpeed;
        currentMana = mana;
        currentMoveSpeed = moveSpeed;
    }




    void Update()
    {
        if (isDead == false)
        {
            // Karakter hareketi
            Move();

            // Sald�r� i�lemi
            if (Input.GetMouseButton(0) && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackSpeed;
            }

            // �l�m kontrol�
            if (health <= 0 && !_anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                Die();
            }
        }
    }

    void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 direction = new Vector2(horizontal, vertical);

        if (direction != Vector2.zero)
        {
            // Son y�n� kaydet
            lastDirection = direction;

            // Y�nlendirme i�lemi: Sa� hareket i�in 0 derece, sola hareket i�in 180 derece
            if (direction.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0); // Sa� y�n
            }
            else if (direction.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0); // Sol y�n
            }

            // Karakteri hareket ettir
            transform.Translate(direction * currentMoveSpeed * Time.deltaTime, Space.World);
                _anim.SetFloat("RunState", 0.5f); // Ko�ma animasyonu
        }
        else
        {
            _anim.SetFloat("RunState", 0f); // Bekleme animasyon
        }
    }

    public void Attack()
    {
        _anim.SetTrigger("Attack");
        _anim.SetFloat("AttackState", 0.0f); // Normal sald�r�
        // Sald�r� menzilindeki d��manlar� bul
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            EnemyController enemy = enemyCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // D��mana geri itme uygula
                Vector3 hitPosition = transform.position; // Oyuncunun pozisyonu
                enemy.TakeDamage(currentAttackDamage);
                if (Random.Range(0,100)<=knockBackForce)
                {
                    Debug.Log("Knock=" + enemy.name);
                }
                Debug.Log("Attacked=" + enemy.name);
            }
        }
    }

    /*
    void Attack()
    {
        // Farkl� sald�r�lar i�in animasyon durumu
        _anim.SetTrigger("Attack");
        _anim.SetFloat("AttackState", 0.0f); // Normal sald�r�
        Debug.Log("Sald�r� yap�ld�! Verilen hasar: " + attackDamage);
    }*/

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        if(HealthCalculate(damage))
        {
            Debug.Log("Hasar al�nd�! Kalan can: " + currentHealth);
        }
        else
        {
            Die();
        }
       
    }
    void ShowHp()
    {
        healthImage.fillAmount = currentHealth / health;
    }
    void ShowExp()
    {
        expImage.fillAmount = experience / experienceToNextLevel;
    }
    public void LevelCalculate(float expValue)
    {
        experience += expValue;
        if(experienceToNextLevel<=experience)
        {
            Debug.Log("LevelUp");
        }
        ShowExp();
    }
    public bool HealthCalculate(float value,bool heal=false)
    {
        if(heal)
        {
            currentHealth=Mathf.Clamp(currentHealth+value,0,health);
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth - value, 0, health);
        }
        ShowHp();
        if(currentHealth<=0)
        {
            //player dead
            return false;
        }
        else
        {
            //player live
            return true;
        }

    }

   

    void Die()
    {
        isDead = true;
        GameManagerCustom.playerIsDeath = true;
        _anim.SetTrigger("Die"); // �l�m animasyonu
        Debug.Log("Karakter �ld�!");
        StartCoroutine(HandleDeath());
    }
    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(1f); // 2 saniye bekle
        gameObject.SetActive(false); // Karakteri devre d��� b�rak
        
    }

    public void UseMana(int amount)
    {
        if (mana >= amount)
        {
            mana -= amount;
            Debug.Log("Mana kullan�ld�! Kalan mana: " + mana);
        }
        else
        {
            Debug.Log("Yeterli mana yok!");
        }
    }

    public void GainExperience(float exp)
    {
        experience += exp;
        Debug.Log(exp + " deneyim kazan�ld�! Toplam deneyim: " + experience);

        // Seviye atlama kontrol�
        if (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        experience = 0;
        experienceToNextLevel += 50;
        attackDamage += 5;
        health += 20;

        Debug.Log("Seviye atland�! Yeni seviye: " + level);

        // Seviye atlama animasyonu
        _anim.SetTrigger("LevelUp");
    }

    public void EarnMoney(int amount)
    {
        money += amount;
        Debug.Log(amount + " alt�n kazan�ld�! Toplam para: " + money);
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            Debug.Log(amount + " alt�n harcand�! Kalan para: " + money);
            return true;
        }
        else
        {
            Debug.Log("Yeterli para yok!"); 
            return false;
        }
    }
    private void OnDrawGizmos()
    {
        // Sald�r� menzilini g�rselle�tirmek i�in bir daire �izin
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
