using UnityEngine;

public class MusicStartTrigger : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("충돌 후 이 오브젝트를 숨길까요?")]
    public bool hideOnTrigger = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어인지 확인 (Tag가 Player인지 확인하거나, 스크립트로 확인)
        if (collision.CompareTag("Player") || collision.GetComponent<RhythmMovement>() != null)
        {
            // 음악이 아직 시작 안 됐다면 실행!
            if (BeatManager.Instance != null && !BeatManager.Instance.IsMusicStarted)
            {
                BeatManager.Instance.PlayMusic();

                // (선택사항) 정확한 싱크를 위해 플레이어 위치 보정 코드를 여기에 넣을 수도 있음
                // 예: collision.transform.position = this.transform.position; 
            }

            // 타일 숨기기 (또 밟아서 재실행되는 것 방지)
            if (hideOnTrigger)
            {
                gameObject.SetActive(false);
            }
        }
    }
}