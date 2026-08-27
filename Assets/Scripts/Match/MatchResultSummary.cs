using UnityEngine;

[System.Serializable]
public class MatchResultSummary
{
    public string PlayerName {  get; private set; }
    public string OpponentName { get; private set; }
    public MatchResult Result { get; private set; }
    public MatchFinishType FinishType { get; private set; }
    public int FinishedRound { get; private set; }
    public float PassedRoundSeconds { get; private set; }
    public MatchCombatStats PlayerStats { get; private set; }
    public MatchCombatStats OpponentStats { get; private set; }
    public int GoldReward { get; private set; }

    public MatchResultSummary (string playerName, string opponentName)
    {
        PlayerName = playerName;
        OpponentName = opponentName;
        Result = MatchResult.None;
        FinishType = MatchFinishType.None;
        PlayerStats = new MatchCombatStats();
        OpponentStats = new MatchCombatStats();
    }

    public void SetMatchResult(MatchResult result, MatchFinishType finishType, int finishedRound, float passedRoundSeconds)
    {
        Result = result;
        FinishType = finishType;
        FinishedRound = finishedRound;
        PassedRoundSeconds = passedRoundSeconds;
    }

    public void SetCombatStats(MatchCombatStats playerStats, MatchCombatStats opponentStats)
    {
        if (playerStats != null)
        {
            PlayerStats = playerStats;
        }

        if (opponentStats != null)
        {
            OpponentStats = opponentStats;
        }
    }

    public void SetGoldReward(int goldReward)
    {
        if (goldReward < 0)
        {
            return;
        }

        GoldReward = goldReward;
    }
}
