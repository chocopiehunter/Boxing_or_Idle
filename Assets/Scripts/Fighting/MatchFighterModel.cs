using System.Collections.Generic;
using UnityEngine;

public class MatchFighterModel
{
    public MatchFighterSide FighterSide { get; private set; }

    private List<string> _ownedSkillIds;

    private MatchCoolTimeModel _coolTimeModel;

    public IReadOnlyList<string> OwnedSkillIds
    {
        get
        {
            return _ownedSkillIds;
        }
    }

    public MatchFighterModel(MatchFighterSide fighterSide, IReadOnlyList<string> ownedSkillIds)
    {
        FighterSide = fighterSide;
        _ownedSkillIds = new List<string>();
        _coolTimeModel = new MatchCoolTimeModel();

        if (ownedSkillIds == null)
        {
            return;
        }

        for (int index = 0; index < ownedSkillIds.Count; index++)
        {
            string skillId = ownedSkillIds[index];

            if (string.IsNullOrEmpty(skillId))
            {
                continue;
            }

            if (skillId == "None")
            {
                continue;
            }

            if (_ownedSkillIds.Contains(skillId))
            {
                continue;
            }

            _ownedSkillIds.Add(skillId);
        }

    }

    public bool HasSkill(string skillId)
    {
        if (string.IsNullOrEmpty(skillId))
        {
            return false;
        }

        return _ownedSkillIds.Contains(skillId);
    }

    public bool IsSkillReady(string skillId)
    {
        if (HasSkill(skillId) == false)
        {
            return false;
        }

        return _coolTimeModel.IsSkillReady(skillId);
    }

    public bool TryStartSkillCooldown(SkillData skillData)
    {
        if (skillData == null)
        {
            return false;
        }

        if (HasSkill(skillData.Id) == false)
        {
            return false;
        }

        return _coolTimeModel.TryStartSkillCooldown(skillData);
    }

    public void UpdateCooldown(float passedSeconds)
    {
        _coolTimeModel.UpdateCooldown(passedSeconds);
    }

    public float GetRemainingCoolTimeSeconds(string skillId)
    {
        return _coolTimeModel.GetRemainingSeconds(skillId);
    }

    public void ResetCoolTimes()
    {
        _coolTimeModel.Reset();
    }
}
