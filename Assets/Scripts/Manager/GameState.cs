using UnityEngine;

public class GameState
{
    public GameFlowState CurrentState { get; private set; } = GameFlowState.Title;

    public void ChangeState(GameFlowState newState)
    {
        CurrentState = newState;
    }
}
