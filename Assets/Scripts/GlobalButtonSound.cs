using UnityEngine;
using UnityEngine.UI; // 버튼 기능을 위해 필수

public class GlobalButtonSound : MonoBehaviour
{
    [Header("Settings")]
    public AudioClip clickSound; // 여기에 효과음 파일 넣기
    [Range(0f, 1f)] public float volume = 1.0f;

    private AudioSource audioSource;

    void Start()
    {
        // 1. 소리를 낼 스피커(AudioSource)를 내 몸에 자동 생성
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 무조건 2D 사운드 (크게 들림)

        // 2. 내 자식들 중에 있는 "모든 버튼"을 다 찾아냄 (꺼져있는 것도 포함)
        Button[] allButtons = GetComponentsInChildren<Button>(true);

        // 3. 찾은 버튼 하나하나에 "클릭하면 소리내!" 명령을 추가함
        foreach (Button btn in allButtons)
        {
            btn.onClick.AddListener(PlaySound);
        }

        Debug.Log($"총 {allButtons.Length}개의 버튼에 효과음을 장착했습니다!");
    }

    // 실제 소리 재생 함수
    void PlaySound()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, volume);
        }
    }
}