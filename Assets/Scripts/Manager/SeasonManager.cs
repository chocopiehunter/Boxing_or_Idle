using System;
using UnityEngine;

public class SeasonManager : MonoBehaviour
{
    public static SeasonManager Instance { get; set; }

    public const int MonthsPerSeason = 3;
    public const int MonthsPerYear = 12;

    public int Year { get; private set; } = 1;
    public Season CurrentSeason { get; private set; } = Season.Spring;
    public int CurrentMonth { get; private set; } = 3;


    public event Action OnMonthAdvanced;

    private void Awake()
    {
        Instance = this;

        CurrentSeason = GetSeasonByMonth(CurrentMonth);
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

    // AdvanceMonth에서 안쓰여서 삭제대기
    //public void AdvanceSeason()
    //{
    //    if (CurrentSeason == Season.Winter)
    //    {
    //        CurrentSeason = Season.Spring;
    //        Year++;
    //    }
    //    else
    //    {
    //        CurrentSeason++;
    //    }
    //}

    private Season GetSeasonByMonth(int month)
    {
        if (month >= 3 && month <= 5) return Season.Spring;
        if (month >= 6 && month <= 8) return Season.Summer;
        if (month >= 9 && month <= 11) return Season.Fall;
        return Season.Winter;
    }


    [ContextMenu("한달 진행 테스트")]
    private void TestAdvanceWeek()
    {
        Debug.Log($"한달 테스트 진행");
        AdvanceMonth();
    }
}
