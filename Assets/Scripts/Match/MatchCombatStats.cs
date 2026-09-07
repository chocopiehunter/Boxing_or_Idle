using UnityEngine;

[System.Serializable]
public class MatchCombatStats
{
    public int SignificantStrikesSucceeded { get; private set; }
    public int TotalStrikesSucceeded { get; private set; }
    public int TotalStrikesAttempted { get; private set; }
    public int TakedownsSucceeded { get; private set; }
    public int TakedownsAttempted { get; private set; }
    public float ControlSeconds { get; private set; }
    public int Knockdowns { get; private set; }
    public int SubmissionAttempts { get; private set; }

    public void RecordStrike(bool isSucceeded, bool isSignificantStrike)
    {
        TotalStrikesAttempted = TotalStrikesAttempted + 1;

        if (isSucceeded == false)
        {
            return;
        }

        TotalStrikesSucceeded = TotalStrikesSucceeded + 1;

        if (isSignificantStrike)
        {
            SignificantStrikesSucceeded = SignificantStrikesSucceeded + 1;
        }
    }

    public void RecordTakedown(bool takedownSucceeded)
    {
        TakedownsAttempted = TakedownsAttempted + 1;

        if (takedownSucceeded == false)
        {
            return;
        }

        TakedownsSucceeded = TakedownsSucceeded + 1;
    }

    public void RecordControlTime(float passedSeconds)
    {
        if (passedSeconds <= 0f)
        {
            return;
        }

        ControlSeconds = ControlSeconds + passedSeconds;
    }

    public void RecordSubmissionAttempt()
    {
        SubmissionAttempts = SubmissionAttempts + 1;
    }

    public void SetStats(int significantStrikesSucceeded, int totalStrikesSucceeded, int totalStrikesAttempted, int takedownsSucceeded, int takedownsAttempted, float controlSeconds, int knockdowns, int submissionAttempts)
    {
        SignificantStrikesSucceeded = significantStrikesSucceeded;
        TotalStrikesSucceeded = totalStrikesSucceeded;
        TotalStrikesAttempted = totalStrikesAttempted;
        TakedownsSucceeded = takedownsSucceeded;
        TakedownsAttempted = takedownsAttempted;
        ControlSeconds = controlSeconds;
        Knockdowns = knockdowns;
        SubmissionAttempts = submissionAttempts;
    }
}
