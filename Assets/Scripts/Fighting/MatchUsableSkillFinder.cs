using System.Collections.Generic;
using UnityEngine;

public class MatchUsableSkillFinder
{
    private GameDataManager _gameDataManager;
    private SkillUseConditionChecker _conditionChecker;

    public MatchUsableSkillFinder(GameDataManager gameDataManager)
    {
        _gameDataManager = gameDataManager;
        _conditionChecker = new SkillUseConditionChecker();
    }

    public List<SkillData> GetUsableSkills(MatchFighterModel fighter, MatchCombatModel combatModel)
    {
        List<SkillData> usableSkills = new List<SkillData>();

        if (_gameDataManager == null)
        {
            return usableSkills;
        }

        if (fighter == null)
        {
            return usableSkills;
        }

        if (combatModel == null)
        {
            return usableSkills;
        }

        IReadOnlyList<string> ownedSkillIds = fighter.OwnedSkillIds;

        if (ownedSkillIds == null)
        {
            return usableSkills;
        }

        for (int index = 0; index < ownedSkillIds.Count; index++)
        {
            string skillId = ownedSkillIds[index];

            if (fighter.IsSkillReady(skillId) == false)
            {
                continue;
            }

            SkillData skillData = _gameDataManager.GetSkillData(skillId);

            if (skillData == null)
            {
                continue;
            }

            List<SkillUseConditionData> conditionDataList = _gameDataManager.GetSkillUseConditions(skillId);

            bool canUseSkill = _conditionChecker.CanUseSkill(conditionDataList, combatModel, fighter.FighterSide);

            if (canUseSkill == false)
            {
                continue;
            }

            usableSkills.Add(skillData);
        }

        return usableSkills;
    }
}
