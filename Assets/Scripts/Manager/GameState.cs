using UnityEngine;

public class GameState
{
    public GameFlowState CurrentState { get; private set; } = GameFlowState.Title;
    public GameSpeedType CurrentSpeed { get; private set; } = GameSpeedType.Normal;

    public void ChangeState(GameFlowState newState)
    {
        CurrentState = newState;
    }

    public void ChangeSpeed(GameSpeedType newSpeed)
    {
        CurrentSpeed = newSpeed;
    }
}
