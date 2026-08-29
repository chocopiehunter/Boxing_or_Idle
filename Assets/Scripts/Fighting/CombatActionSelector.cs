using System.Collections.Generic;
using UnityEngine;

public class CombatActionSelector
{
    public bool TrySelectAction()
    {
        return true;
    }

    private MatchFighterSide SelectSkillUserSide()
    {
        return MatchFighterSide.None;
    }

    private List<SkillData> GetUsableSkills()
    {
        return null;
    }

    private SkillData SelectSkill(List<SkillData> usableSkills)
    {
        return null;
    }

    private MatchFighterSide GetTargetSide()
    {
        return MatchFighterSide.None;
    }
}
