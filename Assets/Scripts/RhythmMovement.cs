using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RhythmMovement : MonoBehaviour
{
    [Header("Rhythm Settings")]
    public float bpm = 120f;
    public float tilesPerBeat = 2f; // 1박자에 2타일 이동 (X축 속도 기준)

    [Header("Physics Settings")]
    public float tileSize = 1f;
    public float jumpHeight = 3f;      // [변경 추천] 4칸 뛸 거면 높이도 3~4 정도로 키워주세요
    public float jumpDistanceTiles = 4f; // [추가] 점프로 몇 칸을 날아갈지 설정 (4칸 = 2박자)

    // 상태 확인용
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpVelocity;
    private bool isGrounded;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CalculateMovementValues();
    }

    void CalculateMovementValues()
    {
        // 1. 1박자에 걸리는 시간 (초)
        float secPerBeat = 60f / bpm;

        // 2. X축 이동 속도 계산 (변함 없음)
        // 1박자에 2칸 가는 속도는 리듬게임의 규칙이므로 유지
        float distancePerBeat = tilesPerBeat * tileSize;
        moveSpeed = distancePerBeat / secPerBeat;

        // 3. 중력과 점프 힘 재계산 (여기가 핵심 변경!)
        // 목표: "설정한 거리(4칸)를 날아갈 때까지 공중에 떠 있어야 함"

        // Q. 4칸(jumpDistanceTiles)을 가려면 박자가 몇 개 필요한가?
        // A. (목표 4칸) / (1박자당 2칸 속도) = 2박자
        float beatsForJump = jumpDistanceTiles / tilesPerBeat;

        // 실제 체류 시간 = 1박자 시간 * 필요 박자 수
        float totalAirTime = secPerBeat * beatsForJump;

        // 정점까지 걸리는 시간 = 총 체류시간의 절반
        float timeToApex = totalAirTime / 2f;

        // 물리 공식 역산:
        // 중력(g) = (2 * 높이) / (시간^2)
        float newGravity = (2 * jumpHeight) / (timeToApex * timeToApex);

        // 점프 힘(v) = 중력 * 시간
        jumpVelocity = newGravity * timeToApex;

        // 4. 계산된 중력을 유니티 리지드바디에 적용
        rb.gravityScale = newGravity / 9.81f;

        Debug.Log($"점프 계산 완료: {beatsForJump}박자 동안 {jumpDistanceTiles}칸 이동");
    }

    void Update()
    {
        // 등속 운동 유지
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        // 점프 입력
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}