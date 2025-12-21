using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수!

public class EndingManager : MonoBehaviour
{
    public GameObject restartButton; // 재시작 버튼을 연결할 변수
    public string mainSceneName = "MainScene"; // 이동할 메인 메뉴 씬의 정확한 이름
    public AudioClip endingBGM;
    private AudioSource endingSource;

    void Start()
    {

        endingSource = GetComponent<AudioSource>();
        // 1. 게임 시작 시 버튼을 숨김
        if (restartButton != null)
        {
            restartButton.SetActive(false);
        }

        // 방법 A: 시간으로 제어 (예: 5초 뒤에 버튼 등 장)
        // 스토리가 끝나는 정확한 시간을 안다면 이 숫자를 조절하세요.
        Invoke("ShowButton", 5.0f); 
    }

    // 버튼을 보여주는 함수
    public void ShowButton()
    {
        restartButton.SetActive(true);
    }

    // 버튼 누르면 실행될 함수 (씬 이동)
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}