using UnityEngine;

public class MatchCombatRunner
{
    private readonly MatchCombatModel _combatModel;
    private readonly float _actionIntervalSeconds;
    private float _actionPassedSeconds;

    public MatchCombatRunner(MatchCombatModel combatModel, float actionIntervalSeconds)
    {
        _combatModel = combatModel;
        _actionIntervalSeconds = actionIntervalSeconds;

        Reset();
    }

    public bool UpdateActionTime(float passedSeconds)
    {
        if (passedSeconds <= 0f)
        {
            return false;
        }

        if (_combatModel == null)
        {
            return false;
        }

        if (_combatModel.CurrentSituation == MatchSituation.None)
        {
            return false;
        }

        if (_actionIntervalSeconds <= 0f)
        {
            return false;
        }

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
    }
}
