using UnityEngine;

[System.Serializable]
public class MatchCombatStats
{
    public int SignificantStrikesLanded { get; private set; }
    public int TotalStrikesLanded { get; private set; }
    public int TotalStrikesAttempted { get; private set; }
    public int TakedownsLanded { get; private set; }
    public int TakedownsAttempted { get; private set; }
    public float ControlSeconds { get; private set; }
    public int Knockdowns { get; private set; }
    public int SubmissionAttempts { get; private set; }

    public void RecordStrike(bool isLanded, bool isSignificantStrike)
    {
        TotalStrikesAttempted = TotalStrikesAttempted + 1;

        if (isLanded == false)
        {
            return;
        }

        TotalStrikesLanded = TotalStrikesLanded + 1;

        if (isSignificantStrike)
        {
            SignificantStrikesLanded = SignificantStrikesLanded + 1;
        }
    }

    public void RecordTakedown(bool takedownSucceeded)
    {
        TakedownsAttempted = TakedownsAttempted + 1;

        if (takedownSucceeded == false)
        {
            return;
        }

        TakedownsLanded = TakedownsLanded + 1;
    }

    public void SetStats(int significantStrikesLanded, int totalStrikesLanded, int totalStrikesAttempted, int takedownsLanded, int takedownsAttempted, float controlSeconds, int knockdowns, int submissionAttempts)
    {
        SignificantStrikesLanded = significantStrikesLanded;
        TotalStrikesLanded = totalStrikesLanded;
        TotalStrikesAttempted = totalStrikesAttempted;
        TakedownsLanded = takedownsLanded;
        TakedownsAttempted = takedownsAttempted;
        ControlSeconds = controlSeconds;
        Knockdowns = knockdowns;
        SubmissionAttempts = submissionAttempts;
    }
}
