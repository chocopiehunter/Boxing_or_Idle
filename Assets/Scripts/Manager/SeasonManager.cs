using System;
using UnityEngine;

public class SeasonManager : MonoBehaviour
{
    public static SeasonManager Instance { get; set; }

    public const int MonthsPerSeason = 3;

    public int Year { get; private set; } = 1;
    public Season CurrentSeason { get; private set; } = Season.Spring;
    public int CurrentMonth { get; private set; } = 1;


    public event Action OnMonthAdvanced;

    private void Awake()
    {
        Instance = this;
    }

    public void AdvanceMonth()
    {
        CurrentMonth++;
        
        if(CurrentMonth > MonthsPerSeason)
        {
            CurrentMonth = 1;
            AdvanceSeason();
        }

        OnMonthAdvanced?.Invoke();
    }

    public void AdvanceSeason()
    {
        if (CurrentSeason == Season.Winter)
        {
            CurrentSeason = Season.Spring;
            Year++;
        }
        else
        {
            CurrentSeason++;
        }
    }


    [ContextMenu("한달 진행 테스트")]
    private void TestAdvanceWeek()
    {
        Debug.Log($"한달 테스트 진행");
        AdvanceMonth();
    }
}
