using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance; // 어디서든 접근 가능하게 싱글톤 처리

    [Header("Settings")]
    [SerializeField] private float _bpm = 120f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<Intervals> _intervals;

    // 점프 앤 런 이동을 위해 현재 노래가 몇 박자만큼 진행됐는지 실수형으로 제공
    public float CurrentBeat { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // [추가] 오디오 소스가 연결되어 있다면 강제로 재생!
        if (_audioSource != null)
        {
            _audioSource.Stop(); // 혹시 꼬였을까봐 한번 멈췄다가
            _audioSource.Play(); // 다시 재생
            Debug.Log("코드에서 강제로 음악을 재생했습니다!");
        }
        else
        {
            Debug.LogError("Audio Source가 연결되지 않았습니다!");
        }
    }

    private void Update()
    {
        // 오디오가 재생 중일 때만 계산
        if (!_audioSource.isPlaying) return;

        // 1. 현재 박자 위치 계산 (전체 노래 기준)
        // 공식: 현재샘플 / (샘플레이트 * (60/BPM)) = 현재까지 진행된 총 박자 수
        float samplesPerBeat = _audioSource.clip.frequency * (60f / _bpm);
        CurrentBeat = _audioSource.timeSamples / samplesPerBeat;

        // 2. 각 인터벌(4분음표, 8분음표 등) 이벤트 체크
        foreach (var interval in _intervals)
        {
            interval.CheckForNewInterval(CurrentBeat);
        }
    }
}

[System.Serializable]
public class Intervals
{
    [Tooltip("1 = 1박자(4분음표), 0.5 = 2박자(2분음표), 2 = 반박자(8분음표)")]
    [SerializeField] private float _steps = 1f;
    [SerializeField] private UnityEvent _trigger;

    private int _lastInterval;

    public void CheckForNewInterval(float currentSongBeat)
    {
        if (_steps == 0) return; // 0 나누기 방지

        // 현재 박자를 스텝(간격)으로 나누어 계산
        // 예: 120BPM 곡에서 0.5박자(8분음표)마다 이벤트를 원하면 steps = 2
        float intervalValue = currentSongBeat * _steps;
        int currentFloor = Mathf.FloorToInt(intervalValue);

        if (currentFloor != _lastInterval)
        {
            _lastInterval = currentFloor;
            _trigger.Invoke(); // 이벤트 발생! (장애물 생성, 박자 UI 깜빡임 등)
        }
    }
}