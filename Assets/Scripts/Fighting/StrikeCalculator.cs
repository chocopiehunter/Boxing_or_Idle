using UnityEngine;

public class StrikeCalculator
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

        if (action.SelectedSkill.ActionType != SkillActionType.Strike)
        {
            return false;
        }

        float successChance = CalculateSuccessChance(action.SelectedSkill, skillUser.StandingOffense, target.StandingDefense);

        float selectedChance = Random.Range(0f, 100f);

        bool isSuccess = selectedChance < successChance;

        float damage = 0f;

        if (isSuccess)
        {
            damage = DamageCalculator.Calculate(skillUser.StandingOffense, target.StandingDefense, action.SelectedSkill.DamageMultiplier);
        }

        actionResult = new CombatActionResult(action, isSuccess, successChance, damage);

        return true;
    }

    private float CalculateSuccessChance(SkillData skillData, float offense, float defense)
    {
        float successChance = skillData.BaseSuccessChance;
        float totalStat = offense + defense;

        if (totalStat > 0f)
        {
            float offenseRate = offense / totalStat;

            successChance = skillData.BaseSuccessChance * offenseRate * 2f;
        }

        successChance = Mathf.Clamp(successChance, 0f, 100f);

        return successChance;
    }
}
