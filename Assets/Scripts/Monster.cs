using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Settings")]
    public int maxHp = 1; // ��ȹ��: ���� ����=1, ��� ����=2 [cite: 78, 79]
    private int currentHp;
    public int attackDamage = 1;

    [Header("Feedback")]
    public AudioClip hitSound; // Ÿ���� [cite: 71]
    public GameObject hitEffect; // Ÿ�� ����Ʈ (��ƼŬ ��)

    private AudioSource audioSource;

    private Rigidbody2D rb;

    void Start()
    {
        currentHp = maxHp;
        audioSource = GetComponent<AudioSource>(); // ������ Add Component �ʿ�
        rb = GetComponent<Rigidbody2D>();
    }

    // �÷��̾ ȣ���� �Լ�
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        currentHp -= damage;

        if (audioSource && hitSound) audioSource.PlayOneShot(hitSound);
        if (hitEffect) Instantiate(hitEffect, transform.position, Quaternion.identity);

        // �ִϸ��̼��� �ִٸ� ���⼭ Ʈ���� (��: animator.SetTrigger("Hit"))

        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            ApplyKnockback(attackerPosition);
        }
    }

    public void ApplyKnockback(Vector2 playerPosition)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Vector2 knockbackDir = ((Vector2)transform.position - playerPosition).normalized;
        rb.velocity = Vector2.zero;
        rb.AddForce(knockbackDir * 5f, ForceMode2D.Impulse);
    }

    void Die()
    {
        // ���� ��� ó�� (���� �߰� �� ������ �ִٸ� ���⿡ �߰�)
        Destroy(gameObject);
    }

    // ���� Monster.cs ���� �Ʒ��� �� �Լ��� �߰��ϼ���.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // [Source: 80] ������ �ǰݵǴ� ���� / ���� ���� ����
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

            // �÷��̾� ü�� ��ũ��Ʈ�� �ִٸ� ������ �ֱ�
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);

                // ���� ����: 
                // ���Ͷ� �ε����� �� ���Ͱ� ������� �Ѵٸ�: Destroy(gameObject);
                // ���ʹ� �״�� �ְ� �÷��̾ ���� ���·� ����ؾ� �Ѵٸ�: �ƹ��͵� �� �ص� �� (PlayerHealth���� ���� ó����)
            }
        }
    }
}