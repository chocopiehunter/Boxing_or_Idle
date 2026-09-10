using System.Collections.Generic;
using UnityEngine;

public class MatchUsableSkillFinder
{
    private GameDataManager _gameDataManager;
    private MatchDistanceModel _distanceModel;
    private SkillUseConditionChecker _conditionChecker;

    public MatchUsableSkillFinder(GameDataManager gameDataManager, MatchDistanceModel distanceModel)
    {
        _gameDataManager = gameDataManager;
        _distanceModel = distanceModel;
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

            if (IsSkillInsideUseDistance(skillData) == false)
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

    private bool IsSkillInsideUseDistance(SkillData skillData)
    {
        if (skillData == null)
        {
            return false;
        }

        if (skillData.MinUseDistance == 0f && skillData.MaxUseDistance == 0f)
        {
            return true;
        }

        if (skillData.MinUseDistance < 0f || skillData.MaxUseDistance <= 0f)
        {
            return false;
        }

        if (skillData.MaxUseDistance < skillData.MinUseDistance)
        {
            return false;
        }

        if (_distanceModel == null || _distanceModel.IsReady == false)
        {
            return false;
        }

        float currentDistance = _distanceModel.CurrentDistance;

        return currentDistance >= skillData.MinUseDistance && currentDistance <= skillData.MaxUseDistance;
    }
}
