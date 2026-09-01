using UnityEngine;

[System.Serializable]
public class SkillData : GameDataBase
{
    public string Name;
    public string Description;
    public string Category;
    public string ActionType;
    public bool IsBasicSkill;
    public float StaminaCost;
    public float StaminaCostPerSecond;
    public float CoolTime;
    public string RequiredUnlockIds;
    public float BaseSuccessChance;
    public float DamageMultiplier;
    public string TargetGroundPosition;
    public bool ChangeTopBottom;
}

public static class SkillCategoryType
{
    public const string Strike = "Strike";
    public const string Wrestling = "Wrestling";
    public const string JiuJitsu = "JiuJitsu";
}

public static class SkillActionType
{
    public const string Strike = "Strike";
    public const string GroundStrike = "GroundStrike";
    public const string Takedown = "Takedown";
    public const string ClinchEntry = "ClinchEntry";
    public const string Submission = "Submission";
    public const string Escape = "Escape";
    public const string PositionChange = "PositionChange";
}
