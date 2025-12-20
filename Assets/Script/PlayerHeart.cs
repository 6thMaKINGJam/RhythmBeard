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

        Debug.Log("플레이어 HP: " + currentHealth);
        
        UpdateUI();

        if (currentHealth <= 0)
        {
            Debug.Log("게임 오버!");
            // 여기에 캐릭터를 멈추거나 재시작하는 코드를 넣을 수 있습니다.
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
}
