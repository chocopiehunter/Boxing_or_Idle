using UnityEngine;

public class MatchDistanceModel
{
    public bool IsReady { get; private set; }
    public Vector2 PlayerPosition { get; private set; }
    public Vector2 OpponentPosition { get; private set; }
    public float MinDistance { get; private set; }
    public Vector2 AreaCenter { get; private set; }
    public float AreaRadius { get; private set; }

    public float CurrentDistance
    {
        get
        {
            return Vector2.Distance(PlayerPosition, OpponentPosition);
        }
    }

    private Vector2 _playerStartPosition;
    private Vector2 _opponentStartPosition;

    public bool TrySetup(Vector2 playerStartPosition, Vector2 opponentStartPosition, float minDistance, Vector2 areaCenter, float areaRadius)
    {
        IsReady = false;

        if (minDistance <= 0f || areaRadius <= 0f)
        {
            return false;
        }

        if (minDistance > areaRadius * 2f)
        {
            return false;
        }

        if (IsPositionInsideArea(playerStartPosition, areaCenter, areaRadius) == false)
        {
            return false;
        }

        if (IsPositionInsideArea(opponentStartPosition, areaCenter, areaRadius) == false)
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
        AreaRadius = areaRadius;
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

        return IsPositionInsideArea(position, AreaCenter, AreaRadius);
    }

    public Vector2 GetPositionInsideArea(Vector2 position)
    {
        if (IsReady == false)
        {
            return position;
        }

        Vector2 centerToPosition = position - AreaCenter;

        if (centerToPosition.sqrMagnitude <= AreaRadius * AreaRadius)
        {
            return position;
        }

        return AreaCenter + centerToPosition.normalized * AreaRadius;
    }

    private bool IsPositionInsideArea(Vector2 position, Vector2 areaCenter, float areaRadius)
    {
        return Vector2.Distance(position, areaCenter) <= areaRadius;
    }
}
