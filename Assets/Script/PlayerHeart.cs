using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeart : MonoBehaviour
{
    public int maxHealth = 3; 
    public int currentHealth;

    public GameObject[] hearts;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

   public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0) currentHealth = 0;

        //Debug.Log("플레이어 HP: " + currentHealth);
        
        if (currentHealth <= 0)
        {
            FindObjectOfType<ResultManager>().ShowResult();
        }
    }
   
    void UpdateUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].SetActive(true); 
            }
            else
            {
                hearts[i].SetActive(false); 
            }
        }
    }
    public GameObject burstEffectPrefab;

public void ReduceHeart(int index)
{
    if (hearts[index].activeSelf)
    {
      
        GameObject burst = Instantiate(burstEffectPrefab, hearts[index].transform.position, Quaternion.identity);
        
        // 2. 파티클을 UI 레이어(Canvas)에 보이게 설정 (UI 하트일 경우 중요)
        burst.transform.SetParent(hearts[index].transform.parent);
        burst.transform.localScale = Vector3.one;

        // 3. 하트 비활성화
        hearts[index].SetActive(false);
        Destroy(burst, 1f);
    }
}
private void OnCollisionEnter2D(Collision2D collision)
{
   if (collision.gameObject.CompareTag("Monster"))
    {
        if (currentHealth > 0)
        {
            // 지워야 할 하트의 번호 (체력이 3이면 2번 하트, 2면 1번 하트 삭제)
            int targetIndex = currentHealth - 1;

            // 하트 터지는 효과 실행 (이 함수 안에서 하트가 SetActive(false) 됨)
            ReduceHeart(targetIndex);

            // 실제 체력 수치 감소
            TakeDamage(1);
        }
    }
}
}
