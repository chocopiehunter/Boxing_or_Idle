using UnityEngine;

[System.Serializable]
public class TrainingFacilityData : GameDataBase
{
    public string Type;
    public int Level;
    public string Name;
    public string Description;
    public int GoldCost;
    public string RequiredUnlockIds;
    public string TrainingDataId;
    public string NextLevelId;
    public float TrainingStaminaPerSecond;

    public float Hp;
    public float Stamina;
    public float StandingOffense;
    public float StandingDefense;
    public float WrestlingOffense;
    public float WrestlingDefense;
    public float JiuJitsuOffense;
    public float JiuJitsuDefense;
}
