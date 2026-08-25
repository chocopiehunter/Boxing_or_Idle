using UnityEngine;

[System.Serializable]
public class SkillData : GameDataBase
{
    public string Name;
    public string Description;
    public string Category;
    public float StaminaCost;
    public float StaminaCostPerSecond;
    public string RequiredUnlockIds;
}
