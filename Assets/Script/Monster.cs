using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMonster : MonoBehaviour
{
   [Header("몬스터 설정")]
    public int hp = 1; 
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
    void ApplyKnockback(Vector2 attackerPosition)
    {
        Vector2 knockbackDirection = (Vector2)transform.position - attackerPosition;
        
        knockbackDirection = knockbackDirection.normalized;

        rb.velocity = Vector2.zero; 
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        
        //Debug.Log("몬스터가 밀려납니다! 남은 HP: " + hp);
    }
    void Die()
    { 
        Debug.Log("빨간 몬스터 처치!");
        Destroy(gameObject); 
    }
}
