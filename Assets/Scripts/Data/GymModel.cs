using UnityEngine;

public class GymModel
{
    public int Level { get; private set; }
    public int Gold { get; private set; }

    public GymModel (int startLevel, int startGold)
    {
        Level = startLevel;
        Gold = startGold;
    }

    public void SetLevel(int level)
    {
        Level = level;
    }

    public void AddGold(int amount)
    {
        Gold = Gold + amount;

    }

    
}
