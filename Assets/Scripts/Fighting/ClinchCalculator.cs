using UnityEngine;

public class ClinchCalculator
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

        if (action.SelectedSkill.ActionType != SkillActionType.ClinchEntry)
        {
            return false;
        }

        float offense = StaminaPenaltyCalculator.ApplyStaminaPenalty(skillUser.WrestlingOffense, skillUser);
        float defense = StaminaPenaltyCalculator.ApplyStaminaPenalty(target.WrestlingDefense, target);

        float controlChance = SuccessChanceCalculator.Calculate(action.SelectedSkill.BaseSuccessChance, offense, defense);
        float selectedChance = Random.Range(0f, 100f);

        bool skillUserHasControl = selectedChance < controlChance;

        CombatActionResultType resultType = CombatActionResultType.ClinchReversed;

        if (skillUserHasControl)
        {
            resultType = CombatActionResultType.ClinchStarted;
        }

        actionResult = new CombatActionResult(action, resultType, skillUserHasControl, controlChance, 0f);

        return true;
    }
}
