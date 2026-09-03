using UnityEngine;

public class SubmissionCalculator
{
    private const float MinSubmissionDamage = 1f;

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

        float defense = StaminaPenaltyCalculator.ApplyStaminaPenalty(target.JiuJitsuDefense, target);
        float offense = StaminaPenaltyCalculator.ApplyStaminaPenalty(skillUser.JiuJitsuOffense, skillUser);

        float escapeChance = SuccessChanceCalculator.Calculate(action.SelectedSkill.BaseSuccessChance, defense, offense);
        float selectedChance = Random.Range(0f, 100f);

        bool escaped = selectedChance < escapeChance;

        if (escaped == true)
        {
            actionResult = new CombatActionResult(action, CombatActionResultType.SubmissionEscaped, false, escapeChance, 0f);
            return true;
        }

        float totalStat = offense + defense;
        float offenseRate = 0.5f;

        if (totalStat > 0f)
        {
            offenseRate = offense / totalStat;
        }

        float submissionDamage = MinSubmissionDamage + (offenseRate * 10f);

        submissionDamage = submissionDamage * action.SelectedSkill.SubmissionDamageMultiplier;
        submissionDamage = submissionDamage * currentPositionData.SubmissionSuccessMultiplier;

        float nextSubmissionResistHp = currentSubmissionResistHp - submissionDamage;

        CombatActionResultType resultType = CombatActionResultType.SubmissionInProgress;
        bool isSuccess = false;

        if (nextSubmissionResistHp <= 0f)
        {
            resultType = CombatActionResultType.SubmissionSucceeded;
            isSuccess = true;
        }

        actionResult = new CombatActionResult(action, resultType, isSuccess, escapeChance, submissionDamage);

        return true;
    }
}
