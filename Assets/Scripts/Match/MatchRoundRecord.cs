
[System.Serializable]
public class MatchRoundRecord
{
    public int RoundNumber { get; private set; }
    public float PlayerHpLostRate { get; private set; }
    public float OpponentHpLostRate { get; private set; }
    public int PlayerSignificantStrikesSucceeded { get; private set; }
    public int OpponentSignificantStrikesSucceeded { get; private set; }
    public int PlayerTakedownsSucceeded { get; private set; }
    public int PlayerTakedownsAttempted { get; private set; }
    public int OpponentTakedownsSucceeded { get; private set; }
    public int OpponentTakedownsAttempted { get; private set; }
    public float PlayerControlSeconds { get; private set; }
    public float OpponentControlSeconds { get; private set; }

    public MatchRoundRecord(int roundNumber)
    {
        RoundNumber = roundNumber;
    }

    public void SetHpLostRates(float playerHpLostRate, float opponentHpLostRate)
    {
        PlayerHpLostRate = playerHpLostRate;

        OpponentHpLostRate = opponentHpLostRate;
    }

    public void SetSignificantStrikes(int playerSignificantStrikesSucceeded, int opponentSignificantStrikesSucceeded)
    {
        if (playerSignificantStrikesSucceeded < 0)
        {
            playerSignificantStrikesSucceeded = 0;
        }

        if (opponentSignificantStrikesSucceeded < 0)
        {
            opponentSignificantStrikesSucceeded = 0;
        }

        PlayerSignificantStrikesSucceeded = playerSignificantStrikesSucceeded;
        OpponentSignificantStrikesSucceeded = opponentSignificantStrikesSucceeded;
    }

    public void SetTakedowns(int playerTakedownsSucceeded, int playerTakedownsAttempted, int opponentTakedownsSucceeded, int opponentTakedownsAttempted)
    {
        if (playerTakedownsAttempted < 0)
        {
            playerTakedownsAttempted = 0;
        }

        if (opponentTakedownsAttempted < 0)
        {
            opponentTakedownsAttempted = 0;
        }

        if (playerTakedownsSucceeded < 0)
        {
            playerTakedownsSucceeded = 0;
        }

        if (opponentTakedownsSucceeded < 0)
        {
            opponentTakedownsSucceeded = 0;
        }

        if (playerTakedownsSucceeded > playerTakedownsAttempted)
        {
            playerTakedownsSucceeded = playerTakedownsAttempted;
        }

        if (opponentTakedownsSucceeded > opponentTakedownsAttempted)
        {
            opponentTakedownsSucceeded = opponentTakedownsAttempted;
        }

        PlayerTakedownsSucceeded = playerTakedownsSucceeded;
        PlayerTakedownsAttempted = playerTakedownsAttempted;

        OpponentTakedownsSucceeded = opponentTakedownsSucceeded;
        OpponentTakedownsAttempted = opponentTakedownsAttempted;
    }

    public void SetControlSeconds(float playerControlSeconds, float opponentControlSeconds)
    {
        if (playerControlSeconds < 0f)
        {
            playerControlSeconds = 0f;
        }

        if (opponentControlSeconds < 0f)
        {
            opponentControlSeconds = 0f;
        }

        PlayerControlSeconds = playerControlSeconds;
        OpponentControlSeconds = opponentControlSeconds;
    }
}
