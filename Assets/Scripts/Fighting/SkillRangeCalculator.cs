using UnityEngine;

public static class SkillRangeCalculator
{
    public static bool TryCalculate(SkillData skillData, float reach, out float minUseDistance, out float maxUseDistance)
    {
        minUseDistance = 0f;
        maxUseDistance = 0f;

        if (skillData == null)
        {
            return false;
        }

        if (skillData.MinUseDistance == 0f && skillData.MaxUseDistance == 0f)
        {
            return true;
        }

        if (skillData.MinUseDistance < 0f || skillData.MaxUseDistance <= 0f)
        {
            return false;
        }

        if (skillData.MaxUseDistance < skillData.MinUseDistance)
        {
            return false;
        }

        if (UsesReach(skillData) == false)
        {
            minUseDistance = skillData.MinUseDistance;
            maxUseDistance = skillData.MaxUseDistance;
            return true;
        }

        if (reach <= 0f)
        {
            return false;
        }

        minUseDistance = skillData.MinUseDistance * reach;
        maxUseDistance = skillData.MaxUseDistance * reach;

        return true;
    }

    private static bool UsesReach(SkillData skillData)
    {
        if (skillData.ActionType == SkillActionType.Strike)
        {
            return true;
        }

        if (skillData.ActionType == SkillActionType.Kick)
        {
            return true;
        }

        return false;
    }
}
