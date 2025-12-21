using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    public static BeatManager Instance;

    [Header("Settings")]
    [SerializeField] private float _bpm = 120f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private List<Intervals> _intervals;

    public float CurrentBeat { get; private set; }

    // [추가] 음악이 시작되었는지 확인하는 변수
    public bool IsMusicStarted { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // [중요] 시작하자마자 재생되지 않도록 방지
        if (_audioSource != null)
        {
            _audioSource.Stop();
            _audioSource.playOnAwake = false; // 코드로 확실하게 끕니다.
        }
    }

    // [추가] 외부(타일)에서 호출할 재생 함수
    public void PlayMusic()
    {
        if (_audioSource != null && !IsMusicStarted)
        {
            _audioSource.Play();
            IsMusicStarted = true;
            Debug.Log("Music Started by Trigger!");
        }
    }

    private void Update()
    {
        // 음악이 재생 중이 아니면 계산 중단
        if (!_audioSource.isPlaying) return;

        // --- 기존 로직 유지 ---
        float samplesPerBeat = _audioSource.clip.frequency * (60f / _bpm);
        CurrentBeat = _audioSource.timeSamples / samplesPerBeat;

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