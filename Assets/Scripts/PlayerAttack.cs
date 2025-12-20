using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRangeX = 1.5f; // ���� ��Ÿ� (����)
    public float attackRangeY = 1.0f; // ���� ���� (����)
    public Vector2 offset = new Vector2(1.0f, 0); // �÷��̾� �߽ɿ��� �󸶳� ���� ������
    public LayerMask enemyLayer; // ���͸� ������ ���� ���̾� ����
    public int damage = 1; // ���ݷ� 1 [cite: 70]

    public AudioClip attackSound;
    private AudioSource audioSource;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Attack();
        }
    }

    void Attack()
    {
        if (anim) anim.SetTrigger("Attack");
        if (audioSource && attackSound) audioSource.PlayOneShot(attackSound,3.0f);

        // 2. ���� ���� ��� (�÷��̾� ��ġ + offset ��ġ�� �簢�� �׸�)
        Vector2 attackPos = (Vector2)transform.position + offset;
        Vector2 boxSize = new Vector2(attackRangeX, attackRangeY);

        // 3. �ش� ���� ���� ��� �ݶ��̴� ����
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPos, boxSize, 0f, enemyLayer);

        // 4. ������ ������ ������ ����
        foreach (Collider2D enemy in hitEnemies)
        {
            Monster monsterScript = enemy.GetComponent<Monster>();
            if (monsterScript != null)
            {
                monsterScript.TakeDamage(damage, (Vector2)gameObject.transform.position);
                Debug.Log("���� Ÿ�� ����!");
            }
        }
    }

    // �����Ϳ��� ���� ������ ������ ���� ���� ��� (���ӿ� ���� X)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 attackPos = (Vector2)transform.position + offset;
        Gizmos.DrawWireCube(attackPos, new Vector2(attackRangeX, attackRangeY));
    }
}