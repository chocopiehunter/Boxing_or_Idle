using UnityEngine;

[System.Serializable]
public class GymLevelData : GameDataBase
{
    public string Type;
    public int Level;
    public string Name;
    public string Description;
    public string FacilityIds;
    public int GoldCost;
    public string RequiredUnlockIds;
    public string NextLevelId;
}
