using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RhythmMovement : MonoBehaviour
{
    [Header("Rhythm Settings")]
    public float bpm = 120f;   // 기획서 기준 BPM
    public float tilesPerBeat = 2f; // 요청하신 1박자당 2타일 이동

    [Header("Physics Settings")]
    public float tileSize = 1f; // 타일맵 그리드 사이즈 (보통 1)

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

        // 2. X축 이동 속도 계산 (거리 / 시간)
        // [cite: 51] 달리기 속도는 BPM 상수에 따름
        float distancePerBeat = tilesPerBeat * tileSize;
        moveSpeed = distancePerBeat / secPerBeat;

        // 3. Y축 점프 힘 계산 (정확히 1박자 뒤에 착지하도록)
        // 공식: 초기속도 = (중력 * 시간) / 2
        //  한 박에 딱 땅에 떨어져야 함
        float gravity = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        jumpVelocity = (gravity * secPerBeat) / 2f;
    }

    void Update()
    {
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        // 점프 입력 (스페이스바)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        // 기존 Y속도를 0으로 초기화 후 점프 힘 적용 (더 깔끔한 점프를 위해)
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        isGrounded = false; // 점프 즉시 땅에서 떨어짐 처리
    }

    // 바닥 체크 (Collision 활용)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 바닥 태그 확인 (타일맵에 "Ground" 태그 설정 필요)
        if (collision.gameObject.CompareTag("Ground"))
        {
            // 착지 시점 오차 보정 (선택사항):
            // 미세하게 Y위치가 어긋나면 강제로 정수로 맞춰주는 로직이 필요할 수 있음
            isGrounded = true;
        }
    }
}