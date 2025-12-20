using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Settings")]
    public int maxHp = 1; // 기획서: 빨간 몬스터=1, 노란 몬스터=2 [cite: 78, 79]
    private int currentHp;

    [Header("Feedback")]
    public AudioClip hitSound; // 타격음 [cite: 71]
    public GameObject hitEffect; // 타격 이펙트 (파티클 등)

    private AudioSource audioSource;

    void Start()
    {
        currentHp = maxHp;
        audioSource = GetComponent<AudioSource>(); // 없으면 Add Component 필요
    }

    // 플레이어가 호출할 함수
    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
        if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);

        // 애니메이션이 있다면 여기서 트리거 (예: animator.SetTrigger("Hit"))

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // 몬스터 사망 처리 (점수 추가 등 로직이 있다면 여기에 추가)
        Destroy(gameObject);
    }

    // 기존 Monster.cs 내용 아래에 이 함수를 추가하세요.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // [Source: 80] 닿으면 피격되는 가시 / 몬스터 공통 로직
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            // 플레이어 체력 스크립트가 있다면 데미지 주기
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);

                // 선택 사항: 
                // 몬스터랑 부딪혔을 때 몬스터가 사라져야 한다면: Destroy(gameObject);
                // 몬스터는 그대로 있고 플레이어만 무적 상태로 통과해야 한다면: 아무것도 안 해도 됨 (PlayerHealth에서 무적 처리함)
            }
        }
    }
}