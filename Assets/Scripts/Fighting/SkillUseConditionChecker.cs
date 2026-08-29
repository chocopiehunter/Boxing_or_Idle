using System.Collections.Generic;
using UnityEngine;

public class SkillUseConditionChecker
{
    public bool CanUseSkill(List<SkillUseConditionData> conditionDataList, MatchCombatModel combatModel, MatchFighterSide skillUserSide)
    {
        if (conditionDataList == null)
        {
            return false;
        }

        if (conditionDataList.Count == 0)
        {
            return false;
        }

        if (combatModel == null)
        {
            return false;
        }

        if (skillUserSide == MatchFighterSide.None)
        {
            return false;
        }


        return false;
    }
}
