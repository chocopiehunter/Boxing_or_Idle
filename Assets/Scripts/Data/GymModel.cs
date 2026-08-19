using System.Collections.Generic;
using UnityEngine;

public class GymModel
{
    public int Gold { get; private set; }

    private Dictionary<string, string> _currentLevelIds = new Dictionary<string, string>();

    public GymModel (int startGold)
    {
        Gold = startGold;
    }

    public string GetLevelId(string type)
    {
        string levelId;
        if (_currentLevelIds.TryGetValue(type, out levelId) == true)
        {
            return levelId;
        }

        return null;
    }

    public void ApplyLevelData(GymLevelData data)
    {
        _currentLevelIds[data.Type] = data.Id;
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
