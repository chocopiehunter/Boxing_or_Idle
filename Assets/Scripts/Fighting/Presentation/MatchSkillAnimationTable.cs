using System.Collections.Generic;
using UnityEngine;

public enum MatchFighterAnimationType
{
    None,
    Jab,
    HighKick
}

[System.Serializable]
public class MatchSkillAnimationEntry
{
    public string SkillId;
    public MatchFighterAnimationType AnimationType;
    [Min(0f)] public float ImpactDelay;
}


[CreateAssetMenu(fileName = "MatchSkillAnimationTable", menuName = "Scriptable Objects/MatchSkillAnimationTable")]
public class MatchSkillAnimationTable : ScriptableObject
{
    [SerializeField] private List<MatchSkillAnimationEntry> Entries = new List<MatchSkillAnimationEntry>();

    public bool TryGetEntry(string skillId, out MatchSkillAnimationEntry entry)
    {
        entry = null;

        if (string.IsNullOrEmpty(skillId) || Entries == null)
        {
            return false;
        }

        for (int i = 0; i < Entries.Count; i++)
        {
            MatchSkillAnimationEntry candidate = Entries[i];

            if (candidate != null && candidate.SkillId == skillId)
            {
                entry = candidate;
                return true;
            }
        }

        return false;
    }
}
