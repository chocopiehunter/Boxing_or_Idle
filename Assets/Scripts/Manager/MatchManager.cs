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
        if (CurrentState != MatchState.Scheduled)
        {
            Debug.LogError($"경기 판정 실패. 현재 상태= {CurrentState}");
            return false;
        }

        int playerRoundWins = 0;
        int opponentRoundWins = 0;

        for (int round = 1; round <= DefaultRoundCount; round++)
        {
            float playerRemainingHp = HpCalculator.CalculateRemainingHp(OpponentData.Hp, OpponentData.Def, PlayerFighter.Atk);

            float opponentRemainingHp = HpCalculator.CalculateRemainingHp(OpponentData.Hp, OpponentData.Def, PlayerFighter.Atk);

            float playerLostRate = HpCalculator.CalculateLostHpRate(PlayerFighter.Hp, playerRemainingHp);
            float opponentLostRate = HpCalculator.CalculateLostHpRate(OpponentData.Hp, opponentRemainingHp);

            Debug.Log($"{round}라운드 {PlayerFighter.Name} {playerRemainingHp}/{PlayerFighter.Hp} (체력 {playerLostRate} 잃음");

            // KO시 즉시 경기 종료 로직
            if (playerRemainingHp <= 0f && opponentRemainingHp <= 0f)
            {
                LastResult = MatchResult.Draw;
                CurrentState = MatchState.Finished;
                Debug.Log("동시 KO. 경기 종료 무승부");
                return true;
            }

            if(playerRemainingHp <= 0f)
            {
                LastResult = MatchResult.Lose;
                CurrentState = MatchState.Finished;
                Debug.Log($"{PlayerFighter.Name} {round}라운드 KO 패배");
                return true;
            }

            if(opponentRemainingHp <= 0f)
            {
                LastResult = MatchResult.Win;
                CurrentState = MatchState.Finished;
                Debug.Log($"{PlayerFighter.Name} {round}라운드 KO 승리");
                return true;
            }

            MatchResult roundResult = JudgeRoundByLostHpRate(playerLostRate, opponentLostRate);
            if (roundResult == MatchResult.Win)
            {
                playerRoundWins = playerRoundWins + 1;
            }
            else
            {
                opponentRoundWins = opponentRoundWins + 1;
            }

            Debug.Log($"{round}라운드 판정 : {roundResult}");
        }

        LastResult = JudgeMatchByRoundWins(playerRoundWins, opponentRoundWins);
        CurrentState = MatchState.Finished;

        Debug.Log($"경기 종료 {playerRoundWins} 대 {opponentRoundWins} {LastResult}");

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
