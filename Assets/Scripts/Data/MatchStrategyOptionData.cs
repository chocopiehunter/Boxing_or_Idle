using UnityEngine;

[System.Serializable]
public class MatchStrategyOptionData : GameDataBase
{
    public string Name;
    public string Description;
    public string ParentOptionId;
    public string ActionType;
    public string StrategyId;
    public int SortOrder;
}

public static class MatchStrategyOptionActionType
{
    public const string OpenSubOptions = "OpenSubOptions";
    public const string ApplyStrategy = "ApplyStrategy";
    public const string KeepCurrent = "KeepCurrent";
    public const string Disabled = "Disabled";
}
