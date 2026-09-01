using UnityEngine;

public class PositionChangeCalculator
{
    public bool TryCalculate(MatchCombatAction action, MatchFighterModel skillUser, MatchFighterModel target, GroundPositionData targetPositionData, out CombatActionResult actionResult)
    {
        actionResult = null;

        if (action == null || action.SelectedSkill == null)
        {
            return false;
        }

        if (skillUser == null || target == null || targetPositionData == null)
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

        if (action.SelectedSkill.ActionType != SkillActionType.PositionChange)
        {
            return false;
        }

        if (targetPositionData.Position != action.SelectedSkill.TargetGroundPosition)
        {
            return false;
        }

        float successChance = SuccessChanceCalculator.Calculate(action.SelectedSkill.BaseSuccessChance, skillUser.WrestlingOffense, target.WrestlingDefense);
        successChance = successChance * targetPositionData.PositionEntrySuccessMultiplier;
        successChance = Mathf.Clamp(successChance, 0f, 100f);

        float selectedChance = Random.Range(0f, 100f);
        bool isSuccess = selectedChance < successChance;

        CombatActionResultType resultType = CombatActionResultType.GroundPositionChangeFailed;

        if (isSuccess == true)
        {
            resultType = CombatActionResultType.GroundPositionChangeSucceeded;
        }

        actionResult = new CombatActionResult(action, resultType, isSuccess, successChance, 0f);

        return true;
    }
}
