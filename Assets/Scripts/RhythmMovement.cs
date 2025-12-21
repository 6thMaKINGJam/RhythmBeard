using UnityEngine;
using System.Collections;

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

    [Header("Sync Settings (동기화)")]
    public bool useSync = true; // 동기화 기능 켜기/끄기
    [Tooltip("오차를 얼마나 강하게 보정할지 (높을수록 팍팍 움직임)")]
    public float syncStrength = 5f;

    [Header("Audio Settings")]
    public AudioClip jumpSound;
    public AudioSource bgmSource;

    [Header("Juice & FX")]
    public Vector2 stretchScale = new Vector2(0.7f, 1.3f);
    public Vector2 squashScale = new Vector2(1.3f, 0.7f);
    public float effectDuration = 0.2f;
    public GameObject jumpEffectPrefab;
    public GameObject landEffectPrefab;
    public Vector3 effectOffset = new Vector3(0, -0.5f, 0);

    // 내부 변수
    private Vector3 originalScale;
    private Coroutine squeezeCoroutine;
    private float baseSpeed; // 기준 속도
    private float jumpVelocity;
    private bool isGrounded;
    private bool isFirstLanding = true;

    // [중요] 동기화를 위한 시작 위치 저장용
    private float startXPosition;
    private bool hasMusicStarted = false;

    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource sfxAudioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sfxAudioSource = GetComponent<AudioSource>();
        originalScale = transform.localScale;

        CalculateMovementValues();
    }

    void CalculateMovementValues()
    {
        float secPerBeat = 60f / bpm;
        float distancePerBeat = tilesPerBeat * tileSize;

        // 이것이 이론상 완벽한 속도
        baseSpeed = distancePerBeat / secPerBeat;

        float beatsForJump = jumpDistanceTiles / tilesPerBeat;
        float totalAirTime = secPerBeat * beatsForJump;
        float timeToApex = totalAirTime / 2f;
        float newGravity = (2 * jumpHeight) / (timeToApex * timeToApex);

        jumpVelocity = newGravity * timeToApex;
        rb.gravityScale = newGravity / 9.81f;
    }

    void Update()
    {
        // 1. 음악 시작 체크 (타일 밟아서 시작된 순간을 포착)
        if (bgmSource != null && bgmSource.isPlaying)
        {
            if (!hasMusicStarted)
            {
                // 노래가 딱 켜진 순간의 내 위치를 기준점으로 삼음
                hasMusicStarted = true;
                startXPosition = transform.position.x;

                // [보정] 노래가 0초가 아니라 0.05초 등에서 시작했을 수도 있으므로 역산
                startXPosition -= bgmSource.time * baseSpeed;
            }
        }
        else
        {
            // 노래 안 나오면 그냥 리셋
            hasMusicStarted = false;
        }

        // 2. 이동 로직 (동기화 적용)
        float currentSpeed = baseSpeed;

        if (useSync && hasMusicStarted && bgmSource != null)
        {
            // A. 현재 음악 시간 기준, 내가 '있어야 할 X 좌표' 계산
            // 공식: 시작위치 + (지금까지흐른시간 * 속도)
            float songTime = bgmSource.time;
            float targetX = startXPosition + (songTime * baseSpeed);

            // B. 실제 내 위치와의 오차 계산
            float currentX = transform.position.x;
            float error = targetX - currentX;

            // C. 오차만큼 속도를 더하거나 뺌 (P-Controller 방식)
            // 뒤쳐졌으면(error > 0) 빨라지고, 앞서갔으면(error < 0) 느려짐
            currentSpeed += error * syncStrength;
        }

        // 최종 속도 적용
        rb.velocity = new Vector2(currentSpeed, rb.velocity.y);


        // --- 이하 점프 및 FX 로직 동일 ---

        if (anim != null) anim.speed = 1;

        if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (anim != null) anim.SetTrigger("Attack");
        }

        if (anim != null) anim.SetBool("IsGrounded", isGrounded);
    }

    void Jump()
    {
        isGrounded = false;
        rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);

        if (sfxAudioSource && jumpSound) sfxAudioSource.PlayOneShot(jumpSound, 3.0f);
        if (jumpEffectPrefab) Instantiate(jumpEffectPrefab, transform.position + effectOffset, Quaternion.identity);
        ApplySquashStretch(stretchScale.x, stretchScale.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (isFirstLanding)
            {
                isFirstLanding = false;
                isGrounded = true;
                return;
            }

            if (!isGrounded)
            {
                if (landEffectPrefab) Instantiate(landEffectPrefab, transform.position + effectOffset, Quaternion.identity);
                ApplySquashStretch(squashScale.x, squashScale.y);
            }
            isGrounded = true;
        }
    }

    void ApplySquashStretch(float targetX, float targetY)
    {
        if (squeezeCoroutine != null) StopCoroutine(squeezeCoroutine);
        squeezeCoroutine = StartCoroutine(CoSquashStretch(targetX, targetY));
    }

    IEnumerator CoSquashStretch(float targetXMult, float targetYMult)
    {
        Vector3 targetScale = new Vector3(originalScale.x * targetXMult, originalScale.y * targetYMult, 1f);
        transform.localScale = targetScale;

        float elapsed = 0f;
        while (elapsed < effectDuration)
        {
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / effectDuration);
            yield return null;
        }
        transform.localScale = originalScale;
    }
}