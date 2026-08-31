using UnityEngine;

public class ClinchEscapeCalculator
{
    public bool TryCalculate(MatchCombatAction action, MatchFighterModel skillUser, MatchFighterModel target, out CombatActionResult actionResult)
    {
        actionResult = null;

        if (action == null || action.SelectedSkill == null)
        {
            return false;
        }

        if (skillUser == null || target == null)
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

        if (action.SelectedSkill.ActionType != SkillActionType.Escape)
        {
            return false;
        }

        float escapeChance = SuccessChanceCalculator.Calculate(action.SelectedSkill.BaseSuccessChance, skillUser.WrestlingDefense, target.WrestlingOffense);
        float selectedChance = Random.Range(0f, 100f);
        bool isSuccess = selectedChance < escapeChance;

        CombatActionResultType resultType = CombatActionResultType.ClinchEscapeFailed;

        if (isSuccess)
        {
            resultType = CombatActionResultType.ClinchEscaped;
        }

        actionResult = new CombatActionResult(action, resultType, isSuccess, escapeChance, 0f);

        return true;
    }
}
