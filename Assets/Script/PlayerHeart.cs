using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeart : MonoBehaviour
{
    public int maxHealth = 3; 
    public int currentHealth;

    public GameObject[] hearts;
    public GameObject burstEffectPrefab; // [꼭 확인] 인스펙터에서 하늘색 프리팹을 연결해야 합니다.

    void Start()
    {
        currentHealth = maxHealth;
        for (int i = 0; i < hearts.Length; i++)
    {
        if (hearts[i] != null)
            hearts[i].SetActive(true);
    }
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        
        // 중요: 체력이 0이 되었을 때 결과창 띄우기
        if (currentHealth <= 0)
        {
            FindObjectOfType<ResultManager>().ShowResult();
        }
    }
   
    void UpdateUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            // 현재 체력보다 낮은 인덱스의 하트만 켭니다.
            hearts[i].SetActive(i < currentHealth);
        }
    }

    public void ReduceHeart(int index)
    {
        // 인덱스 범위 확인 및 하트가 활성화 상태인지 체크
    if (index >= 0 && index < hearts.Length && hearts[index].activeSelf)
    {
       
        GameObject burst = Instantiate(burstEffectPrefab, hearts[index].transform.position, Quaternion.identity);
        
        burst.transform.SetParent(hearts[index].transform.parent);

        burst.transform.localPosition = new Vector3(burst.transform.localPosition.x, burst.transform.localPosition.y, 0);
        burst.transform.localScale = Vector3.one;

        hearts[index].SetActive(false);

        Destroy(burst, 1f);
    }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       if (collision.gameObject.CompareTag("Monster"))
    {
        Monster monster = collision.gameObject.GetComponent<Monster>();
        int damageToTake = 1; // 기본값

        if (monster != null)
        {
            damageToTake = monster.attackDamage; // 몬스터의 공격력을 가져옴
            monster.ApplyKnockback(transform.position);
        }

        // 가져온 공격력 수치만큼 반복해서 하트를 터뜨림
        for (int i = 0; i < damageToTake; i++)
        {
            if (currentHealth > 0)
            {
                int targetIndex = currentHealth - 1;
                ReduceHeart(targetIndex); // 하트 이펙트
                TakeDamage(1);            // 체력 감소
            }
        }
    }
    }
}