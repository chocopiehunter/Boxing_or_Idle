using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public GameFlowState CurrentState { get; private set; } = GameFlowState.Title;
    public GameSpeedType CurrentSpeed { get; private set; } = GameSpeedType.Normal;

    // 프로토타입용 1회차 엔딩 클리어 여부
    public bool HasClearedFirstGame { get; private set; } = false;

    public int NgPlusPoints { get; private set; } = 0;
    public int TotalWinCount { get; private set; }
    public int CurrentWinStreak { get; private set; }

    private List<string> _receivedNgAchievementIds = new List<string>();

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
        HasClearedFirstGame = cleared;
    }

    public void RecordMatchResult(MatchResult result)
    {
        if (result == MatchResult.Win)
        {
            TotalWinCount = TotalWinCount + 1;
            CurrentWinStreak = CurrentWinStreak + 1;
            return;
        }

        CurrentWinStreak = 0;
    }

    public void AddNgPlusPoints(int amount)
    {
        NgPlusPoints = NgPlusPoints + amount;
    }

    public bool HasReceivedNgAchievement(string id)
    {
        return _receivedNgAchievementIds.Contains(id);
    }

    public void AddReceivedNgAchievement(string id)
    {
        if (HasReceivedNgAchievement(id) == true)
        {
            return;
        }

        _receivedNgAchievementIds.Add(id);
    }
}
