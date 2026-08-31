using UnityEngine;

public class GroundStrikeCalculator
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

        if (action.SelectedSkill.ActionType != SkillActionType.GroundStrike)
        {
            return false;
        }

        float successChance = SuccessChanceCalculator.Calculate(action.SelectedSkill.BaseSuccessChance, skillUser.WrestlingOffense, target.WrestlingDefense);
        float selectedChance = Random.Range(0f, 100f);
        bool isSuccess = selectedChance < successChance;

        CombatActionResultType resultType = CombatActionResultType.StrikeMissed;

        if (isSuccess)
        {
            resultType = CombatActionResultType.StrikeHit;
        }

        float damage = 0f;

        if (isSuccess)
        {
            damage = DamageCalculator.Calculate(skillUser.WrestlingOffense, target.WrestlingDefense, action.SelectedSkill.DamageMultiplier);
        }

        actionResult = new CombatActionResult(action, resultType, isSuccess, successChance, damage);

        return true;
    }
}
