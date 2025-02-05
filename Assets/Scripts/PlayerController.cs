using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerController : MonoBehaviour
{
    public Image healthImage;
    public Image expImage;

    // Temel özellikler
    public float health;       // Can miktarý
    public float attackSpeed;// Saldýrý hýzý
    public int mana;         // Mana miktarý
    public int attackDamage;  // Saldýrý gücü
    public float moveSpeed;   // Hareket hýzý
    
    // Ek özellikler
    public int level = 1;          // Karakter seviyesi
    public float experience = 0;     // Deneyim puaný
    public float experienceToNextLevel = 100;
    public float expBost = 100;
    public float money = 0;          // Para miktarý

    private float nextAttackTime = 0f;

    public Animator _anim;        // Animator bileþeni referansý
    private bool isDead=false;
    private Vector2 lastDirection = Vector2.right; // Son hareket yönünü tut


    private float currentHealth;
    private float currentAttackSpeed; // Saldýrý hýzý
    private int currentMana;        // Mana miktarý
    private int currentAttackDamage;  // Saldýrý gücü
    private float currentMoveSpeed;   // Hareket hýzý




    public float knockBackChance;
    public float knockBackForce; // Geri itme kuvveti
    public float knockBackTime; // Geri itme kuvveti
    public float attackRange; // Saldýrý menzili
    public LayerMask enemyLayer;  // Düþmanlarý belirlemek için Layer Mask

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

            // Saldýrý iþlemi
            if (Input.GetMouseButton(0) && Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackSpeed;
            }

            // Ölüm kontrolü
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
            // Son yönü kaydet
            lastDirection = direction;

            // Yönlendirme iþlemi: Sað hareket için 0 derece, sola hareket için 180 derece
            if (direction.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0); // Sað yön
            }
            else if (direction.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0); // Sol yön
            }

            // Karakteri hareket ettir
            transform.Translate(direction * currentMoveSpeed * Time.deltaTime, Space.World);
                _anim.SetFloat("RunState", 0.5f); // Koþma animasyonu
        }
        else
        {
            _anim.SetFloat("RunState", 0f); // Bekleme animasyon
        }
    }

    public void Attack()
    {
        _anim.SetTrigger("Attack");
        _anim.SetFloat("AttackState", 0.0f); // Normal saldýrý
        // Saldýrý menzilindeki düþmanlarý bul
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            EnemyController enemy = enemyCollider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                // Düþmana geri itme uygula
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
        // Farklý saldýrýlar için animasyon durumu
        _anim.SetTrigger("Attack");
        _anim.SetFloat("AttackState", 0.0f); // Normal saldýrý
        Debug.Log("Saldýrý yapýldý! Verilen hasar: " + attackDamage);
    }*/

    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        if(HealthCalculate(damage))
        {
            Debug.Log("Hasar alýndý! Kalan can: " + currentHealth);
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
        _anim.SetTrigger("Die"); // Ölüm animasyonu
        Debug.Log("Karakter öldü!");
        StartCoroutine(HandleDeath());
    }
    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(1f); // 2 saniye bekle
        gameObject.SetActive(false); // Karakteri devre dýþý býrak
        
    }

    public void UseMana(int amount)
    {
        if (mana >= amount)
        {
            mana -= amount;
            Debug.Log("Mana kullanýldý! Kalan mana: " + mana);
        }
        else
        {
            Debug.Log("Yeterli mana yok!");
        }
    }

    public void GainExperience(float exp)
    {
        experience += exp;
        Debug.Log(exp + " deneyim kazanýldý! Toplam deneyim: " + experience);

        // Seviye atlama kontrolü
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

        Debug.Log("Seviye atlandý! Yeni seviye: " + level);

        // Seviye atlama animasyonu
        _anim.SetTrigger("LevelUp");
    }

    public void EarnMoney(int amount)
    {
        money += amount;
        Debug.Log(amount + " altýn kazanýldý! Toplam para: " + money);
    }

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            Debug.Log(amount + " altýn harcandý! Kalan para: " + money);
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
        // Saldýrý menzilini görselleþtirmek için bir daire çizin
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
