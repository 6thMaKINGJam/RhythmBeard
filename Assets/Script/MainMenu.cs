using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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
}
