using UnityEngine;

public class MatchStepRunner
{
    private MatchStepMover _stepMover = new MatchStepMover();
    private float _stepIntervalSeconds;
    private float _stepMoveDistance;
    private float _playerPreferredMinDistance;
    private float _playerPreferredMaxDistance;
    private float _opponentPreferredMinDistance;
    private float _opponentPreferredMaxDistance;
    private float _insideRangeMoveChance;
    private float _stepRemainingSeconds;

    public bool IsReady { get; private set; }

    public bool TrySetup(float stepIntervalSeconds, float stepMoveDistance, float playerPreferredMinDistance, float playerPreferredMaxDistance,
                         float opponentPreferredMinDistance, float opponentPreferredMaxDistance, float insideRangeMoveChance)
    {
        IsReady = false;

        if (stepIntervalSeconds <= 0f || stepMoveDistance <= 0f)
        {
            return false;
        }

        if (playerPreferredMinDistance <= 0f || playerPreferredMaxDistance < playerPreferredMinDistance)
        {
            return false;
        }

        if (opponentPreferredMinDistance <= 0f || opponentPreferredMaxDistance < opponentPreferredMinDistance)
        {
            return false;
        }

        if (insideRangeMoveChance < 0f || insideRangeMoveChance > 1f)
        {
            return false;
        }

        _stepIntervalSeconds = stepIntervalSeconds;
        _stepMoveDistance = stepMoveDistance;
        _playerPreferredMinDistance = playerPreferredMinDistance;
        _playerPreferredMaxDistance = playerPreferredMaxDistance;
        _opponentPreferredMinDistance = opponentPreferredMinDistance;
        _opponentPreferredMaxDistance = opponentPreferredMaxDistance;
        _insideRangeMoveChance = insideRangeMoveChance;
        IsReady = true;

        Reset();

        return true;
    }

    public void Reset()
    {
        _stepRemainingSeconds = _stepIntervalSeconds;
    }

    public bool TryUpdate(float passedSeconds, MatchDistanceModel distanceModel, out MatchStepResult stepResult)
    {
        stepResult = null;

        if (IsReady == false || distanceModel == null || distanceModel.IsReady == false)
        {
            return false;
        }

        if (passedSeconds <= 0f)
        {
            return false;
        }

        _stepRemainingSeconds = _stepRemainingSeconds - passedSeconds;

        if (_stepRemainingSeconds > 0f)
        {
            return false;
        }

        _stepRemainingSeconds = _stepIntervalSeconds;

        float currentDistance = distanceModel.CurrentDistance;
        MatchStepType playerStepType = ChooseStepType(currentDistance, _playerPreferredMinDistance, _playerPreferredMaxDistance);
        MatchStepType opponentStepType = ChooseStepType(currentDistance, _opponentPreferredMinDistance, _opponentPreferredMaxDistance);

        if (playerStepType == MatchStepType.None && opponentStepType == MatchStepType.None)
        {
            return false;
        }

        Vector2 previousPlayerPosition = distanceModel.PlayerPosition;
        Vector2 previousOpponentPosition = distanceModel.OpponentPosition;

        bool moveSuccess = _stepMover.TryMove(
            distanceModel,
            playerStepType,
            opponentStepType,
            _stepMoveDistance,
            _stepMoveDistance);

        if (moveSuccess == false)
        {
            return false;
        }

        bool playerPositionChanged = (distanceModel.PlayerPosition - previousPlayerPosition).sqrMagnitude > 0.000001f;
        bool opponentPositionChanged = (distanceModel.OpponentPosition - previousOpponentPosition).sqrMagnitude > 0.000001f;

        if (playerPositionChanged == false && opponentPositionChanged == false)
        {
            return false;
        }

        stepResult = new MatchStepResult(
            playerStepType,
            opponentStepType,
            distanceModel.PlayerPosition,
            distanceModel.OpponentPosition);

        return true;
    }

    private MatchStepType ChooseStepType(float currentDistance, float preferredMinDistance, float preferredMaxDistance)
    {
        if (currentDistance > preferredMaxDistance)
        {
            return MatchStepType.Forward;
        }

        if (currentDistance < preferredMinDistance)
        {
            return MatchStepType.Back;
        }

        if (Random.value > _insideRangeMoveChance)
        {
            return MatchStepType.None;
        }

        if (Random.value < 0.5f)
        {
            return MatchStepType.Forward;
        }

        return MatchStepType.Back;
    }
}
