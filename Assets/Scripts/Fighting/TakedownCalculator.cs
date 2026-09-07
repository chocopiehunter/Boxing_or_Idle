using UnityEngine;

public class TakedownCalculator
{
    private const float ClinchTransitionRange = 20f;

    public bool TryCalculate(MatchCombatAction action, MatchFighterModel attacker, MatchFighterModel defender, out CombatActionResult actionResult)
    {
        actionResult = null;

        if (action == null || action.SelectedSkill == null)
        {
            return false;
        }

        if (attacker == null || defender == null)
        {
            return false;
        }

        if (action.SkillUserSide != attacker.FighterSide)
        {
            return false;
        }

        if (action.TargetSide != defender.FighterSide)
        {
            return false;
        }

        if (action.SelectedSkill.ActionType != SkillActionType.Takedown)
        {
            return false;
        }

        float offense = StaminaPenaltyCalculator.ApplyStaminaPenalty(attacker.WrestlingOffense, attacker);
        float defense = StaminaPenaltyCalculator.ApplyStaminaPenalty(defender.WrestlingDefense, defender);

        float successChance = SuccessChanceCalculator.Calculate(action.SelectedSkill.BaseSuccessChance, offense, defense);
        float selectedChance = Random.Range(0f, 100f);

        CombatActionResultType resultType = CombatActionResultType.TakedownDefended;

        bool isSuccess = false;

        if (selectedChance < successChance)
        {
            resultType = CombatActionResultType.TakedownSucceeded;
            isSuccess = true;
        }
        else
        {
            float clinchTransitionEnd = successChance + ClinchTransitionRange;

            clinchTransitionEnd = Mathf.Clamp(clinchTransitionEnd, 0f, 100f);

            if (selectedChance < clinchTransitionEnd)
            {
                resultType = CombatActionResultType.TakedownToClinch;
            }
        }

        actionResult = new CombatActionResult(action, resultType, isSuccess, successChance, 0f);

        return true;
    }
}
