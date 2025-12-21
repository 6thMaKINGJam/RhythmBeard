using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

public class OpeningController : MonoBehaviour
{
    public PlayableDirector director;       // 타임라인 감독
    public GameObject startButton;          // [추가] 나중에 켜줄 버튼
    public string nextSceneName = "MainMenu"; // 이동할 씬 이름


    [Header("Audio Settings")]
    public AudioClip openingBGM;
    private AudioSource openingSource;
    void OnEnable()
    {
        // 감독님 찾기 (혹시 연결 안 됐을까 봐 안전장치)
        if (director == null) director = GetComponentInChildren<PlayableDirector>();

        // "끝나면 버튼 보여줘(ShowButton)!" 라고 등록
        if (director != null) director.stopped += ShowButton;
    }

    // 1. 타임라인이 끝나면 호출되는 함수
    void ShowButton(PlayableDirector obj)
    {
        // 숨겨뒀던 버튼을 짠! 하고 켭니다.
        startButton.SetActive(true);
    }

    // 2. 버튼을 누르면 호출될 함수 (직접 연결해야 함)
    public void OnClickStartButton()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDisable()
    {
        if (director != null) director.stopped -= ShowButton;
    }
    void Start()
    {
        openingSource = GetComponent<AudioSource>();
    }
}