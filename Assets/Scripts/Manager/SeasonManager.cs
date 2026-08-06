using System;
using UnityEngine;

public class SeasonManager : MonoBehaviour
{
    public static SeasonManager Instance { get; set; }

    public int Year { get; private set; } = 1;
    public Season CurrentSeason { get; private set; } = Season.Spring;

    public event Action OnTurnAdvanced;

    private void Awake()
    {
        Instance = this;
    }

    public void AdvanceTurn()
    {
        
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
