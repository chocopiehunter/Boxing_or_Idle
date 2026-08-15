using System;
using UnityEngine;

public class SeasonManager : MonoBehaviour
{
    public static SeasonManager Instance { get; set; }

    public const int MonthsPerSeason = 3;
    public const int MonthsPerYear = 12;
    public const int WeeksPerMonth = 4;

    [SerializeField] private float SecondsPerWeek = 5f;

    public int Year { get; private set; } = 1;
    public Season CurrentSeason { get; private set; } = Season.Spring;
    public int CurrentMonth { get; private set; } = 3;
    public int CurrentWeek { get; private set; } = 1;

    public float WeekProgress { get; private set; } = 0f; // 슬라이더용

    public event Action OnWeekAdvanced;
    public event Action OnMonthAdvanced;
    public event Action<float> OnWeekProgressChanged;

    private float _elapsedSecondsInWeek;

    private void Awake()
    {
        Instance = this;

        CurrentSeason = GetSeasonByMonth(CurrentMonth);
    }

    private void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if(GameManager.Instance.GameState.CurrentState != GameFlowState.Play)
        {
            return;
        }

        float speedMultiplier = GetSpeedMultiplier(GameManager.Instance.GameState.CurrentSpeed);
        _elapsedSecondsInWeek += Time.deltaTime * speedMultiplier;

        WeekProgress = Mathf.Clamp01(_elapsedSecondsInWeek / SecondsPerWeek);
        OnWeekProgressChanged?.Invoke(WeekProgress);

        if (_elapsedSecondsInWeek >= SecondsPerWeek)
        {
            _elapsedSecondsInWeek = 0f;
            WeekProgress = 0f;
            OnWeekProgressChanged?.Invoke(WeekProgress);
            AdvanceWeek();
        }
    }

    public void AdvanceWeek()
    {
        CurrentWeek++;

        if(CurrentWeek > WeeksPerMonth)
        {
            CurrentWeek = 1;
            AdvanceMonth();
        }

        OnWeekAdvanced?.Invoke();
    }

    public void AdvanceMonth()
    {
        CurrentMonth++;
        
        if(CurrentMonth > MonthsPerYear)
        {
            CurrentMonth = 1;
            Year++;
        }

        CurrentSeason = GetSeasonByMonth(CurrentMonth);
        OnMonthAdvanced?.Invoke();
    }

    private Season GetSeasonByMonth(int month)
    {
        if (month >= 3 && month <= 5) return Season.Spring;
        if (month >= 6 && month <= 8) return Season.Summer;
        if (month >= 9 && month <= 11) return Season.Fall;
        return Season.Winter;
    }

    private float GetSpeedMultiplier(GameSpeedType speedType)
    {
        if (speedType == GameSpeedType.Fast)
        {
            return 2f;
        }

        return 1f;
    }
}
