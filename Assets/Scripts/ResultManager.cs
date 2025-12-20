using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [Header("UI 연결")]
    public RectTransform resultWindow;   // 결과창 패널 (평소엔 숨겨짐)

    // [수정] 텍스트를 두 개로 분리했습니다.
    public TextMeshProUGUI ingameDistanceText; // 게임 중에 계속 떠 있을 텍스트 (화면 상단 등)
    public TextMeshProUGUI resultDistanceText; // 결과창 패널 안에 있는 텍스트 (게임 끝나고 보여줌)

    [Header("플레이어 설정")]
    public Transform player;             // 플레이어 오브젝트

    private float startPosX;
    private bool isGameOver = false;
    private float currentDist = 0f;      // 현재 거리를 저장할 변수

    void Start()
    {
        // [추가] 재시작 시 시간 정지 해제 (안전장치)
        Time.timeScale = 1f;

        if (player != null) startPosX = player.position.x;
        if (resultWindow != null)
            resultWindow.anchoredPosition = new Vector2(0, -1000); // 시작 시 화면 아래로 숨김
    }

    void Update()
    {
        // 게임 오버가 아니고 플레이어가 있다면 실시간 갱신
        if (!isGameOver && player != null)
        {
            currentDist = player.position.x - startPosX;
            currentDist = Mathf.Max(0, currentDist); // 0보다 작아지지 않게 방지

            // [수정] 게임 중인 화면(ingameDistanceText)에 실시간 반영
            if (ingameDistanceText != null)
            {
                ingameDistanceText.text = "Distance: " + currentDist.ToString("F1") + "m";
            }
        }
    }

    public void ShowResult()
    {
        if (isGameOver) return;
        isGameOver = true;

        // [추가] 게임이 끝나는 순간, 최종 기록을 결과창 텍스트(resultDistanceText)에도 복사
        if (resultDistanceText != null)
        {
            resultDistanceText.text = "Distance: " + currentDist.ToString("F1") + "m";
        }

        // 인게임 텍스트는 가려주거나 끄고 싶다면 아래 코드 주석 해제
        // if (ingameDistanceText != null) ingameDistanceText.gameObject.SetActive(false);

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
            // [수정] UI 애니메이션은 게임 시간이 멈춰도(TimeScale=0) 작동하도록 unscaledDeltaTime 사용
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