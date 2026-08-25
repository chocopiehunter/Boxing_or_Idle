using System.Collections.Generic;
using UnityEngine;

public class GymModel
{
    public int Gold { get; private set; }

    private readonly Dictionary<string, string> _currentLevelIds = new Dictionary<string, string>();

    private readonly Dictionary<string, string> _currentFacilityIds = new Dictionary<string, string>();

    public GymModel (int startGold)
    {
        Gold = startGold;
    }

    public string GetLevelId(string type)
    {
        if (_currentLevelIds.TryGetValue(type, out string levelId) == true)
        {
            return levelId;
        }

        return null;
    }

    public void ApplyLevelData(GymLevelData data)
    {
        if (data == null)
        {
            return;
        }

        _currentLevelIds[data.Type] = data.Id;
    }

    public string GetFacilityId(string facilityType)
    {
        if (string.IsNullOrEmpty(facilityType) == true)
        {
            return null;
        }

        if (_currentFacilityIds.TryGetValue(facilityType, out string facilityId) == true)
        {
            return facilityId;
        }

        return null;
    }

    public bool HasFacility(string facilityType)
    {
        return string.IsNullOrEmpty(GetFacilityId(facilityType)) == false;
    }

    public void ApplyFacilityData(TrainingFacilityData facilityData)
    {
        if (facilityData == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(facilityData.Type) == true)
        {
            return;
        }

        _currentFacilityIds[facilityData.Type] = facilityData.Id;
    }

    public List<string> GetOwnedFacilityIds()
    {
        return new List<string>(_currentFacilityIds.Values);
    }

    public void AddGold(int amount)
    {
        Gold = Gold + amount;
    }

    public bool TrySpendGold(int amount)
    {
        if (amount < 0)
        {
            return false;
        }

        if (Gold < amount)
        {
            return false;
        }

        Gold = Gold - amount;
        return true;
    }
}
