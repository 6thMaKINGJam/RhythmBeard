using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRangeX = 1.5f; // 공격 사거리 (가로)
    public float attackRangeY = 1.0f; // 공격 범위 (세로)
    public Vector2 offset = new Vector2(1.0f, 0); // 플레이어 중심에서 얼마나 앞을 때릴지
    public LayerMask enemyLayer; // 몬스터만 때리기 위한 레이어 설정
    public int damage = 1; // 공격력 1 [cite: 70]

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            Attack();
        }
    }

    void Attack()
    {
        if (anim) anim.SetTrigger("Attack");

        // 2. 공격 범위 계산 (플레이어 위치 + offset 위치에 사각형 그림)
        Vector2 attackPos = (Vector2)transform.position + offset;
        Vector2 boxSize = new Vector2(attackRangeX, attackRangeY);

        // 3. 해당 범위 내의 모든 콜라이더 감지
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPos, boxSize, 0f, enemyLayer);

        // 4. 감지된 적에게 데미지 전달
        foreach (Collider2D enemy in hitEnemies)
        {
            Monster monsterScript = enemy.GetComponent<Monster>();
            if (monsterScript != null)
            {
                monsterScript.TakeDamage(damage);
                Debug.Log("몬스터 타격 성공!");
            }
        }
    }

    // 에디터에서 공격 범위를 눈으로 보기 위한 기능 (게임엔 영향 X)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 attackPos = (Vector2)transform.position + offset;
        Gizmos.DrawWireCube(attackPos, new Vector2(attackRangeX, attackRangeY));
    }
}