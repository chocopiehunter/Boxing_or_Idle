using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SeasonIconTable", menuName = "GameData/SeasonIconTable")]
public class SeasonIconTable : ScriptableObject
{
    [Serializable]
    public class SeasonIconEntry
    {
        public Season Season;
        public string IconAddress;
    }

    [SerializeField] private SeasonIconEntry[] Entries;

    public string GetIconAddress(Season season)
    {
        foreach (SeasonIconEntry entry in Entries)
        {
            if (entry.Season == season)
            {
                return entry.IconAddress;
            }
        }

        Debug.LogError($"계절 아이콘 주소가 없습니다 {season}");
        return string.Empty;
    }
}
