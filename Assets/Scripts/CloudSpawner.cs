using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [Header("설정")]
    public GameObject[] cloudPrefabs; // 구름 프리팹들
    public int nearCloudCount = 2;
    public int farCloudCount = 2;
    public float spawnInterval = 15f; // 구름 사이의 간격 (X)
    public float minY = 4f;           // 구름이 나타날 최소 높이
    public float maxY = 7f;           // 구름이 나타날 최대 높이

    private GameObject[] cloudPool;
    private float lastSpawnX;

    // Start is called before the first frame update
    void Start()
    {
        int totalPoolSize = nearCloudCount + farCloudCount;
        cloudPool = new GameObject[totalPoolSize];
        float startX = Camera.main.transform.position.x;

        // 1. 어떤 종류의 구름을 만들지 미리 리스트에 담기
        List<int> prefabIndices = new List<int>();
        for (int i = 0; i < nearCloudCount; i++) prefabIndices.Add(0); // Near 인덱스
        for (int i = 0; i < farCloudCount; i++) prefabIndices.Add(1);  // Far 인덱스

        // 2. 리스트 순서 랜덤하게 섞기 (Fisher-Yates Shuffle)
        for (int i = 0; i < prefabIndices.Count; i++)
        {
            int temp = prefabIndices[i];
            int randomIndex = Random.Range(i, prefabIndices.Count);
            prefabIndices[i] = prefabIndices[randomIndex];
            prefabIndices[randomIndex] = temp;
        }

        // 3. 섞인 순서대로 생성
        for (int i = 0; i < totalPoolSize; i++)
        {
            float xPos = startX + (i * spawnInterval);
            GameObject go = Instantiate(cloudPrefabs[prefabIndices[i]], GetRandomPos(xPos), Quaternion.identity);
            cloudPool[i] = go;
            
            BackGroundManager.Instance.RegisterCloud(go.GetComponent<SpriteRenderer>());
        }
        
        lastSpawnX = startX + (totalPoolSize - 1) * spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        float camX = Camera.main.transform.position.x;

        foreach (GameObject cloud in cloudPool)
        {
            // 기준점을 기준으로 판정 (이동 스크립트 때문에 실제 transform.position은 다를 수 있음)
            if (cloud.transform.position.x < camX - 22f) 
            {
                // 고정된 간격 대신 약간의 랜덤값을 더해 공백을 불규칙하게 메움
                float randomOffset = Random.Range(-3f, 3f);
                lastSpawnX += spawnInterval;

                // 1. 높이 랜덤 결정
                float newY = Random.Range(minY, maxY);
                
                // 2. 구름의 위치 리셋 함수 호출 (이게 핵심!)
                CloudMovements moveScript = cloud.GetComponent<CloudMovements>();
                if (moveScript != null)
                {
                    moveScript.ResetPosition(lastSpawnX);
                }
                
                // 3. 실제 y축 높이만 즉시 반영
                cloud.transform.position = new Vector3(cloud.transform.position.x, newY, 0);
            }
        }
    }

    Vector3 GetRandomPos(float x)
    {
        return new Vector3(x, Random.Range(minY, maxY), 0);
    }
}
