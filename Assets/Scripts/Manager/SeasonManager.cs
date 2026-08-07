using System;
using UnityEngine;

public class SeasonManager : MonoBehaviour
{
    public static SeasonManager Instance { get; set; }

    public const int WeeksPerSeason = 4;

    public int Year { get; private set; } = 1;
    public Season CurrentSeason { get; private set; } = Season.Spring;
    public int CurrentWeek { get; private set; } = 1;


    public event Action OnWeekAdvanced;

    private void Awake()
    {
        Instance = this;
    }

    public void AdvanceWeek()
    {
        CurrentWeek++;
        
        if(CurrentWeek > WeeksPerSeason)
        {
            CurrentWeek = 1;
            AdvanceSeason();
        }

        OnWeekAdvanced?.Invoke();
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
}
