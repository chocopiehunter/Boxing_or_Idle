using UnityEngine;

public class SubmissionCalculator
{
    private const float MinDamage = 1f;

    public bool TryCalculate(MatchCombatAction action, MatchFighterModel skillUser, MatchFighterModel target, GroundPositionData currentPositionData, 
                             float currentSubmissionResistHp, out CombatActionResult actionResult)
    {
        actionResult = null;

        if (action == null || action.SelectedSkill == null)
        {
            return false;
        }

        if (skillUser == null || target == null || currentPositionData == null)
        {
            return false;
        }

        if (action.SkillUserSide != skillUser.FighterSide)
        {
            return false;
        }

        if (action.TargetSide != target.FighterSide)
        {
            return false;
        }

        if (action.SelectedSkill.ActionType != SkillActionType.Submission)
        {
            return false;
        }

        return true;
    }
}
