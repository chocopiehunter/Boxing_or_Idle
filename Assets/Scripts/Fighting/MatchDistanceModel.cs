using UnityEngine;

public class MatchDistanceModel
{
    public bool IsReady { get; private set; }
    public Vector2 PlayerPosition { get; private set; }
    public Vector2 OpponentPosition { get; private set; }
    public float MinDistance { get; private set; }
    public Vector2 AreaCenter { get; private set; }
    public Vector2 AreaRadii { get; private set; }

    public float CurrentDistance
    {
        get
        {
            return Vector2.Distance(PlayerPosition, OpponentPosition);
        }
    }

    private Vector2 _playerStartPosition;
    private Vector2 _opponentStartPosition;

    public bool TrySetup(Vector2 playerStartPosition, Vector2 opponentStartPosition, float minDistance, Vector2 areaCenter, Vector2 areaRadii)
    {
        IsReady = false;

        if (minDistance <= 0f || areaRadii.x <= 0f || areaRadii.y <= 0f)
        {
            return false;
        }

        float maxAreaDistance = Mathf.Max(areaRadii.x, areaRadii.y) * 2f;

        if (minDistance > maxAreaDistance)
        {
            return false;
        }

        if (IsPositionInsideArea(playerStartPosition, areaCenter, areaRadii) == false)
        {
            return false;
        }

        if (IsPositionInsideArea(opponentStartPosition, areaCenter, areaRadii) == false)
        {
            return false;
        }

        if (Vector2.Distance(playerStartPosition, opponentStartPosition) < minDistance)
        {
            return false;
        }

        _playerStartPosition = playerStartPosition;
        _opponentStartPosition = opponentStartPosition;
        MinDistance = minDistance;
        AreaCenter = areaCenter;
        AreaRadii = areaRadii;
        PlayerPosition = _playerStartPosition;
        OpponentPosition = _opponentStartPosition;

        IsReady = true;

        return true;
    }

    public bool TrySetPositions(Vector2 playerPosition, Vector2 opponentPosition)
    {
        if (IsReady == false)
        {
            return false;
        }

        if (IsInsideArea(playerPosition) == false || IsInsideArea(opponentPosition) == false)
        {
            return false;
        }

        if (Vector2.Distance(playerPosition, opponentPosition) < MinDistance)
        {
            return false;
        }

        PlayerPosition = playerPosition;
        OpponentPosition = opponentPosition;

        return true;
    }

    public void ResetPositions()
    {
        if (IsReady == false)
        {
            return;
        }

        PlayerPosition = _playerStartPosition;
        OpponentPosition = _opponentStartPosition;
    }

    public bool TryGetPosition(MatchFighterSide fighterSide, out Vector2 position)
    {
        position = Vector2.zero;

        if (IsReady == false)
        {
            return false;
        }

        if (fighterSide == MatchFighterSide.Player)
        {
            position = PlayerPosition;
            return true;
        }

        if (fighterSide == MatchFighterSide.Opponent)
        {
            position = OpponentPosition;
            return true;
        }

        return false;
    }

    public bool IsInsideArea(Vector2 position)
    {
        if (IsReady == false)
        {
            return false;
        }

        return IsPositionInsideArea(position, AreaCenter, AreaRadii);
    }

    public Vector2 GetPositionInsideArea(Vector2 position)
    {
        if (IsReady == false)
        {
            return position;
        }

        Vector2 centerToPosition = position - AreaCenter;

        float normalizedX = centerToPosition.x / AreaRadii.x;
        float normalizedY = centerToPosition.y / AreaRadii.y;
        float normalizedDistanceSqr = normalizedX * normalizedX + normalizedY * normalizedY;

        if (normalizedDistanceSqr <= 1f)
        {
            return position;
        }

        float boundaryScale = 1f / Mathf.Sqrt(normalizedDistanceSqr);

        return AreaCenter + centerToPosition * boundaryScale;
    }

    public bool IsNearBoundary(Vector2 position, float nearBoundaryRatio)
    {
        if (IsReady == false)
        {
            return false;
        }

        if (nearBoundaryRatio <= 0f || nearBoundaryRatio > 1f)
        {
            return false;
        }

        Vector2 centerToPosition = position - AreaCenter;
        float normalizedX = centerToPosition.x / AreaRadii.x;
        float normalizedY = centerToPosition.y / AreaRadii.y;
        float normalizedDistanceSqr = normalizedX * normalizedX + normalizedY * normalizedY;
        float nearBoundaryRatioSqr = nearBoundaryRatio * nearBoundaryRatio;

        return normalizedDistanceSqr >= nearBoundaryRatioSqr;
    }

    private bool IsPositionInsideArea(Vector2 position, Vector2 areaCenter, Vector2 areaRadii)
    {
        if (areaRadii.x <= 0f || areaRadii.y <= 0f)
        {
            return false;
        }

        Vector2 centerToPosition = position - areaCenter;
        float normalizedX = centerToPosition.x / areaRadii.x;
        float normalizedY = centerToPosition.y / areaRadii.y;
        float normalizedDistanceSqr = normalizedX * normalizedX + normalizedY * normalizedY;

        return normalizedDistanceSqr <= 1f;
    }
}
