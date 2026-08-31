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
        if (skillData == null)
        {
            return false;
        }

        if (string.IsNullOrEmpty(skillData.Id))
        {
            return false;
        }

        if (skillData.CoolTime < 0f)
        {
            return false;
        }

        if (IsSkillReady(skillData.Id) == false)
        {
            return false;
        }

        if (skillData.CoolTime == 0f)
        {
            return true;
        }

        _remainingSecondsBySkillId.Add(skillData.Id, skillData.CoolTime);
        _coolingSkillIds.Add(skillData.Id);

        return true;
    }

    public void UpdateCooldown(float passedSeconds)
    {
        if (passedSeconds <= 0f)
        {
            return;
        }

        for (int index = _coolingSkillIds.Count - 1; index >= 0; index--)
        {
            string skillId = _coolingSkillIds[index];
            float remainingSeconds = _remainingSecondsBySkillId[skillId];
            remainingSeconds = remainingSeconds - passedSeconds;

            if (remainingSeconds <= 0f)
            {
                _remainingSecondsBySkillId.Remove(skillId);
                _coolingSkillIds.RemoveAt(index);
                continue;
            }

            _remainingSecondsBySkillId[skillId] = remainingSeconds;
        }
    }

    public float GetRemainingSeconds(string skillId)
    {
        if (string.IsNullOrEmpty(skillId))
        {
            return 0f;
        }

        if (_remainingSecondsBySkillId.ContainsKey(skillId) == false)
        {
            return 0f;
        }

        return _remainingSecondsBySkillId[skillId];
    }

    public void Reset()
    {
        _remainingSecondsBySkillId.Clear();
        _coolingSkillIds.Clear();
    }
}
