using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
   [Header("몬스터 설정")]
    public int hp = 1; 
    public int attackDamage = 1; // 몬스터의 공격력 (기본 1)
    public float knockbackForce = 7f;
    public GameObject deathEffect; // 죽을 때 나올 효과 (선택사항)
    
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        } else
        {
            ApplyKnockback(attackerPosition);
        }
    }
    public void ApplyKnockback(Vector2 playerPosition)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 knockbackDir = ((Vector2)transform.position - playerPosition).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);
    }
    void Die()
    { 
        Debug.Log("빨간 몬스터 처치!");
        Destroy(gameObject); 
    }
}
