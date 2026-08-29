using System.Collections.Generic;
using UnityEngine;

public class MatchCombatRunner
{
    private readonly MatchCombatModel _combatModel;
    private readonly MatchFighterModel _playerFighter;
    private readonly MatchFighterModel _opponentFighter;

    private readonly MatchUsableSkillFinder _usableSkillFinder;
    private readonly CombatActionSelector _actionSelector;

    private readonly float _actionIntervalSeconds;
    private float _actionPassedSeconds;

    public MatchCombatRunner(MatchCombatModel combatModel, MatchFighterModel playerFighter, MatchFighterModel opponentFighter, MatchUsableSkillFinder usableSkillFinder,float actionIntervalSeconds)
    {
        _combatModel = combatModel;
        _playerFighter = playerFighter;
        _opponentFighter = opponentFighter;
        _usableSkillFinder = usableSkillFinder;
        _actionSelector = new CombatActionSelector();
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
