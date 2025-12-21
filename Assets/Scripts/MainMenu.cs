using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // 1. 설명창(패널)을 연결할 변수 추가
    public GameObject helpPanel; 

    public void StartGame()
    {
        // "InGame" 부분에 실제 본 게임 씬 이름을 적기
        SceneManager.LoadScene("hs_scene"); 
    }

    public void QuitGame()
    {
        Application.Quit(); // 게임 종료 (빌드된 파일에서만 작동)
        Debug.Log("게임을 종료합니다.");
    }

    // 2. 설명창 여는 함수 (설명 버튼에 연결)
    public void OpenHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(true); // 패널 켜기
        }
    }

    // 3. 설명창 닫는 함수 (닫기/X 버튼에 연결)
    public void CloseHelp()
    {
        if (helpPanel != null)
        {
            helpPanel.SetActive(false); // 패널 끄기
        }
    }
}