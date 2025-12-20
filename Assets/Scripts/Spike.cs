using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    // [Source: 80] 일반 가시는 1, [Source: 81] 즉사 가시는 3~5로 인스펙터에서 설정 가능
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"가시에 찔림! 데미지: {damage}");
            }
        }
    }

}