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
    public PlayerHeart playerHeartUI; // 방금 수정한 UI 스크립트 연결
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

        // 시작 시 하트 꽉 채우기
        if (playerHeartUI != null) playerHeartUI.UpdateHearts(currentHealth);
    }

    // 몬스터가 때릴 때 호출하는 함수
    public void TakeDamage(int amount)
    {
        if (isDead || isInvincible) return;

        if (audioSource && hitSound) audioSource.PlayOneShot(hitSound, 5.0f);

        // 체력 감소
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        // [수정] 파티클 없이, 그냥 하트 그림만 갱신하라고 명령
        if (playerHeartUI != null)
        {
            playerHeartUI.UpdateHearts(currentHealth);
        }

        Debug.Log("플레이어 HP: " + currentHealth);

        // 무적 시간 시작
        StartCoroutine(BlinkEffect());

        // 게임 오버
        if (currentHealth <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        if (isDead) return;
        isDead = true;

        if (movementScript != null) movementScript.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 3. [추가] 애니메이션 멈추기(얼음!)
        // (만약 애니메이터가 자식 오브젝트에 있다면 GetComponentInChildren 사용)
        Animator anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        if (anim != null)
        {
            anim.speed = 0; // 재생 속도를 0으로 하면 현재 동작에서 그대로 멈춥니다.
            // 아예 끄고 싶다면: anim.enabled = false;
        }

        AudioSource bgm = FindObjectOfType<AudioSource>();
        if (bgm != null) bgm.Stop();

        if (resultManager != null) resultManager.ShowResult();
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