using UnityEngine;
using System.Collections; // 코루틴 사용을 위해 필수

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

    [Header("Audio Settings")]
    public AudioClip jumpSound;
    public AudioSource bgmSource;

    [Header("Juice Settings (타격감)")]
    public Vector2 stretchScale = new Vector2(0.7f, 1.3f); // 점프할 때 (홀쭉)
    public Vector2 squashScale = new Vector2(1.3f, 0.7f);  // 착지할 때 (납작)
    public float effectDuration = 0.2f; // 원래대로 돌아오는 데 걸리는 시간

    private Vector3 originalScale; // 원래 크기 저장용
    private Coroutine squeezeCoroutine; // 실행 중인 효과를 제어하기 위한 변수

    // 상태 확인용
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpVelocity;
    private bool isGrounded;
    private Rigidbody2D rb;

    // 애니메이터 변수
    private Animator anim;
    private AudioSource sfxAudioSource; // 효과음용

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // [추가] 애니메이터 컴포넌트 찾아오기
        anim = GetComponent<Animator>();

        // [추가] 시작 시 캐릭터의 원래 크기를 저장합니다.
        originalScale = transform.localScale;

        sfxAudioSource = GetComponent<AudioSource>();

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
        // [핵심 로직] 배경음악이 아직 시작 안 했으면 움직이지 않음
        // bgmSource가 연결되어 있고, 아직 플레이 중이 아니라면 대기
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            // X축 속도는 0, Y축(중력)은 유지 (공중에 떠서 시작할 수도 있으니까)
            rb.velocity = new Vector2(0, rb.velocity.y);

            // 애니메이션도 멈춰있게 하려면 속도 0 (선택사항)
            if (anim != null) anim.speed = 0;

            return; // 아래 코드는 실행 안 하고 여기서 Update 종료!
        }

        // --- 노래가 시작되면 아래 코드가 실행됨 ---

        // 애니메이션 다시 재생 (위에서 멈췄을 경우)
        if (anim != null) anim.speed = 1;

        // 1. 앞으로 계속 달리기
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        // 2. 점프 입력 (Z)
        if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
        {
            Jump();
        }

        // 3. 공격 입력 (M)
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (anim != null)
            {
                anim.SetTrigger("Attack"); // 애니메이터에 'Attack' 신호 보냄
            }
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

        // [추가] 점프 시 스트레치(Stretch) 효과 발동
        ApplySquashStretch(stretchScale.x, stretchScale.y);

        if (sfxAudioSource && jumpSound) sfxAudioSource.PlayOneShot(jumpSound, 3.0f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        // [수정] 착지 시 스쿼시(Squash) 효과 발동
        // 공중에 있다가 땅에 닿았을 때만 발동하도록 체크
        if (!isGrounded)
        {
            ApplySquashStretch(squashScale.x, squashScale.y);
        }
        isGrounded = true;
    }

    // [신규 기능] 캐릭터를 찌그러트렸다가 원래대로 복구하는 코루틴
    void ApplySquashStretch(float targetX, float targetY)
    {
        // 이미 효과가 실행 중이라면 취소하고 새로 시작 (중복 방지)
        if (squeezeCoroutine != null) StopCoroutine(squeezeCoroutine);
        squeezeCoroutine = StartCoroutine(CoSquashStretch(targetX, targetY));
    }

    IEnumerator CoSquashStretch(float targetXMult, float targetYMult)
    {
        // 1. 순간적으로 목표 크기(찌그러진 상태)로 변경
        Vector3 targetScale = new Vector3(originalScale.x * targetXMult, originalScale.y * targetYMult, 1f);
        transform.localScale = targetScale;

        float elapsed = 0f;

        // 2. 지정된 시간(effectDuration) 동안 원래 크기로 부드럽게 복구
        while (elapsed < effectDuration)
        {
            elapsed += Time.deltaTime;
            // Lerp를 사용하여 서서히 originalScale로 돌아옴
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / effectDuration);
            yield return null;
        }

        // 3. 확실하게 원래 크기로 고정
        transform.localScale = originalScale;
    }
}