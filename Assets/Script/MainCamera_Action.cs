using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera_Action : SceneSingleton<MainCamera_Action>
{
    [Header("Target Settings")]
    [SerializeField] private Transform playerTransform; 
    
    [Header("X-Axis (Horizontal) Settings")]
    [SerializeField] private float offsetX = 5.0f;       // 캐릭터 앞쪽 시야 확보
    
    [Header("Y-Axis (Vertical) Settings")]
    [SerializeField] private float yOffset = 2.0f;       // 지면으로부터의 카메라 높이
    [SerializeField] private float ySmoothSpeed = 5.0f;  // Y축 이동 부드러움 정도
    private float targetY;                               // 현재 따라가야 할 지면 높이

    [Header("Shake Settings")]
    private Vector3 shakeOffset = Vector3.zero;

    public new void Awake()
        {
            base.Awake();
            
            // 씬에 "Player" 태그를 가진 오브젝트를 자동으로 찾아 연결
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null) playerTransform = player.transform;
            }
        }

    // Start is called before the first frame update
    void Start()
    {
        if (playerTransform != null)
        {
            targetY = playerTransform.position.y;
        }
    }

    // Update is called once per frame
    private void LateUpdate() {
        if (playerTransform == null) return;

        // 1. X축: 플레이어를 즉시 또는 부드럽게 추적
        float targetX = playerTransform.position.x + offsetX;

        // 2. Y축: 플레이어가 밟고 있는 지면(플랫폼) 높이 추적
        // 플레이어 스크립트에서 지면 착지 시 targetY를 업데이트해준다고 가정
        float desiredY = targetY + yOffset;
        float currentY = Mathf.Lerp(transform.position.y, desiredY, Time.deltaTime * ySmoothSpeed);

        // 3. 최종 위치 적용 (흔들림 효과인 shakeOffset 포함)
        transform.position = new Vector3(targetX, currentY, -10f) + shakeOffset;
    }

    

    // --- 외부 호출용 메서드 ---

    /// <summary>
    /// 플레이어가 밟은 새로운 발판의 높이를 설정합니다. (플레이어 착지 시 호출)
    /// </summary>
    public void UpdateGroundLevel(float newY)
    {
        targetY = newY;
    }

    /// <summary>
    /// 피격 시 카메라 흔들림 효과를 발생시킵니다.
    /// </summary>
    public void PlayHitShake(float duration = 0.2f, float magnitude = 0.3f)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0);
            elapsed += Time.deltaTime;

            yield return null;
        }

        shakeOffset = Vector3.zero;
    }

    [ContextMenu("Test Shake")]
    public void TestShake()
    {
        PlayHitShake(0.2f, 0.5f);
    }
}
