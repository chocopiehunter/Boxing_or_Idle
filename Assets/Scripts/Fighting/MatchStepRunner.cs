using UnityEngine;

public class MatchStepRunner
{
    private MatchStepMover _stepMover = new MatchStepMover();
    private float _playerStepIntervalSeconds;
    private float _playerStepMoveDistance;
    private float _opponentStepIntervalSeconds;
    private float _opponentStepMoveDistance;
    private float _playerPreferredMinDistance;
    private float _playerPreferredMaxDistance;
    private float _opponentPreferredMinDistance;
    private float _opponentPreferredMaxDistance;
    private float _insideRangeMoveChance;
    private float _insideRangeCircleChance;
    private float _playerStepRemainingSeconds;
    private float _opponentStepRemainingSeconds;
    private float _cageNearBoundaryRatio;
    private float _cageEscapeStartChance;
    private float _cagePressureFollowChance;
    private int _cageEscapeStepCount;
    private MatchStepType _playerCageEscapeType;
    private MatchStepType _opponentCageEscapeType;
    private int _playerCageEscapeRemainingSteps;
    private int _opponentCageEscapeRemainingSteps;

    public bool IsReady { get; private set; }

    public bool TrySetup(float playerStepIntervalSeconds, float playerStepMoveDistance,
                         float opponentStepIntervalSeconds, float opponentStepMoveDistance,
                         float playerPreferredMinDistance, float playerPreferredMaxDistance,
                         float opponentPreferredMinDistance, float opponentPreferredMaxDistance,
                         float insideRangeMoveChance, float insideRangeCircleChance)
    {
        IsReady = false;

        if (playerStepIntervalSeconds <= 0f || playerStepMoveDistance <= 0f)
        {
            return false;
        }

        if (opponentStepIntervalSeconds <= 0f || opponentStepMoveDistance <= 0f)
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

        if (insideRangeCircleChance < 0f || insideRangeCircleChance > 1f)
        {
            return false;
        }

        _playerStepIntervalSeconds = playerStepIntervalSeconds;
        _playerStepMoveDistance = playerStepMoveDistance;
        _opponentStepIntervalSeconds = opponentStepIntervalSeconds;
        _opponentStepMoveDistance = opponentStepMoveDistance;
        _playerPreferredMinDistance = playerPreferredMinDistance;
        _playerPreferredMaxDistance = playerPreferredMaxDistance;
        _opponentPreferredMinDistance = opponentPreferredMinDistance;
        _opponentPreferredMaxDistance = opponentPreferredMaxDistance;
        _insideRangeMoveChance = insideRangeMoveChance;
        _insideRangeCircleChance = insideRangeCircleChance;
        IsReady = true;

        Reset();

        return true;
    }

    public bool TrySetupCage(float nearBoundaryRatio, float escapeStartChance, float pressureFollowChance, int escapeStepCount)
    {
        if (IsReady == false)
        {
            return false;
        }

        if (nearBoundaryRatio <= 0f || nearBoundaryRatio > 1f)
        {
            return false;
        }

        if (escapeStartChance < 0f || escapeStartChance > 1f)
        {
            return false;
        }

        if (pressureFollowChance < 0f || pressureFollowChance > 1f)
        {
            return false;
        }

        if (escapeStepCount <= 0)
        {
            return false;
        }

        _cageNearBoundaryRatio = nearBoundaryRatio;
        _cageEscapeStartChance = escapeStartChance;
        _cagePressureFollowChance = pressureFollowChance;
        _cageEscapeStepCount = escapeStepCount;
        _playerCageEscapeType = MatchStepType.None;
        _opponentCageEscapeType = MatchStepType.None;
        _playerCageEscapeRemainingSteps = 0;
        _opponentCageEscapeRemainingSteps = 0;

        return true;
    }

