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
}
