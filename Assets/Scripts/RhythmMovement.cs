using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RhythmMovement : MonoBehaviour
{
    [Header("Rhythm Settings")]
    public float bpm = 120f;
    public float tilesPerBeat = 2f;

    [Header("Physics Settings")]
    public float tileSize = 1f;
    public float jumpHeight = 3f;
    public float jumpDistanceTiles = 4f;

    // 상태 확인용
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpVelocity;
    private bool isGrounded;
    private Rigidbody2D rb;

    // [추가] 애니메이터 변수
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // [추가] 애니메이터 컴포넌트 찾아오기
        anim = GetComponent<Animator>();
        // 만약 자식 오브젝트(Sprite)에 애니메이터가 있다면 아래 줄 주석 해제
        // if (anim == null) anim = GetComponentInChildren<Animator>();

        CalculateMovementValues();
    }

    void CalculateMovementValues()
    {
        float secPerBeat = 60f / bpm;
        float distancePerBeat = tilesPerBeat * tileSize;
        moveSpeed = distancePerBeat / secPerBeat;

        float beatsForJump = jumpDistanceTiles / tilesPerBeat;
        float totalAirTime = secPerBeat * beatsForJump;
        float timeToApex = totalAirTime / 2f;
        float newGravity = (2 * jumpHeight) / (timeToApex * timeToApex);
        jumpVelocity = newGravity * timeToApex;
        rb.gravityScale = newGravity / 9.81f;

        Debug.Log($"점프 설정: {jumpDistanceTiles}칸 점프");
    }

    void Update()
    {
        // 1. 앞으로 계속 달리기 (Run)
        // Idle 상태 없이 무조건 달립니다.
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        // 2. 점프 입력 (Z)
        if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
        {
            Jump();
        }

        // 3. [수정] 공격 입력 (M)
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (anim != null)
            {
                anim.SetTrigger("Attack"); // 애니메이터에 'Attack' 신호 보냄
            }

            // 여기에 실제 공격 판정(Hitbox 켜기 등) 함수를 넣으시면 됩니다.
            // 예: AttackLogic();
        }

        // 4. [추가] 애니메이터에 땅에 닿았는지 알려주기 (점프 모션용)
        if (anim != null)
        {
            anim.SetBool("IsGrounded", isGrounded);
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