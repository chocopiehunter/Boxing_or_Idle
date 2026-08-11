using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    public MatchState CurrentState { get; private set; } = MatchState.None;
    public FighterModel PlayerFighter { get; private set; }
    public FighterData OpponentData { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public bool TryScheduleMatch(FighterModel player, FighterData opponent)
    {
        return true;
    }

    public void ClearMatch()
    {

    }
}
