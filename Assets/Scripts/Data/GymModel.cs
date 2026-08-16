using UnityEngine;

public class GymModel
{
    public string LevelId { get; private set; }
    public int Level { get; private set; }
    public int Gold { get; private set; }

    public GymModel (GymLevelData data, int startGold)
    {
        LevelId = data.Id;
        Level = data.Level;
        Gold = startGold;
    }

    public void ApplyLevelData(GymLevelData data)
    {
        LevelId = data.Id;
        Level = data.Level;
    }

    public void AddGold(int amount)
    {
        Gold = Gold + amount;

    }

    public bool TrySpendGold(int amount)
    {
        if (Gold < amount)
        {
            return false;
        }

        Gold = Gold - amount;
        return true;
    }
}
