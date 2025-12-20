using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float parallaxEffect; // 0이면 카메라와 같이 움직임, 1이면 고정
    
    private Transform cameraTransform;
    private float startPositionX;

    private void Start()
    {
        // 싱글톤으로 만든 카메라의 트랜스폼 참조
        cameraTransform = MainCamera_Action.Instance.transform;
        startPositionX = transform.position.x;
    }

    private void LateUpdate()
    {
        // 카메라가 움직인 거리 계산
        float distance = cameraTransform.position.x * parallaxEffect;
        
        // 내 위치를 카메라 이동량에 비례해서 이동
        transform.position = new Vector3(startPositionX + distance, transform.position.y, transform.position.z);
    }
}
