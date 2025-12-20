using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필수
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

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer spriteRenderer; // [추가] 색상을 바꾸기 위해 필요

    void Start()
    {
        currentHp = maxHp;
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // [추가] 렌더러 가져오기
    }

    // 1. 칼 공격 맞았을 때
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        currentHp -= damage;

        if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
        if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);

        if (currentHp <= 0)
        {
            // 체력 0됨 -> 사망 넉백 적용 후 죽음
            ApplyKnockback(attackerPosition, deathKnockbackForce);
            Die();
        }
        else
        {
            // 체력 남음 -> 넉백 없음 (버팀)
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

                // [수정] 부딪히면 튕겨나가는 힘 적용
                ApplyKnockback(collision.transform.position, bumpKnockbackForce);

                // [추가] 몸통 박치기 후 몬스터도 죽어야(사라져야) 함!
                Die();
            }
        }
    }

    public void ApplyKnockback(Vector2 targetPos, float force)
    {
        if (rb == null || rb.bodyType != RigidbodyType2D.Dynamic) return;

        Vector2 dir = ((Vector2)transform.position - targetPos).normalized;

        // 대각선 위로 띄우는 비율을 0.5 -> 0.2로 줄임 (너무 붕 뜨지 않게)
        Vector2 finalDir = (dir + Vector2.up * 0.2f).normalized;

        rb.velocity = Vector2.zero;
        rb.AddForce(finalDir * force, ForceMode2D.Impulse);
    }

    void Die()
    {
        // 충돌을 즉시 꺼서 플레이어가 지나갈 수 있게 함
        if (col != null) col.enabled = false;

        // 2. [변경] 그냥 삭제하지 않고, 서서히 사라지는 연출 시작
        StartCoroutine(FadeOutAndDestroy());
    }

    // [추가된 기능] 서서히 투명해지는 함수
    IEnumerator FadeOutAndDestroy()
    {
        float duration = 0.4f; // 사라지는 데 걸리는 시간 (1초)
        float timer = 0f;
        Color startColor = spriteRenderer.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / duration); // 1(불투명) -> 0(투명)으로 변환

            // 색상의 알파값(투명도)만 변경
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null; // 한 프레임 대기
        }

        // 완전히 투명해지면 삭제
        Destroy(gameObject);
    }
}