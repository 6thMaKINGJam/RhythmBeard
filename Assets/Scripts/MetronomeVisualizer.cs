using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MetronomeVisualizer : MonoBehaviour
{
    [Header("Visual Settings")]
    [Tooltip("비트의 간격 (1 = 4분음표, 0.5 = 2분음표 등)")]
    [SerializeField] private float _beatStep = 1f;

    [Tooltip("값이 클수록 잔상이 빨리 사라져서 타격감이 강해집니다.")]
    [SerializeField] private float _tensionPower = 3f;

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // 비트매니저가 없거나 오디오가 재생 중이 아니면 숨김 처리 (선택사항)
        if (BeatManager.Instance == null || BeatManager.Instance.CurrentBeat == 0)
        {
            SetAlpha(0f);
            return;
        }

        // 1. 현재 박자 정보를 가져옴 (예: 10.5박자)
        // _beatStep을 곱해서 리듬 간격 조절 (기본 1)
        float currentBeat = BeatManager.Instance.CurrentBeat * _beatStep;

        // 2. 비트의 진행도(0.0 ~ 1.0)만 추출
        // 정박(1.0, 2.0...)일 때 0이 됨
        float progress = currentBeat % 1f;

        // 3. 텐션 계산 (역방향: 정박일 때 1, 시간이 지날수록 0으로 감)
        // 1 - progress를 하면 비트 시작(0.0)일 때 1.0이 됨
        float rawAlpha = 1f - progress;

        // 4. 곡선 적용 (Power 함수를 써서 선형이 아닌 '텐션' 있는 감쇠 효과)
        // 1차 함수(직선)보다 3제곱 등을 하면 확! 켜졌다가 빠르게 식는 느낌이 남
        float finalAlpha = Mathf.Pow(rawAlpha, _tensionPower);

        // 5. 적용
        SetAlpha(finalAlpha);
    }

    private void SetAlpha(float alpha)
    {
        Color c = _spriteRenderer.color;
        c.a = alpha;
        _spriteRenderer.color = c;
    }
}