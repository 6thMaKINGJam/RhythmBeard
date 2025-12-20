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

        if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);

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