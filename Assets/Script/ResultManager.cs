using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
   [Header("UI 연결")]
    public RectTransform resultWindow;   // ResultWindow 패널 (RectTransform)
    public TextMeshProUGUI distanceText; // "지나간 거리"를 표시할 텍스트

    [Header("플레이어 설정")]
    public Transform player;             // 플레이어 오브젝트
    
    private float startPosX;
    private bool isGameOver = false;
    // Start is called before the first frame update
    void Start()
    {
        if (player != null) startPosX = player.position.x;
        if (resultWindow != null)
            resultWindow.anchoredPosition = new Vector2(0, -1000);
    }

    // Update is called once per frame
    void Update()
    {
       if (!isGameOver && player != null)
    {
        float currentDist = player.position.x - startPosX;

        distanceText.text = "지나간 거리 : " + Mathf.Max(0, currentDist).ToString("F1") + "m";
    }
    }

    public void ShowResult()
    {
      if (isGameOver) return; // 이미 실행 중이면 무시
        isGameOver = true;

        // 아래에서 위로 올라오는 코루틴 실행
        StartCoroutine(AppearRoutine());
    }
    IEnumerator AppearRoutine()
    {
        Vector2 startPos = new Vector2(0, -1000); // 화면 아래 숨겨진 위치
        Vector2 endPos = Vector2.zero;            // 화면 중앙 위치 (0, 0)
        
        float duration = 0.6f; // 올라오는 데 걸리는 시간 (0.6초)
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;
            
            // Lerp를 이용해 부드럽게 이동 (끝에서 살짝 느려지는 효과)
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
