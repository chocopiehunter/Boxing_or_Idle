
[System.Serializable]
public class MatchRoundRecord
{
    public int RoundNumber { get; private set; }
    public float PlayerHpLostRate { get; private set; }
    public float OpponentHpLostRate { get; private set; }
    public int PlayerSignificantStrikesLanded { get; private set; }
    public int OpponentSignificantStrikesLanded { get; private set; }

    public MatchRoundRecord(int roundNumber)
    {
        RoundNumber = roundNumber;
    }

    public void SetHpLostRates(float playerHpLostRate, float opponentHpLostRate)
    {
        PlayerHpLostRate = playerHpLostRate;

        OpponentHpLostRate = opponentHpLostRate;
    }

    public void SetSignificantStrikes(int playerSignificantStrikesLanded, int opponentSignificantStrikesLanded)
    {
        if (playerSignificantStrikesLanded < 0)
        {
            playerSignificantStrikesLanded = 0;
        }

        if (opponentSignificantStrikesLanded < 0)
        {
            opponentSignificantStrikesLanded = 0;
        }

        PlayerSignificantStrikesLanded = playerSignificantStrikesLanded;
        OpponentSignificantStrikesLanded = opponentSignificantStrikesLanded;
    }
}
