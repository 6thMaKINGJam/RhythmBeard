using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Image 사용을 위해 필수

public class PlayerHeart : MonoBehaviour
{
    [Header("UI 연결")]
    public Image[] heartImages;    // 인스펙터에서 하트 이미지 3개 연결
    public Sprite fullHeart;       // 꽉 찬 하트 그림
    public Sprite emptyHeart;      // 빈 하트 그림

    // PlayerHealth에서 호출할 함수 (이게 없어서 에러가 났었습니다)
    public void UpdateHearts(int currentHealth)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < currentHealth)
            {
                // 체력만큼 꽉 찬 하트 보여줌
                heartImages[i].sprite = fullHeart;
            }
            else
            {
                // 체력 잃은 만큼 빈 하트 보여줌
                heartImages[i].sprite = emptyHeart;
            }
        }
    }
}