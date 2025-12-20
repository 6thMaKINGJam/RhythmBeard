using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundManager : SceneSingleton<BackGroundManager>
{
    [System.Serializable]
    public struct BackgroundPhase
    {
        public string phaseName;
        public SpriteRenderer bgRenderer; // 해당 배경의 Renderer
        public Color cloudColor;       // 이 배경일 때 구름의 색상
    }

    // 실시간으로 변하는 구름 리스트와 초기 색상 저장
    private List<SpriteRenderer> cloudList = new List<SpriteRenderer>();
    private Dictionary<SpriteRenderer, Color> initialCloudColors = new Dictionary<SpriteRenderer, Color>();

    public BackgroundPhase[] phases;
    public SpriteRenderer[] cloudRenderers; // 씬에 있는 모든 구름들
    public float fadeSpeed = 2.0f;

    private int currentPhaseIndex = 0;

    private void Start()
    {
        // 시작할 때 각 구름의 고유한 색상을 저장해둡니다.
        foreach (var cloud in cloudRenderers)
        {
            if (cloud != null)
                RegisterCloud(cloud);
        }
    }
    private void Update()
    {
        // 업데이트에서는 현재 정해진 페이즈로 부드럽게 색상/알파값을 변경하는 일만 합니다.
        UpdateBackgrounds();
        UpdateCloudColors();
    }

    // 새로운 구름이 생성되거나 루프될 때 호출
    public void RegisterCloud(SpriteRenderer cloud)
    {
        if (cloud == null) return;
        if (!cloudList.Contains(cloud))
        {
            cloudList.Add(cloud);
            if (!initialCloudColors.ContainsKey(cloud))
                initialCloudColors[cloud] = cloud.color;
        }
    }

    // [핵심] 리듬 매니저나 노트 판정 스크립트에서 이 함수를 호출하게 됩니다.
    // 예: BackGroundManager.Instance.SetPhase(1); 
    // 0: Day, 1: SunSet, 2: Night, 3: Dawn 
    public void SetPhase(int index)
    {
        if (index >= 0 && index < phases.Length)
        {
            currentPhaseIndex = index;
            Debug.Log($"배경 페이즈 전환: {phases[index].phaseName}");
        }
        else
        {
            Debug.LogWarning("잘못된 배경 페이즈 인덱스입니다.");
        }
    }

    void UpdateBackgrounds()
    {
        for (int i = 0; i < phases.Length; i++)
        {
            if (phases[i].bgRenderer == null) continue;

            float targetAlpha = (i == currentPhaseIndex) ? 1f : 0f;
            Color c = phases[i].bgRenderer.color;
            
            // 부드럽게 알파값 변경 (Fade In/Out)
            c.a = Mathf.MoveTowards(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
            phases[i].bgRenderer.color = c;
        }
    }

    void UpdateCloudColors()
    {
        if (phases.Length == 0) return;

        // 현재 배경 페이즈의 테마 색상 (예: 노을의 경우 약간 주황빛)
        Color themeColor = phases[currentPhaseIndex].cloudColor;

        foreach (var cloud in cloudList)
        {
            if (cloud == null || !initialCloudColors.ContainsKey(cloud)) continue;

            // [방법] 고유 색상과 테마 색상을 곱해서 섞습니다.
            // 이렇게 하면 원래 밝은 구름은 테마 아래서도 조금 더 밝게 유지됩니다.
            Color targetColor = initialCloudColors[cloud] + themeColor;

            // 색상값이 1(흰색)을 넘어가서 눈부시지 않도록 제한
            targetColor.r = Mathf.Clamp01(targetColor.r);
            targetColor.g = Mathf.Clamp01(targetColor.g);
            targetColor.b = Mathf.Clamp01(targetColor.b);
            // 투명도는 구름의 원래 투명도를 유지합니다.
            targetColor.a = initialCloudColors[cloud].a;
            
            cloud.color = Color.Lerp(cloud.color, targetColor, Time.deltaTime * fadeSpeed);
        }
    }

    // 테스트용
    [ContextMenu("Test Phase 0")] void TestPhase0() => SetPhase(0);
    [ContextMenu("Test Phase 1")] void TestPhase1() => SetPhase(1);
    [ContextMenu("Test Phase 2")] void TestPhase2() => SetPhase(2);
    [ContextMenu("Test Phase 3")] void TestPhase3() => SetPhase(3);
}
