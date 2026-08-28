using UnityEngine;

[System.Serializable]
public class SkillData : GameDataBase
{
    public string Name;
    public string Description;
    public string Category;
    public bool IsBasicSkill;
    public float StaminaCost;
    public float StaminaCostPerSecond;
    public float CoolTime;
    public string RequiredUnlockIds;
}
