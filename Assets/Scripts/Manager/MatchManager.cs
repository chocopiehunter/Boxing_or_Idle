using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }
    private IMatchJudge _matchJudge;
    private List<MatchRoundRecord> _roundRecords = new List<MatchRoundRecord>();

    public MatchState CurrentState { get; private set; } = MatchState.None;
    public FighterModel PlayerFighter { get; private set; }
    public FighterData OpponentData { get; private set; }
    public MatchRuleData CurrentRuleData { get; private set; }
    public MatchStrategyData CurrentStrategyData { get; private set; }
    public int CurrentRound { get; private set; }
    public float RoundRemainingSeconds { get; private set; }
    public float RoundBreakRemainingSeconds { get; private set; }
    public float PlayerCurrentHp { get; private set; }
    public float OpponentCurrentHp { get; private set; }

    public MatchResult LastResult { get; private set; } = MatchResult.None;

    private void Awake()
    {
        Instance = this;

        _matchJudge = new MatchJudge();
    }

    private void Update()
    {
        if (CurrentState == MatchState.RoundInProgress)
        {
            UpdateRoundTime(Time.unscaledDeltaTime);
            return;
        }

        if (CurrentState == MatchState.RoundBreak)
        {
            UpdateRoundBreakTime(Time.unscaledDeltaTime);
        }
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

        bool initializeSuccess = InitializeMatchRuntime();
        if (initializeSuccess == false)
        {
            return false;
        }

        CurrentRound = 1;
        RoundBreakRemainingSeconds = 0;

        StartCurrentRound();

        return true;
    }

    private bool InitializeMatchRuntime()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("경기 초기화 실패. GameDataManager없음");
            return false;
        }

        MatchStrategyData defaultStrategyData = GameDataManager.Instance.GetDefaultMatchStrategyData();
        if (defaultStrategyData == null)
        {
            Debug.LogError("경기 초기화 실패. 기본 경기 전략 없음");
            return false;
        }

        PlayerCurrentHp = PlayerFighter.Hp;
        OpponentCurrentHp = OpponentData.Hp;
        LastResult = MatchResult.None;

        CurrentStrategyData = defaultStrategyData;

        _roundRecords.Clear();

        Debug.Log($"기본 경기 전략 적용 : {CurrentStrategyData.Name}");
        return true;
    }

    public bool TryChangeMatchStrategy(string strategyId)
    {
        if (CurrentState != MatchState.RoundBreak)
        {
            Debug.LogWarning($"경기 전략 변경 실패. 현재 상태={CurrentState}");
            return false;
        }

        if (string.IsNullOrEmpty(strategyId))
        {
            Debug.LogError("경기 전략 변경 실패. 전략 Id 없음");
            return false;
        }

        if (GameDataManager.Instance == null)
        {
            Debug.LogError("경기 전략 변경 실패. GameDataManager 없음");
            return false;
        }

        MatchStrategyData strategyData = GameDataManager.Instance.GetMatchStrategyData(strategyId);
        if (strategyData == null)
        {
            Debug.LogError($"경기 전략 변경 실패. MatchStrategyData 없음 Id={strategyId}");
            return false;
        }

        CurrentStrategyData = strategyData;

        Debug.Log($"경기 전략 변경 완료: {CurrentStrategyData.Name}");
        return true;
    }

    private void StartCurrentRound()
    {
        RoundRemainingSeconds = CurrentRuleData.RoundSeconds;

        CurrentState = MatchState.RoundInProgress;

        Debug.Log($"{CurrentRound}라운드 시작 / 남은 시간 {RoundRemainingSeconds}초");
    }

    private void UpdateRoundTime(float passedSeconds)
    {
        if (passedSeconds <= 0f)
        {
            return;
        }

        RoundRemainingSeconds = RoundRemainingSeconds - passedSeconds;

        if (RoundRemainingSeconds > 0f)
        {
            return;
        }

        RoundRemainingSeconds = 0f;
        EndCurrentRound();
    }

    private void EndCurrentRound()
    {
        bool resolveSuccess = TryResolveCurrentRound();
        if (resolveSuccess == false)
        {
            return;
        }

        if (CurrentState == MatchState.Finished)
        {
            return;
        }

        if (CurrentRound >= CurrentRuleData.RoundCount)
        {
            TryCompleteMatchByDecision();
            return;
        }

        RoundBreakRemainingSeconds = CurrentRuleData.RoundBreakSeconds;

        CurrentState = MatchState.RoundBreak;

        Debug.Log($"{CurrentRound}라운드 종료 / 휴식시간 {RoundBreakRemainingSeconds}초");
    }

    private bool TryResolveCurrentRound()
    {
        float playerRemainingHp = HpCalculator.CalculateRemainingHp(PlayerCurrentHp, PlayerFighter.StandingDefense, OpponentData.StandingOffense);
        float opponentRemainingHp = HpCalculator.CalculateRemainingHp(OpponentCurrentHp, OpponentData.StandingDefense, PlayerFighter.StandingOffense);
        float playerLostRate = HpCalculator.CalculateLostHpRate(PlayerCurrentHp, playerRemainingHp);
        float opponentLostRate = HpCalculator.CalculateLostHpRate(OpponentCurrentHp, opponentRemainingHp);

        PlayerCurrentHp = playerRemainingHp;
        OpponentCurrentHp = opponentRemainingHp;
        MatchRoundRecord roundRecord = new MatchRoundRecord(CurrentRound);
        roundRecord.SetHpLostRates(playerLostRate, opponentLostRate);
        _roundRecords.Add(roundRecord);

        Debug.Log($"{CurrentRound}라운드 {PlayerFighter.Name} {PlayerCurrentHp}/{PlayerFighter.Hp} (체력 {playerLostRate}잃음 vs {OpponentData.Name} {OpponentCurrentHp}/{OpponentData.Hp} (체력 {opponentLostRate}잃음)");
        
        if (PlayerCurrentHp <= 0f && OpponentCurrentHp <= 0f)
        {
            CompleteMatch(MatchResult.Draw);
            Debug.Log("동시 KO. 경기 종료 무승부");
            return true;
        }

        if (PlayerCurrentHp <= 0f)
        {
            CompleteMatch(MatchResult.Lose);
            Debug.Log($"{PlayerFighter.Name} {CurrentRound}라운드 KO 패배");
            return true;
        }

        if (OpponentCurrentHp <= 0f)
        {
            CompleteMatch(MatchResult.Win);
            Debug.Log($"{PlayerFighter.Name} {CurrentRound}라운드 KO 승리");
            return true;
        }

        MatchResult roundResult = _matchJudge.JudgeRound(roundRecord);
        if (roundResult == MatchResult.None)
        {
            StopMatchByError($"{CurrentRound}라운드 판정 결과 없음");
            return false;
        }

        Debug.Log($"{CurrentRound}라운드 판정 : {roundResult}");

        return true;
    }

    private bool TryCompleteMatchByDecision()
    {
        MatchResult matchResult = _matchJudge.JudgeMatch(_roundRecords);

        if (matchResult == MatchResult.None)
        {
            StopMatchByError("에러로 인해 최종 경기 판정 결과 없음");
            return false;
        }

        CompleteMatch(matchResult);

        Debug.Log($"경기 종료 / 판정 라운드 수 {_roundRecords.Count} / 결과 {LastResult}");

        return true;
    }

    private void UpdateRoundBreakTime(float passedSeconds)
    {
        if (passedSeconds <= 0f)
        {
            return;
        }

        RoundBreakRemainingSeconds = RoundBreakRemainingSeconds - passedSeconds;

        if (RoundBreakRemainingSeconds > 0f)
        {
            return;
        }

        RoundBreakRemainingSeconds = 0f;

        CurrentRound = CurrentRound + 1;

        StartCurrentRound();
    }

    public bool TryJudgeMatch()
    {
        if (CurrentState != MatchState.Scheduled)
        {
            Debug.LogError($"경기 판정 실패. 현재 상태= {CurrentState}");
            return false;
        }

        bool initializeSuccess = InitializeMatchRuntime();
        if (initializeSuccess == false)
        {
            return false;
        }

        for (int round = 1; round <= CurrentRuleData.RoundCount; round++)
        {
            CurrentRound = round;

            bool resolveSuccess = TryResolveCurrentRound();
            if (resolveSuccess == false)
            {
                return false;
            }

            if (CurrentState == MatchState.Finished)
            {
                return true;
            }
        }

        return TryCompleteMatchByDecision();
    }

    private void StopMatchByError(string errorMessage)
    {
        LastResult = MatchResult.None;
        CurrentState = MatchState.Finished;
        Debug.LogError($"경기 처리 중단. {errorMessage}");
    }

    private void CompleteMatch(MatchResult result)
    {
        if (result == MatchResult.None)
        {
            StopMatchByError("유효하지 않은 경기 결과가 전달됨");
            return;
        }

        LastResult = result;
        CurrentState = MatchState.Finished;

        if (GameManager.Instance == null)
        {
            Debug.LogError($"GameManager가 없어서 경기 결과를 기록못함");
            return;
        }

        GameManager.Instance.RecordMatchResult(result);
    }

    public void ClearMatch()
    {
        PlayerFighter = null;
        OpponentData = null;
        CurrentRuleData = null;
        CurrentStrategyData = null;

        CurrentRound = 0;
        RoundRemainingSeconds = 0f;
        RoundBreakRemainingSeconds = 0f;

        PlayerCurrentHp = 0f;
        OpponentCurrentHp = 0f;
        LastResult = MatchResult.None;
        _roundRecords.Clear();

        CurrentState = MatchState.None;
    }
}
