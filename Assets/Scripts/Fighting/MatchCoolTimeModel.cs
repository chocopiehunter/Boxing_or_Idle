using System.Collections.Generic;
using UnityEngine;

public class MatchCoolTimeModel
{
    private Dictionary<string, float> _remainingSecondsBySkillId;
    private List<string> _coolingSkillIds;

    public MatchCoolTimeModel()
    {
        _remainingSecondsBySkillId = new Dictionary<string, float>();
        _coolingSkillIds = new List<string>();
    }

    public bool IsSkillReady(string skillId)
    {
        if (string.IsNullOrEmpty(skillId))
        {
            return false;
        }

        return _remainingSecondsBySkillId.ContainsKey(skillId) == false;
    }

    public bool TryStartSkillCooldown(SkillData skillData)
    {


        return true;
    }

    public void UpdateCooldown(float passedSeconds)
    {

    }

    public float GetRemainingSeconds(string skillId)
    {

        return _remainingSecondsBySkillId[skillId];
    }

    public void Reset()
    {
        _remainingSecondsBySkillId.Clear();
        _coolingSkillIds.Clear();
    }
}
