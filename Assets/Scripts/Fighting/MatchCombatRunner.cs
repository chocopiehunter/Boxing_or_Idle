using System.Collections.Generic;
using UnityEngine;

public class MatchCombatRunner
{
    private readonly MatchCombatModel _combatModel;
    private readonly MatchFighterModel _playerFighter;
    private readonly MatchFighterModel _opponentFighter;

    private readonly MatchCombatStats _playerCombatStats;
    private readonly MatchCombatStats _opponentCombatStats;

    private readonly MatchUsableSkillFinder _usableSkillFinder;
    private readonly CombatActionSelector _actionSelector;
    private readonly StrikeCalculator _strikeCalculator;

    private readonly float _actionIntervalSeconds;
    private float _actionPassedSeconds;

    public MatchCombatRunner(MatchCombatModel combatModel, MatchFighterModel playerFighter, MatchFighterModel opponentFighter, MatchUsableSkillFinder usableSkillFinder,float actionIntervalSeconds)
    {
        _combatModel = combatModel;
        _playerFighter = playerFighter;
        _opponentFighter = opponentFighter;
        _playerCombatStats = new MatchCombatStats();
        _opponentCombatStats = new MatchCombatStats();
        _usableSkillFinder = usableSkillFinder;
        _actionSelector = new CombatActionSelector();
        _strikeCalculator = new StrikeCalculator();
        _actionIntervalSeconds = actionIntervalSeconds;
        Reset();
    }

    public bool UpdateCombatTime(float passedSeconds)
    {
        if (passedSeconds <= 0f)
        {
            return false;
        }

        if (_combatModel == null)
        {
            return false;
        }

        if (_playerFighter == null || _opponentFighter == null)
        {
            return false;
        }

        if(_combatModel.CurrentSituation == MatchSituation.None)
        {
            return false;
        }

        if (_actionIntervalSeconds <= 0f)
        {
            return false;
        }

        _playerFighter.UpdateCooldown(passedSeconds);
        _opponentFighter.UpdateCooldown(passedSeconds);
        _actionPassedSeconds = _actionPassedSeconds + passedSeconds;
        
        if (_actionPassedSeconds < _actionIntervalSeconds)
        {
            return false;
        }

        _actionPassedSeconds = _actionPassedSeconds - _actionIntervalSeconds;

        return true;
    }

    public void Reset()
    {
        _actionPassedSeconds = 0f;

        if (_playerFighter != null)
        {
            _playerFighter.ResetCoolTimes();
        }

        if (_opponentFighter != null)
        {
            _opponentFighter.ResetCoolTimes();
        }
    }

    public bool TryCreateNextAction(MatchStrategyData playerStrategyData, MatchStrategyData opponentStrategyData, out MatchCombatAction selectedAction)
    {
        selectedAction = null;

        if (_actionSelector == null)
        {
            return false;
        }

        List<SkillData> playerUsableSkills = GetUsableSkills(MatchFighterSide.Player);
        List<SkillData> opponentUsableSkills = GetUsableSkills(MatchFighterSide.Opponent);

        return _actionSelector.TrySelectAction(playerUsableSkills, opponentUsableSkills, playerStrategyData, opponentStrategyData, out selectedAction);
    }

    public bool TryRunAction(MatchCombatAction action, out CombatActionResult actionResult)
    {
        actionResult = null;

        if (action == null || action.SelectedSkill == null)
        {
            return false;
        }

        MatchFighterModel skillUser = GetFighter(action.SkillUserSide);

        MatchFighterModel target = GetFighter(action.TargetSide);

        if (skillUser == null || target == null)
        {
            return false;
        }

        MatchCombatStats skillUserStats = GetCombatStats(action.SkillUserSide);

        if (skillUserStats == null)
        {
            return false;
        }

        bool actionPrepared = false;

        if (action.SelectedSkill.ActionType == SkillActionType.Strike)
        {
            actionPrepared = _strikeCalculator.TryCalculate(action, skillUser, target, out actionResult);
        }

        if (action.SelectedSkill.ActionType == SkillActionType.Takedown)
        {
            if (_combatModel.CurrentSituation != MatchSituation.Standing)
            {
                return false;
            }

            bool situationChanged = _combatModel.ChangeToWrestling(WrestlingSituation.TakedownAttempt, action.SkillUserSide);

            if (situationChanged == false)
            {
                return false;
            }

            actionResult = new CombatActionResult(action, CombatActionResultType.TakedownStarted, true, 0f, 0f);

            actionPrepared = true;
        }

        if (actionPrepared == false)
        {
            return false;
        }

        bool cooldownStartSuccess = skillUser.TryStartSkillCooldown(action.SelectedSkill);

        if (cooldownStartSuccess == false)
        {
            actionResult = null;
            return false;
        }

        skillUser.UseStamina(action.SelectedSkill.StaminaCost);

        if (action.SelectedSkill.ActionType == SkillActionType.Strike)
        {
            if (actionResult.IsSuccess)
            {
                target.TakeDamage(actionResult.Damage);
            }

            skillUserStats.RecordStrike(actionResult.IsSuccess, true);
        }

        return true;
    }

    public MatchCombatStats GetCombatStats(MatchFighterSide fighterSide)
    {
        if (fighterSide == MatchFighterSide.Player)
        {
            return _playerCombatStats;
        }

        if (fighterSide == MatchFighterSide.Opponent)
        {
            return _opponentCombatStats;
        }

        return null;
    }

    public List<SkillData> GetUsableSkills(MatchFighterSide fighterSide)
    {
        List<SkillData> usableSkills = new List<SkillData>();

        if (_usableSkillFinder == null)
        {
            return usableSkills;
        }

        MatchFighterModel fighter = GetFighter(fighterSide);

        if (fighter == null)
        {
            return usableSkills;
        }

        return _usableSkillFinder.GetUsableSkills(fighter, _combatModel);
    }

    private MatchFighterModel GetFighter(MatchFighterSide fighterSide)
    {
        if (fighterSide == MatchFighterSide.Player)
        {
            return _playerFighter;
        }

        if (fighterSide == MatchFighterSide.Opponent)
        {
            return _opponentFighter;
        }

        return null;
    }
}
