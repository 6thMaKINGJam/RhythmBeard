using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float delay = 0.5f; // 애니메이션 길이만큼 설정 (보통 0.3~0.5초)

    void Start()
    {
        // 태어나자마자 자신의 삭제 타이머를 켭니다.
        Destroy(gameObject, delay);
    }
}