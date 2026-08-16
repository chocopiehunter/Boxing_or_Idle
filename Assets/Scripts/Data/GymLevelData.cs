using UnityEngine;

[System.Serializable]
public class GymLevelData : GameDataBase
{
    public string Type;
    public int Level;
    public string Name;
    public string Description;
    public int GoldCost;
    public string RequiredItem;
    public string NextLevelId;
}
