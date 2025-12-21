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
    public PlayerHeart playerHeartUI; // ��� ������ UI ��ũ��Ʈ ����
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

        // ���� �� ��Ʈ �� ä���
        if (playerHeartUI != null) playerHeartUI.UpdateHearts(currentHealth);
    }

    // ���Ͱ� ���� �� ȣ���ϴ� �Լ�
    public void TakeDamage(int amount)
    {
        if (isDead || isInvincible) return;

        if (MainCamera_Action.Instance != null)
        {
            MainCamera_Action.Instance.PlayHitShake(0.2f, 0.2f);
        }

        if (audioSource && hitSound) audioSource.PlayOneShot(hitSound, 5.0f);

        // ü�� ����
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        // [����] ��ƼŬ ����, �׳� ��Ʈ �׸��� �����϶�� ����
        if (playerHeartUI != null)
        {
            playerHeartUI.UpdateHearts(currentHealth);
        }

        Debug.Log("�÷��̾� HP: " + currentHealth);

        // ���� �ð� ����
        StartCoroutine(BlinkEffect());

        // ���� ����
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

        // 3. [�߰�] �ִϸ��̼� ���߱�(����!)
        // (���� �ִϸ����Ͱ� �ڽ� ������Ʈ�� �ִٸ� GetComponentInChildren ���)
        Animator anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();

        if (anim != null)
        {
            anim.speed = 0; // ��� �ӵ��� 0���� �ϸ� ���� ���ۿ��� �״�� ����ϴ�.
            // �ƿ� ���� �ʹٸ�: anim.enabled = false;
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