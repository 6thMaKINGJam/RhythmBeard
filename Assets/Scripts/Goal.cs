using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    [Header("Settings")]
    public string endingSceneName = "EndingScene";
    public Sprite successSprite;
    public AudioClip successSound;
    public float floatHeight = 1.2f;
    public float floatDuration = 0.8f;

    private bool isReached = false;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 태그가 "Player"인지 확인
        // 2. 이미 도착했는지 확인 (중복 실행 방지)
        if (collision.CompareTag("Player") && !isReached)
        {
            // PlayerHealth 스크립트를 가져와서 죽었는지 확인
            PlayerHealth health = collision.GetComponent<PlayerHealth>();
            
            if (health != null && !health.isDead)
            {
                isReached = true;
                StartCoroutine(SuccessSequence(collision.gameObject, health));
            }
        }
    }

    IEnumerator SuccessSequence(GameObject player, PlayerHealth health)
    {
        // [1단계] 플레이어 조작 즉시 멈춤
        // PlayerHealth에 선언된 movementScript 이름을 그대로 사용합니다.
        if (health.movementScript != null) health.movementScript.enabled = false;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // 중력 끄기
        }

        // [2단계] 1초 대기 (극적인 효과)
        yield return new WaitForSeconds(1.0f);

        // [3단계] 스프라이트 교체 및 효과음
        // 애니메이터가 켜져 있으면 스프라이트 교체가 안 되므로 끕니다.
        Animator anim = player.GetComponent<Animator>();
        if (anim == null) anim = player.GetComponentInChildren<Animator>();
        if (anim != null) anim.enabled = false;

        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
        if (sr != null && successSprite != null)
        {
            sr.sprite = successSprite;
            sr.color = Color.white; // 피격 붉은색 등이 남지 않게 초기화
        }

        if (successSound != null) 
        {
            audioSource.PlayOneShot(successSound);
        }
        // [4단계] 캐릭터 위로 부양
        Vector3 startPos = player.transform.position;
        Vector3 endPos = startPos + new Vector3(0, floatHeight, 0);
        float elapsed = 0;

        while (elapsed < floatDuration)
        {
            player.transform.position = Vector3.Lerp(startPos, endPos, elapsed / floatDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        player.transform.position = endPos;

        // [5단계] 5초 후 엔딩 컷씬으로 전환
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(endingSceneName);
    }
}
