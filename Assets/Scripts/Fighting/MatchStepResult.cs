using UnityEngine;

public class MatchStepResult
{
    public MatchStepType PlayerStepType { get; private set; }
    public MatchStepType OpponentStepType { get; private set; }
    public Vector2 PlayerPosition { get; private set; }
    public Vector2 OpponentPosition { get; private set; }

    public MatchStepResult(MatchStepType playerStepType, MatchStepType opponentStepType, Vector2 playerPosition, Vector2 opponentPosition)
    {
        PlayerStepType = playerStepType;
        OpponentStepType = opponentStepType;
        PlayerPosition = playerPosition;
        OpponentPosition = opponentPosition;
    }
}
