using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    private const int DefaultRoundCount = 3;

    public MatchState CurrentState { get; private set; } = MatchState.None;
    public FighterModel PlayerFighter { get; private set; }
    public FighterData OpponentData { get; private set; }

    public MatchResult LastResult { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public bool TryScheduleMatch(FighterModel player, FighterData opponent)
    {
        if (player == null)
        {
            Debug.LogError("경기 신청 실패 플레이어 선수없음");
            return false;
        }

        if (opponent == null)
        {
            Debug.LogError("경기 신청 실패 상대 데이터 없음");
            return false;
        }

        if (CurrentState != MatchState.None && CurrentState != MatchState.Finished)
        {
            Debug.LogWarning("경기 신청 실패. 이미 예정된 경기가 있습니다");
            return false;
        }

        PlayerFighter = player;
        OpponentData = opponent;
        CurrentState = MatchState.Scheduled;

        Debug.Log($"경기 신청 완료 {player.Name} vs {opponent.Name}");
        return true;
    }

    public bool TryJudgeMatch()
    {

        return true;
    }

    private MatchResult JudgeRoundByLostHpRate(float playerLostRate, float opponentLostRate)
    {

        return MatchResult.Lose;
    }

    private MatchResult JudgeMatchByRoundWins(int playerRoundWins, int opponentRoundWins)
    {

        return MatchResult.Lose;
    }

    public void ClearMatch()
    {
        PlayerFighter = null;
        OpponentData = null;
        CurrentState = MatchState.None;
    }
}
