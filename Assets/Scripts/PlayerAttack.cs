using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRangeX = 1.5f;
    public float attackRangeY = 1.0f;
    public Vector2 offset = new Vector2(1.0f, 0);
    public LayerMask enemyLayer;
    public int damage = 1;

    [Header("FX Settings")]
    public GameObject attackEffectPrefab; // 1. 휘두를 때 나오는 이펙트 (검기/베기)
    public GameObject hitEffectPrefab;    // 2. 적이 맞았을 때 터지는 이펙트 (타격/스파크)

    [Header("Audio")]
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
        // 1. 기본 애니메이션 & 소리
        if (anim) anim.SetTrigger("Attack");
        if (audioSource && attackSound) audioSource.PlayOneShot(attackSound, 3.0f);

        // 공격 범위 중심점 계산
        Vector2 attackPos = (Vector2)transform.position + offset;

        // 2. [휘두르기 이펙트] 무조건 실행 (허공에 공격해도 나옴)
        if (attackEffectPrefab != null)
        {
            Instantiate(attackEffectPrefab, attackPos, Quaternion.identity);
        }

        // --- 타격 판정 시작 ---
        Vector2 boxSize = new Vector2(attackRangeX, attackRangeY);
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPos, boxSize, 0f, enemyLayer);

        bool hitAnything = false;

        foreach (Collider2D enemy in hitEnemies)
        {
            Monster monsterScript = enemy.GetComponent<Monster>();
            if (monsterScript != null)
            {
                monsterScript.TakeDamage(damage, (Vector2)gameObject.transform.position);
                hitAnything = true;

                // 3. [타격 성공 이펙트] 적을 맞췄을 때만 몬스터 위치에서 실행
                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, enemy.transform.position, Quaternion.identity);
                }
            }
        }

        // 4. 카메라 흔들림 (적을 하나라도 때렸으면)
        if (hitAnything && MainCamera_Action.Instance != null)
        {
            MainCamera_Action.Instance.PlayHitShake(0.1f, 0.05f);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 attackPos = (Vector2)transform.position + offset;
        Gizmos.DrawWireCube(attackPos, new Vector2(attackRangeX, attackRangeY));
    }
}