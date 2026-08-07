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

        }

        return string.Empty;
    }
}