    public void Reset()
    {
        _playerStepRemainingSeconds = _playerStepIntervalSeconds;
        _opponentStepRemainingSeconds = _opponentStepIntervalSeconds;
        _playerCageEscapeType = MatchStepType.None;
        _opponentCageEscapeType = MatchStepType.None;
        _playerCageEscapeRemainingSteps = 0;
        _opponentCageEscapeRemainingSteps = 0;
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

        _playerStepRemainingSeconds = _playerStepRemainingSeconds - passedSeconds;
        _opponentStepRemainingSeconds = _opponentStepRemainingSeconds - passedSeconds;

        bool playerStepReady = _playerStepRemainingSeconds <= 0f;
        bool opponentStepReady = _opponentStepRemainingSeconds <= 0f;

        if (playerStepReady == false && opponentStepReady == false)
        {
            return false;
        }

        if (playerStepReady)
        {
            _playerStepRemainingSeconds = _playerStepRemainingSeconds + _playerStepIntervalSeconds;

            if (_playerStepRemainingSeconds <= 0f)
            {
                _playerStepRemainingSeconds = _playerStepIntervalSeconds;
            }
        }

        if (opponentStepReady)
        {
            _opponentStepRemainingSeconds = _opponentStepRemainingSeconds + _opponentStepIntervalSeconds;

            if (_opponentStepRemainingSeconds <= 0f)
            {
                _opponentStepRemainingSeconds = _opponentStepIntervalSeconds;
            }
        }

        float currentDistance = distanceModel.CurrentDistance;
        MatchStepType playerStepType = MatchStepType.None;
        MatchStepType opponentStepType = MatchStepType.None;

        if (playerStepReady)
        {
            playerStepType = ChooseStepTypeWithCage(
                distanceModel,
                distanceModel.PlayerPosition,
                currentDistance,
                _playerPreferredMinDistance,
                _playerPreferredMaxDistance,
                ref _playerCageEscapeType,
                ref _playerCageEscapeRemainingSteps);
        }

        if (opponentStepReady)
        {
            opponentStepType = ChooseStepTypeWithCage(
                distanceModel,
                distanceModel.OpponentPosition,
                currentDistance,
                _opponentPreferredMinDistance,
                _opponentPreferredMaxDistance,
                ref _opponentCageEscapeType,
                ref _opponentCageEscapeRemainingSteps);
        }

        ApplyCagePressureFollow(currentDistance, playerStepReady, opponentStepReady, ref playerStepType, ref opponentStepType);

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
            _playerStepMoveDistance,
            _opponentStepMoveDistance);

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
            distanceModel.OpponentPosition,
            playerPositionChanged,
            opponentPositionChanged);

        return true;
    }

    private void ApplyCagePressureFollow(float currentDistance, bool playerStepReady, bool opponentStepReady, ref MatchStepType playerStepType, ref MatchStepType opponentStepType)
    {
        MatchStepType playerEscapeType = GetActiveCageEscapeType(playerStepType, _playerCageEscapeType, _playerCageEscapeRemainingSteps);
        MatchStepType opponentEscapeType = GetActiveCageEscapeType(opponentStepType, _opponentCageEscapeType, _opponentCageEscapeRemainingSteps);
        bool playerIsEscaping = IsCageEscapeStep(playerEscapeType);
        bool opponentIsEscaping = IsCageEscapeStep(opponentEscapeType);

        if (playerIsEscaping == opponentIsEscaping)
        {
            return;
        }

        if (playerIsEscaping && opponentStepReady && currentDistance <= _opponentPreferredMaxDistance)
        {
            if (Random.value < _cagePressureFollowChance)
            {
                opponentStepType = GetPressureFollowStepType(playerEscapeType);
            }

            return;
        }

        if (opponentIsEscaping && playerStepReady && currentDistance <= _playerPreferredMaxDistance)
        {
            if (Random.value < _cagePressureFollowChance)
            {
                playerStepType = GetPressureFollowStepType(opponentEscapeType);
            }
        }
    }

    private MatchStepType GetActiveCageEscapeType(MatchStepType currentStepType, MatchStepType cageEscapeType, int cageEscapeRemainingSteps)
    {
        if (IsCageEscapeStep(currentStepType))
        {
            return currentStepType;
        }

        if (cageEscapeRemainingSteps > 0 && IsCageEscapeStep(cageEscapeType))
        {
            return cageEscapeType;
        }

        return MatchStepType.None;
    }

    private bool IsCageEscapeStep(MatchStepType stepType)
    {
        return stepType == MatchStepType.CageEscapeLeft || stepType == MatchStepType.CageEscapeRight;
    }

    private MatchStepType GetPressureFollowStepType(MatchStepType cageEscapeType)
    {
        if (cageEscapeType == MatchStepType.CageEscapeLeft)
        {
            return MatchStepType.CircleRight;
        }

        if (cageEscapeType == MatchStepType.CageEscapeRight)
        {
            return MatchStepType.CircleLeft;
        }

        return MatchStepType.None;
    }

    private MatchStepType ChooseStepTypeWithCage(
        MatchDistanceModel distanceModel,
        Vector2 fighterPosition,
        float currentDistance,
        float preferredMinDistance,
        float preferredMaxDistance,
        ref MatchStepType cageEscapeType,
        ref int cageEscapeRemainingSteps)
    {
        if (cageEscapeRemainingSteps > 0)
        {
            cageEscapeRemainingSteps = cageEscapeRemainingSteps - 1;

            return cageEscapeType;
        }

        cageEscapeType = MatchStepType.None;

        if (distanceModel.IsNearBoundary(fighterPosition, _cageNearBoundaryRatio) && Random.value < _cageEscapeStartChance)
        {
            if (Random.value < 0.5f)
            {
                cageEscapeType = MatchStepType.CageEscapeLeft;
            }
            else
            {
                cageEscapeType = MatchStepType.CageEscapeRight;
            }

            cageEscapeRemainingSteps = _cageEscapeStepCount - 1;

            return cageEscapeType;
        }

        return ChooseStepType(currentDistance, preferredMinDistance, preferredMaxDistance);
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

        if (Random.value < _insideRangeCircleChance)
        {
            if (Random.value < 0.5f)
            {
                return MatchStepType.CircleLeft;
            }

            return MatchStepType.CircleRight;
        }

        if (Random.value < 0.5f)
        {
            return MatchStepType.Forward;
        }

        return MatchStepType.Back;
    }
}
