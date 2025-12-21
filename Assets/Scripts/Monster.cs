using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour
{
    [Header("Status")]
    public int maxHp = 2;
    private int currentHp;
    public int attackDamage = 1;

    [Header("Knockback Settings")]
    public float deathKnockbackForce = 15f;
    public float bumpKnockbackForce = 5f;

    [Header("Feedback")]
    public AudioClip hitSound;
    public GameObject hitEffect;

    [Header("Juice Settings (타격감)")] // [추가]
    public Vector2 hitSquashScale = new Vector2(1.2f, 0.8f); // 맞았을 때 (옆으로 퍼짐)
    public float hitDuration = 0.2f; // 원래대로 돌아오는 시간

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer spriteRenderer;

    private Vector3 originalScale; // [추가] 원래 크기 저장
    private Coroutine hitRoutine;  // [추가] 중복 실행 방지용

    void Start()
    {
        currentHp = maxHp;
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // [추가] 시작 시 크기 저장
        originalScale = transform.localScale;
    }

    // 1. 칼 공격 맞았을 때
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        currentHp -= damage;

        if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
        if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);

        // [추가] 맞았으니까 찌그러짐 연출 실행!
        PlayHitReaction();

        if (currentHp <= 0)
        {
            ApplyKnockback(attackerPosition, deathKnockbackForce);
            Die();
        }
        else
        {
            Debug.Log("몬스터가 공격을 버텼습니다.");
        }
    }

    // 2. 플레이어와 몸통 박치기
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);

                ApplyKnockback(collision.transform.position, bumpKnockbackForce);

                // [추가] 부딪힐 때도 충격 느낌을 주기 위해 찌그러짐 실행
                PlayHitReaction();

                Die();
            }
        }
    }

    public void ApplyKnockback(Vector2 targetPos, float force)
    {
        if (rb == null || rb.bodyType != RigidbodyType2D.Dynamic) return;

        Vector2 dir = ((Vector2)transform.position - targetPos).normalized;
        Vector2 finalDir = (dir + Vector2.up * 0.2f).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(finalDir * force, ForceMode2D.Impulse);
    }

    void Die()
    {
        if (col != null) col.enabled = false;
        StartCoroutine(FadeOutAndDestroy());
    }

    IEnumerator FadeOutAndDestroy()
    {
        float duration = 0.4f;
        float timer = 0f;
        Color startColor = spriteRenderer.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / duration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    // [추가] 몬스터가 맞았을 때 실행할 스쿼시 연출 함수
    void PlayHitReaction()
    {
        if (hitRoutine != null) StopCoroutine(hitRoutine);
        hitRoutine = StartCoroutine(CoHitSquash());
    }

    // [추가] 실제로 크기를 조절하는 코루틴
    IEnumerator CoHitSquash()
    {
        // 1. 순식간에 찌그러짐 (Squash)
        // 몬스터가 왼쪽/오른쪽을 보고 있을 수 있으므로(X축 반전 등) 절대값 대신 비율을 곱합니다.
        Vector3 targetScale = new Vector3(
            originalScale.x * hitSquashScale.x,
            originalScale.y * hitSquashScale.y,
            1f
        );

        transform.localScale = targetScale;

        float elapsed = 0f;

        // 2. 부드럽게 원래 크기로 복구
        while (elapsed < hitDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / hitDuration);
            yield return null;
        }

        transform.localScale = originalScale;
    }
}