using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovements : MonoBehaviour
{
    [Range(0f, 1f)]
    public float parallaxEffect; // 0에 가까울수록 카메라와 같이 가고, 1에 가까울수록 가만히 있음

    private float startPosX;
    private Transform cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main.transform;
        // 스패너가 배치해준 초기 X 위치를 기억합니다.
        startPosX = transform.position.x;
    }

    // CloudSpawner가 구름을 앞으로 옮길 때 호출해줄 함수
    public void ResetPosition(float newX)
    {
        startPosX = newX;
    }

    // Update is called once per frame
    void Update()
    {
        // 카메라가 움직인 거리만큼 비례해서 구름을 움직여 입체감을 줍니다.
        float distance = cam.position.x * parallaxEffect;
        transform.position = new Vector3(startPosX + distance, transform.position.y, transform.position.z);
    }
}
