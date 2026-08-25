using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrganizationData : GameDataBase
{
    public string Name;
    public string Description;
    public string FighterIds;
    public string InitialChampionId;
    public string InitialRankedIds;

    public List<string> GetFighterIdList()
    {
        List<string> result = new List<string>();

        if (string.IsNullOrEmpty(FighterIds)) 
        {
            return result;
        }

        string[] splitIds = FighterIds.Split(',');
        for (int i = 0; i < splitIds.Length; i++)
        {
            string id = splitIds[i].Trim();
            if (string.IsNullOrEmpty(id) == false)
            {
                result.Add(id);
            }
        }

        return result;
    }
}
