using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }

    public MatchState CurrentState { get; private set; } = MatchState.None;
    public FighterModel PlayerFighter { get; private set; }
    public FighterData OpponentData { get; private set; }
    public MatchRuleData CurrentRuleData { get; private set; }
    public int CurrentRound { get; private set; }
    public float RoundRemainingSeconds { get; private set; }

    public MatchResult LastResult { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (CurrentState != MatchState.RoundInProgress)
        {
            return;
        }

        UpdateRoundTime();
    }

    public bool TryScheduleMatch(FighterModel player, FighterData opponent, MatchRuleData ruleData)
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

        if (ruleData == null)
        {
            Debug.LogError("경기 신청 실패. 경기 규칙 데이터 없음");
            return false;
        }

        if (ruleData.RoundCount <= 0 || ruleData.RoundSeconds <= 0f || ruleData.RoundBreakSeconds < 0f)
        {
            Debug.LogError($"경기 신청 실패. 경기 규칙 데이터 오류 {ruleData.Id}");
            return false;
        }

        PlayerFighter = player;
        OpponentData = opponent;
        CurrentRuleData = ruleData;
        CurrentState = MatchState.Scheduled;

        Debug.Log($"경기 신청 완료 {player.Name} vs {opponent.Name} / 경기 규칙: {ruleData.Name}");
        return true;
    }

    public bool TryStartMatch()
    {
        if (CurrentState != MatchState.Scheduled)
        {
            Debug.LogError($"경기 시작 실패. 현재 상태={CurrentState}");
            return false;
        }

        if (CurrentRuleData == null)
        {
            Debug.LogError("경기 시작 실패. 경기 규칙 데이터 없음");
            return false;
        }

        CurrentRound = 1;

        StartCurrentRound();

        return true;
    }

    private void StartCurrentRound()
    {
        RoundRemainingSeconds = CurrentRuleData.RoundSeconds;

        CurrentState = MatchState.RoundInProgress;

        Debug.Log($"{CurrentRound}라운드 시작 / 남은 시간 {RoundRemainingSeconds}초");
    }

    private void UpdateRoundTime()
    {

    }

    private void EndCurrentRound()
    {

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

        float playerCurrentHp = PlayerFighter.Hp;
        float opponentCurrentHp = OpponentData.Hp;

        for (int round = 1; round <= CurrentRuleData.RoundCount; round++)
        {
            float playerRemainingHp = HpCalculator.CalculateRemainingHp(playerCurrentHp, PlayerFighter.StandingDefense, OpponentData.StandingOffense);

            float opponentRemainingHp = HpCalculator.CalculateRemainingHp(opponentCurrentHp, OpponentData.StandingDefense, PlayerFighter.StandingOffense);

            float playerLostRate = HpCalculator.CalculateLostHpRate(playerCurrentHp, playerRemainingHp);
            float opponentLostRate = HpCalculator.CalculateLostHpRate(opponentCurrentHp, opponentRemainingHp);

            Debug.Log($"{round}라운드 {PlayerFighter.Name} {playerRemainingHp}/{PlayerFighter.Hp} (체력 {playerLostRate} 잃음) vs {OpponentData.Name} {opponentRemainingHp}/{OpponentData.Hp} (체력 {opponentLostRate} 잃음)");

            // KO시 즉시 경기 종료 로직
            if (playerRemainingHp <= 0f && opponentRemainingHp <= 0f)
            {
                CompleteMatch(MatchResult.Draw);
                Debug.Log("동시 KO. 경기 종료 무승부");
                return true;
            }

            if(playerRemainingHp <= 0f)
            {
                CompleteMatch(MatchResult.Lose);
                Debug.Log($"{PlayerFighter.Name} {round}라운드 KO 패배");
                return true;
            }

            if(opponentRemainingHp <= 0f)
            {
                CompleteMatch(MatchResult.Win);
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

            playerCurrentHp = playerRemainingHp;
            opponentCurrentHp = opponentRemainingHp;
        }

        CompleteMatch(JudgeMatchByRoundWins(playerRoundWins, opponentRoundWins));

        Debug.Log($"경기 종료 {playerRoundWins} 대 {opponentRoundWins} {LastResult}");

        return true;
    }

    private void CompleteMatch(MatchResult result)
    {
        LastResult = result;
        CurrentState = MatchState.Finished;

        if (GameManager.Instance == null)
        {
            Debug.LogError($"GameManager가 없어서 경기 결과를 기록못함");
            return;
        }

        GameManager.Instance.RecordMatchResult(result);
    }

    private MatchResult JudgeRoundByLostHpRate(float playerLostRate, float opponentLostRate)
    {
        if (playerLostRate < opponentLostRate) 
        {
            return MatchResult.Win;
        }

        return MatchResult.Lose;
    }

    private MatchResult JudgeMatchByRoundWins(int playerRoundWins, int opponentRoundWins)
    {
        if (playerRoundWins > opponentRoundWins) 
        {
            return MatchResult.Win;
        }

        return MatchResult.Lose;
    }

    public void ClearMatch()
    {
        PlayerFighter = null;
        OpponentData = null;
        CurrentRuleData = null;
        CurrentState = MatchState.None;
    }
}
