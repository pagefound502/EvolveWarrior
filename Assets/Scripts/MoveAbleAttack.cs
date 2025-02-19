using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAbleAttack : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody2D rigidbody2;

    public bool ownerPlayer = false;


    public float moveSpeed = 7f;
    public float damage = 0;


    public void TargetLocation(Vector2 pos)
    {
        rigidbody2.velocity = pos.normalized* moveSpeed;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")&&!ownerPlayer)
        {
            Attack(collision.GetComponent<PlayerController>());
            DeSpawn();
        }
        else if(collision.CompareTag("Obstacle"))
        {
            DeSpawn();
        }
    }
    public void DeSpawn()
    {
        gameObject.SetActive(false);
    }
    public void Attack(PlayerController playerController)
    {
        playerController.TakeDamage(damage);

    }

}
