using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [Header("UI 연결")]
    public RectTransform resultWindow;    // 결과창 패널

    // [수정] 텍스트를 두 개로 분리하여 연결
    public TextMeshProUGUI ingameDistanceText; // (New) 게임 중 화면 상단에 계속 뜨는 텍스트
    public TextMeshProUGUI resultDistanceText; // (New) 게임 오버 후 결과창 안에 뜨는 텍스트

    [Header("플레이어 설정")]
    public Transform player;              // 플레이어 오브젝트

    private float startPosX;
    private bool isGameOver = false;
    private float currentDist = 0f;       // 거리를 저장해둘 멤버 변수

    void Start()
    {
        // 재시작 시 시간이 멈춰있지 않도록 초기화
        Time.timeScale = 1f;

        if (player != null) startPosX = player.position.x;
        if (resultWindow != null)
            resultWindow.anchoredPosition = new Vector2(0, -1000);
    }

    void Update()
    {
        if (!isGameOver && player != null)
        {
            // 거리 계산
            currentDist = player.position.x - startPosX;
            currentDist = Mathf.Max(0, currentDist);

            // [요청 반영] 인게임 텍스트에 실시간 거리 표시
            if (ingameDistanceText != null)
            {
                // 한글 깨짐 방지를 위해 영어 "Distance" 사용
                ingameDistanceText.text = "Distance: " + currentDist.ToString("F1") + "m";
            }
        }
    }

    public void ShowResult()
    {
        if (isGameOver) return;
        isGameOver = true;

        // 게임 오버 시 결과창 텍스트에 '최종 거리' 한 번 딱 입력
        if (resultDistanceText != null)
        {
            resultDistanceText.text = "Final Distance: " + currentDist.ToString("F1") + "m";
        }

        // 아래에서 위로 올라오는 코루틴 실행
        StartCoroutine(AppearRoutine());
    }

    IEnumerator AppearRoutine()
    {
        Vector2 startPos = new Vector2(0, -1000);
        Vector2 endPos = Vector2.zero;

        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // UI는 게임 시간이 멈춰도 움직여야 하므로 unscaledDeltaTime 사용
            elapsed += Time.unscaledDeltaTime;
            float percent = elapsed / duration;

            resultWindow.anchoredPosition = Vector2.Lerp(startPos, endPos, Mathf.SmoothStep(0, 1, percent));
            yield return null;
        }

        resultWindow.anchoredPosition = endPos;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("hs_scene");
    }
    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}