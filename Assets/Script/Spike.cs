using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
   public int damage = 1;

   private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어의 스크립트를 가져와 데미지를 입힘
            // 예: other.GetComponent<PlayerHealth>().TakeDamage(damage);
            Debug.Log("플레이어가 가시에 찔렸습니다! 데미지: " + damage);
        }
    }
}
