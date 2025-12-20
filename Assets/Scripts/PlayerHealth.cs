using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // 게임오버 처리를 위해 필요
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;       //  HP 3개
    public int currentHealth;
    public bool isDead = false;

    [Header("UI Settings")]
    public GameObject[] hearts;

    [Header("Hit Feedback")]
    public float invincibleTime = 1.0f; //  1초간 깜빡임
    public Color hurtColor = Color.red; //  피격 시 빨간색
    public AudioClip hitSound;          //  피격 효과음

    public ResultManager resultManager; // 결과창 매니저 연결
    public RhythmMovement movementScript; // 움직임 스크립트 연결 (멈추기 위해)

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();

        UpdateUI();
    }

    // 몬스터가 호출할 함수
    public void TakeDamage(int amount)
    {
        // 이미 죽었거나 무적 상태면 데미지 무시
        if (isDead || isInvincible) return;

        // 1. 체력 감소
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        Debug.Log("플레이어 HP: " + currentHealth);

        // 2. UI 즉시 갱신 (UI 팀원 코드 반영)
        UpdateUI();

        // 3. 효과음 재생
        if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);

        // 4. 무적 및 깜빡임 효과 시작
        StartCoroutine(BlinkEffect());

        // 5. 게임오버 체크
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }
    void UpdateUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth) hearts[i].SetActive(true);
            else hearts[i].SetActive(false);
        }
    }

    void GameOver()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Game Over!");

        // 1. 플레이어 움직임 멈추기
        if (movementScript != null) movementScript.enabled = false;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 미끄러짐 방지
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; // 물리 영향 제거

        AudioSource bgm = FindObjectOfType<AudioSource>();
        if (bgm != null) bgm.Stop();

        if (resultManager != null)
        {
            resultManager.ShowResult();
        }
    }

    IEnumerator BlinkEffect()
    {
        isInvincible = true;
        Color originalColor = spriteRenderer.color;

        float timer = 0;
        while (timer < invincibleTime)
        {
            spriteRenderer.color = hurtColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(0.1f);
            timer += 0.2f;
        }

        spriteRenderer.color = originalColor;
        isInvincible = false;
    }
}