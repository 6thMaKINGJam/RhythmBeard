using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeart : MonoBehaviour
{
    public int maxHealth = 3; 
    public int currentHealth;

    [Header("하트 이미지 설정")]
    public Image[] heartImages;    // Element 0:왼쪽, 1:가운데, 2:오른쪽 순서로 연결하세요.
    public Sprite fullHeart;       
    public Sprite emptyHeart;      
    
    [Header("효과 설정")]
    public GameObject burstEffectPrefab; 

    private bool isInvincible = false; // 무적 상태 변수

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    // --- 충돌 감지 부분 분리 ---

    // 1. 일반 몬스터와 물리적 충돌 시
    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleDamage(collision.gameObject);
    }

    // 2. 스파이크(Is Trigger 체크됨)와 겹쳤을 때
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HandleDamage(collision.gameObject);
    }

    // 3. 공통 데미지 처리 로직
    public void HandleDamage(GameObject enemy)
    {
        if (isInvincible) return; // 무적이면 무시

        if (enemy.CompareTag("Monster"))
        {
            Monster monster = enemy.GetComponent<Monster>();
            int damageToTake = (monster != null) ? monster.attackDamage : 1;

            if (monster != null) monster.ApplyKnockback(transform.position);

            // 무적 시간 및 깜빡임 시작
            StartCoroutine(InvincibilityRoutine());

            // 데미지만큼 반복
            for (int i = 0; i < damageToTake; i++)
            {
                if (currentHealth > 0)
                {
                    // [중요] 현재 남은 체력의 마지막 인덱스를 타겟으로 잡음
                    int targetIndex = currentHealth - 1; 

                    // 데이터 먼저 감소
                    currentHealth--; 
                    
                    // 해당 인덱스의 하트 UI 연출 실행
                    ReduceHeart(targetIndex);

                    if (currentHealth <= 0)
                    {
                        FindObjectOfType<ResultManager>().ShowResult();
                        break;
                    }
                }
            }
        }
    }

    public void ReduceHeart(int index)
    {
        if (index >= 0 && index < heartImages.Length)
        {
            GameObject burst = Instantiate(burstEffectPrefab, heartImages[index].transform.position, Quaternion.identity);
            burst.transform.SetParent(heartImages[index].transform.parent);
            burst.transform.localPosition = new Vector3(heartImages[index].transform.localPosition.x, heartImages[index].transform.localPosition.y, 0);
            burst.transform.localScale = Vector3.one;
            Destroy(burst, 1f);

            StartCoroutine(BlinkAndChangeRoutine(index));
        }
    }

    IEnumerator BlinkAndChangeRoutine(int index)
    {
        Image heartImg = heartImages[index];
        float blinkSpeed = 0.1f;

        heartImg.color = new Color(1, 1, 1, 0); 
        yield return new WaitForSeconds(blinkSpeed);
        
        heartImg.sprite = emptyHeart; 
        heartImg.color = Color.white; 
        yield return new WaitForSeconds(blinkSpeed);

        heartImg.color = new Color(1, 1, 1, 0);
        yield return new WaitForSeconds(blinkSpeed);
        
        heartImg.color = Color.white;
    }

    IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        
        // 1초 동안 캐릭터 깜빡임
        for (int i = 0; i < 5; i++)
        {
            if(spr) spr.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(0.1f);
            if(spr) spr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        isInvincible = false;
    }

    void UpdateUI()
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].sprite = (i < currentHealth) ? fullHeart : emptyHeart;
            heartImages[i].color = Color.white;
        }
    }
}