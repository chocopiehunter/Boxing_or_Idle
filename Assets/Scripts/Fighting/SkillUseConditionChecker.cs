using System;
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

        for (int index = 0; index < conditionDataList.Count; index++)
        {
            SkillUseConditionData conditionData = conditionDataList[index];

            if (IsConditionMet(conditionData, combatModel, skillUserSide))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsConditionMet(SkillUseConditionData conditionData, MatchCombatModel combatModel, MatchFighterSide skillUserSide)
    {
        if (conditionData == null)
        {
            return false;
        }

        MatchSituation requiredMatchSituation;

        if (Enum.TryParse(conditionData.MatchSituation, out requiredMatchSituation) == false)
        {
            return false;
        }

        if (combatModel.CurrentSituation != requiredMatchSituation)
        {
            return false;
        }

        WrestlingSituation requiredWrestlingSituation;

        if (Enum.TryParse(conditionData.WrestlingSituation, out requiredWrestlingSituation) == false)
        {
            return false;
        }

        if (combatModel.CurrentWrestlingSituation != requiredWrestlingSituation)
        {
            return false;
        }

        GroundPosition requiredGroundPosition;

        if (Enum.TryParse(conditionData.GroundPosition, out requiredGroundPosition) == false)
        {
            return false;
        }

        if (combatModel.CurrentGroundPosition != requiredGroundPosition)
        {
            return false;
        }

        SkillUserRole requiredUserRole;

        if (Enum.TryParse(conditionData.UserRole, out requiredUserRole) == false)
        {
            return false;
        }

        return IsUserRoleMet(requiredUserRole, combatModel, skillUserSide);
    }

    private bool IsUserRoleMet(SkillUserRole requiredUserRole, MatchCombatModel combatModel, MatchFighterSide skillUserSide)
    {
        if (requiredUserRole == SkillUserRole.Any)
        {
            return true;
        }

        if (requiredUserRole == SkillUserRole.Attacker)
        {
            return combatModel.Attacker == skillUserSide;
        }

        if (requiredUserRole == SkillUserRole.Defender)
        {
            return combatModel.Defender == skillUserSide;
        }

        if (requiredUserRole == SkillUserRole.Top)
        {
            return combatModel.TopSide == skillUserSide;
        }

        if (requiredUserRole == SkillUserRole.Bottom)
        {
            return combatModel.BottomSide == skillUserSide;
        }

        return false;
    }
}
