using UnityEngine;

[System.Serializable]
public class MatchStrategyData : GameDataBase
{
    public string Name;
    public string Description;
    public bool IsDefault;
    public int SortOrder;
    public float ActionSelectionWeight;
    public float StrikeSelectionWeight;
    public float WrestlingSelectionWeight;
    public float JiuJitsuSelectionWeight;
}
