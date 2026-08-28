using UnityEngine;

[System.Serializable]
public class MatchRuleData : GameDataBase
{
    public string Name;
    public int RoundCount;
    public float RoundSeconds;
    public float RoundBreakSeconds;
    public float ActionIntervalSeconds;
    public int MinOpponentRank;
    public int MaxOpponentRank;
    public bool IncludeChampion;
    public bool IncludeUnranked;
}
