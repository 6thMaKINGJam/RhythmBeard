using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ObjectLinePlacer : MonoBehaviour
{
    [Header("설정")]
    public GameObject prefab;      // 배치할 프리팹
    public float startX = 0f;      // 시작 X 위치
    public float endX = 10f;       // 끝 X 위치
    public float intervalX = 1f;   // X축 간격
    public float fixedY = 0f;      // 고정할 Y 위치
    public float fixedZ = 0f;      // 고정할 Z 위치

    [Header("배치 옵션")]
    public Transform parentTransform; // 생성된 오브젝트들을 묶을 부모 (비워두면 이 오브젝트의 자식으로)

    /// <summary>
    /// 에디터에서 호출할 배치 함수
    /// </summary>
    public void PlaceObjects()
    {
        if (prefab == null)
        {
            Debug.LogError("배치할 프리팹이 설정되지 않았습니다!");
            return;
        }

        if (intervalX <= 0)
        {
            Debug.LogError("간격(Interval)은 0보다 커야 합니다.");
            return;
        }

        // 기존 배치된 자식들 정리 (선택 사항)
        // 만약 '딸깍' 할 때마다 새로 배치하고 싶다면 아래 주석을 해제하세요.
        // ClearExistingObjects();

        int count = Mathf.FloorToInt((endX - startX) / intervalX) + 1;
        Transform parent = parentTransform != null ? parentTransform : transform;

        for (int i = 0; i < count; i++)
        {
            float currentX = startX + (i * intervalX);
            
            // 끝 좌표를 초과하면 중단
            if (currentX > endX) break;

            Vector3 position = new Vector3(currentX, fixedY, fixedZ);
            
            // 프리팹 링크를 유지하며 생성 (Editor 전용)
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = position;
            instance.transform.SetParent(parent);

            // 실행 취소(Undo) 등록
            Undo.RegisterCreatedObjectUndo(instance, "Place Objects");
        }

        Debug.Log($"{count}개의 오브젝트가 배치되었습니다.");
    }
}

// --- 에디터 버튼 구현부 ---
#if UNITY_EDITOR
[CustomEditor(typeof(ObjectLinePlacer))]
public class ObjectLinePlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 기본 인스펙터 그리기
        DrawDefaultInspector();

        ObjectLinePlacer script = (ObjectLinePlacer)target;

        GUILayout.Space(10);
        if (GUILayout.Button("오브젝트 배치하기 (딸깍)", GUILayout.Height(30)))
        {
            script.PlaceObjects();
        }
    }
}
#endif