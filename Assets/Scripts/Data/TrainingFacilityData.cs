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
    public float Standing_Atk;
    public float Standing_Def;
    public float MatchStamina;
}
