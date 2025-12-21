using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    public int currentHealth;
    public bool isDead = false;

    [Header("Ref Settings")]
    public PlayerHeart playerHeartUI; 
    public ResultManager resultManager;
    public RhythmMovement movementScript;

    [Header("Hit Feedback")]
    public float invincibleTime = 1.0f;
    public Color hurtColor = Color.red;
    public AudioClip hitSound;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private bool isInvincible = false;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        if (playerHeartUI != null) playerHeartUI.UpdateHearts(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        if (isDead || isInvincible) return;

        // 카메라 쉐이크 (Action 스크립트가 있을 경우)
        if (MainCamera_Action.Instance != null)
        {
            MainCamera_Action.Instance.PlayHitShake(0.2f, 0.2f);
        }

        if (audioSource && hitSound) audioSource.PlayOneShot(hitSound, 5.0f);

        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        // UI 업데이트
        if (playerHeartUI != null)
        {
            playerHeartUI.UpdateHearts(currentHealth);
        }

        Debug.Log("플레이어 HP: " + currentHealth);

        // 무적 및 깜빡임 효과
        StartCoroutine(BlinkEffect());

        // 사망 판정
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        if (isDead) return;
        isDead = true;

        // 1. 즉시 멈춰야 할 것들
        if (movementScript != null) movementScript.enabled = false; // 이동 스크립트 중지

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero; // 물리적 속도 제거
            rb.bodyType = RigidbodyType2D.Kinematic; // 물리 영향 차단
        }

        // 2. 애니메이션 멈추기
        Animator anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.speed = 0; // 애니메이션 정지
        }

        // 3. 배경음악 중지
        AudioSource bgm = FindObjectOfType<AudioSource>(); // 씬 내의 메인 오디오 소스 찾기
        if (bgm != null) bgm.Stop();

        // 4. 결과창만 1.5초 뒤에 띄우도록 코루틴 시작
        StartCoroutine(GameOverDelayRoutine(1.5f));
    }

    // 결과창 지연 노출 코루틴
    IEnumerator GameOverDelayRoutine(float delay)
    {
        yield return new WaitForSeconds(delay); // 1.5초 대기

        if (resultManager != null)
        {
            resultManager.ShowResult(); // 결과창 출력
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