using System.Collections.Generic;
using UnityEngine;

public class CombatActionSelector
{
    public bool TrySelectAction(List<SkillData> playerUsableSkills, List<SkillData> opponentUsableSkills, MatchStrategyData playerStrategyData, MatchStrategyData opponentStrategyData, out MatchCombatAction selectedAction)
    {
        selectedAction = null;

        bool playerCanAct = playerUsableSkills != null && playerUsableSkills.Count > 0;
        bool opponentCanAct = opponentUsableSkills != null && opponentUsableSkills.Count > 0;

        if (playerCanAct == false && opponentCanAct == false)
        {
            return false;
        }

        MatchFighterSide skillUserSide = SelectSkillUserSide(playerCanAct, opponentCanAct, playerStrategyData, opponentStrategyData);
        List<SkillData> usableSkills = GetUsableSkills(skillUserSide, playerUsableSkills, opponentUsableSkills);
        MatchStrategyData skillSelectionStrategyData = GetStrategyData(skillUserSide, playerStrategyData, opponentStrategyData);
        SkillData selectedSkill = SelectSkill(usableSkills, skillSelectionStrategyData);

        if (selectedSkill == null)
        {
            return false;
        }

        MatchFighterSide targetSide = GetTargetSide(skillUserSide);

        if (targetSide == MatchFighterSide.None)
        {
            return false;
        }

        selectedAction = new MatchCombatAction(skillUserSide, targetSide, selectedSkill);

        return true;
    }

    private MatchFighterSide SelectSkillUserSide(bool playerCanAct, bool opponentCanAct, MatchStrategyData playerStrategyData, MatchStrategyData opponentStrategyData)
    {
        if (playerCanAct && opponentCanAct)
        {
            float playerSelectionWeight = GetActionSelectionWeight(playerStrategyData);
            float opponentSelectionWeight = GetActionSelectionWeight(opponentStrategyData);
            float totalSelectionWeight = playerSelectionWeight + opponentSelectionWeight;

            if (totalSelectionWeight <= 0f)
            {
                return MatchFighterSide.None;
            }

            float selectedWeight = Random.Range(0f, totalSelectionWeight);

            if (selectedWeight < playerSelectionWeight)
            {
                return MatchFighterSide.Player;
            }

            return MatchFighterSide.Opponent;
        }

        if (playerCanAct)
        {
            return MatchFighterSide.Player;
        }

        if (opponentCanAct)
        {
            return MatchFighterSide.Opponent;
        }

        return MatchFighterSide.None;
    }

    private float GetActionSelectionWeight(MatchStrategyData strategyData)
    {
        if (strategyData == null)
        {
            return 1f;
        }

        if (strategyData.ActionSelectionWeight < 0f)
        {
            return 0f;
        }

        return strategyData.ActionSelectionWeight;
    }

    private List<SkillData> GetUsableSkills(MatchFighterSide fighterSide, List<SkillData> playerUsableSkills, List<SkillData> opponentUsableSkills)
    {
        if (fighterSide == MatchFighterSide.Player)
        {
            return playerUsableSkills;
        }

        if (fighterSide == MatchFighterSide.Opponent)
        {
            return opponentUsableSkills;
        }

        return null;
    }

    private MatchStrategyData GetStrategyData(MatchFighterSide fighterSide, MatchStrategyData playerStrategyData, MatchStrategyData opponentStrategyData)
    {
        if (fighterSide == MatchFighterSide.Player)
        {
            return playerStrategyData;
        }

        if (fighterSide == MatchFighterSide.Opponent)
        {
            return opponentStrategyData;
        }

        return null;
    }

    private SkillData SelectSkill(List<SkillData> usableSkills, MatchStrategyData strategyData)
    {
        if (usableSkills == null)
        {
            return null;
        }

        if (usableSkills.Count == 0)
        {
            return null;
        }

        float totalSelectionWeight = 0f;

        for (int index = 0; index < usableSkills.Count; index++)
        {
            SkillData skillData = usableSkills[index];

            float selectionWeight = GetSkillSelectionWeight(skillData, strategyData);

            totalSelectionWeight = totalSelectionWeight + selectionWeight;
        }

        if (totalSelectionWeight <= 0f)
        {
            return null;
        }

        float selectedWeight = Random.Range(0f, totalSelectionWeight);

        float accumulatedWeight = 0f;

        SkillData lastSelectableSkill = null;

        for (int index = 0; index < usableSkills.Count; index++)
        {
            SkillData skillData = usableSkills[index];

            float selectionWeight = GetSkillSelectionWeight(skillData, strategyData);

            if (selectionWeight <= 0f)
            {
                continue;
            }

            lastSelectableSkill = skillData;

            accumulatedWeight = accumulatedWeight + selectionWeight;

            if (selectedWeight < accumulatedWeight)
            {
                return skillData;
            }
        }

        return lastSelectableSkill;
    }

    private float GetSkillSelectionWeight(SkillData skillData, MatchStrategyData strategyData)
    {
        if (skillData == null)
        {
            return 0f;
        }

        float selectionWeight = 1f;

        if (strategyData == null)
        {
            return selectionWeight;
        }

        if (skillData.Category == SkillCategoryType.Strike)
        {
            selectionWeight = strategyData.StrikeSelectionWeight;
        }

        if (skillData.Category == SkillCategoryType.Wrestling)
        {
            selectionWeight = strategyData.WrestlingSelectionWeight;
        }

        if (skillData.Category == SkillCategoryType.JiuJitsu)
        {
            selectionWeight = strategyData.JiuJitsuSelectionWeight;
        }

        if (selectionWeight < 0f)
        {
            return 0f;
        }

        return selectionWeight;
    }

    private MatchFighterSide GetTargetSide(MatchFighterSide skillUserSide)
    {
        if (skillUserSide == MatchFighterSide.Player)
        {
            return MatchFighterSide.Opponent;
        }

        if (skillUserSide == MatchFighterSide.Opponent)
        {
            return MatchFighterSide.Player;
        }

        return MatchFighterSide.None;
    }
}
