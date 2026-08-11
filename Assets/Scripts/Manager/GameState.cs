using UnityEngine;

public class GameState
{
    public GameFlowState CurrentState { get; private set; } = GameFlowState.Title;
    public GameSpeedType CurrentSpeed { get; private set; } = GameSpeedType.Normal;

    // 프로토타입용 1회차 엔딩 클리어 여부
    public bool HasClearedFirstGame { get; private set; } = false;

    public void ChangeState(GameFlowState newState)
    {
        CurrentState = newState;
    }

    public void ChangeSpeed(GameSpeedType newSpeed)
    {
        CurrentSpeed = newSpeed;
    }

    public void SetFirstGameCleared(bool cleared)
    {
        HasClearedFirstGame = true;
    }
}
