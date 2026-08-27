
[System.Serializable]
public class MatchRoundRecord
{
    public int RoundNumber { get; private set; }
    public float PlayerHpLostRate { get; private set; }
    public float OpponentHpLostRate { get; private set; }

    public MatchRoundRecord(int roundNumber)
    {
        RoundNumber = roundNumber;
    }

    public void SetHpLostRates(float playerHpLostRate, float opponentHpLostRate)
    {
        PlayerHpLostRate = playerHpLostRate;

        OpponentHpLostRate = opponentHpLostRate;
    }
}
