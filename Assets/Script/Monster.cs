using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedMonster : MonoBehaviour
{
   [Header("몬스터 설정")]
    public int hp = 1; 
    public GameObject deathEffect; // 죽을 때 나올 효과 (선택사항)

    public void TakeDamage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            Die();
        }
    }
    void Die()
    { 
        Debug.Log("빨간 몬스터 처치!");
        Destroy(gameObject); 
    }
}
