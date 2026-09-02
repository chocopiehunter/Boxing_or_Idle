using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance { get; private set; }
    private IMatchJudge _matchJudge;
    private MatchCombatRunner _combatRunner;
    private MatchFighterModel _playerMatchFighter;
    private MatchFighterModel _opponentMatchFighter;
    private List<MatchRoundRecord> _roundRecords = new List<MatchRoundRecord>();
    private float _playerRoundStartHp;
    private float _opponentRoundStartHp;
    private int _playerRoundStartSignificantStrikes;
    private int _opponentRoundStartSignificantStrikes;
    private int _playerRoundStartTakedownsSucceeded;
    private int _playerRoundStartTakedownsAttempted;
    private int _opponentRoundStartTakedownsSucceeded;
    private int _opponentRoundStartTakedownsAttempted;

    public MatchState CurrentState { get; private set; } = MatchState.None;
    public FighterModel PlayerFighter { get; private set; }
    public FighterData OpponentData { get; private set; }
    public MatchRuleData CurrentRuleData { get; private set; }
    public MatchStrategyData CurrentStrategyData { get; private set; }
    public MatchCombatModel CombatModel { get; private set; }
    public int CurrentRound { get; private set; }
    public float RoundRemainingSeconds { get; private set; }
    public float RoundBreakRemainingSeconds { get; private set; }
    public float PlayerCurrentHp
    {
        get
        {
            if (_playerMatchFighter == null)
            {
                return 0;
            }

            return _playerMatchFighter.CurrentHp;
        }
    }
    public float OpponentCurrentHp
    {
        get
        {
            if (_opponentMatchFighter == null)
            {
                return 0;
            }

            return _opponentMatchFighter.CurrentHp;
        }
    }

    public MatchResult LastResult { get; private set; } = MatchResult.None;
    public MatchResultSummary LastResultSummary { get; private set; } // 결과요약

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

        if(ruleData.RoundCount <= 0 || ruleData.RoundSeconds <= 0f || ruleData.RoundBreakSeconds < 0f || ruleData.ActionIntervalSeconds <= 0f)
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

        LastResult = MatchResult.None;
        LastResultSummary = null;

        CurrentStrategyData = defaultStrategyData;
        CombatModel = new MatchCombatModel();

        List<string> opponentSkillIds = GameDataManager.Instance.GetStartingSkillIds(OpponentData);

        _playerMatchFighter = new MatchFighterModel(
            MatchFighterSide.Player,
            PlayerFighter.Hp,
            PlayerFighter.Stamina,
            PlayerFighter.StandingOffense,
            PlayerFighter.StandingDefense,
            PlayerFighter.WrestlingOffense,
            PlayerFighter.WrestlingDefense,
            PlayerFighter.JiuJitsuOffense,
            PlayerFighter.JiuJitsuDefense,
            PlayerFighter.OwnedSkillIds);

        _opponentMatchFighter = new MatchFighterModel(
            MatchFighterSide.Opponent,
            OpponentData.Hp,
            OpponentData.Stamina,
            OpponentData.StandingOffense,
            OpponentData.StandingDefense,
            OpponentData.WrestlingOffense,
            OpponentData.WrestlingDefense,
            OpponentData.JiuJitsuOffense,
            OpponentData.JiuJitsuDefense,
            opponentSkillIds);

        MatchUsableSkillFinder usableSkillFinder = new MatchUsableSkillFinder(GameDataManager.Instance);

        _combatRunner = new MatchCombatRunner(CombatModel, _playerMatchFighter, _opponentMatchFighter, usableSkillFinder, CurrentRuleData.ActionIntervalSeconds);

        _roundRecords.Clear();

        Debug.Log($"경기 선수 기술 구성 완료 / 플레이어 {_playerMatchFighter.OwnedSkillIds.Count}개 / 상대 {_opponentMatchFighter.OwnedSkillIds.Count}개");
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
        CombatModel.StartRound();
        _combatRunner.Reset();

        _playerRoundStartHp = PlayerCurrentHp;
        _opponentRoundStartHp = OpponentCurrentHp;

        MatchCombatStats playerCombatStats = _combatRunner.GetCombatStats(MatchFighterSide.Player);
        MatchCombatStats opponentCombatStats = _combatRunner.GetCombatStats(MatchFighterSide.Opponent);

        if (playerCombatStats == null || opponentCombatStats == null)
        {
            StopMatchByError("라운드 시작 실패. 경기 통계 없음");
            return;
        }

        _playerRoundStartSignificantStrikes = playerCombatStats.SignificantStrikesSucceeded;
        _opponentRoundStartSignificantStrikes = opponentCombatStats.SignificantStrikesSucceeded;
        _playerRoundStartTakedownsSucceeded = playerCombatStats.TakedownsSucceeded;
        _playerRoundStartTakedownsAttempted = playerCombatStats.TakedownsAttempted;
        _opponentRoundStartTakedownsSucceeded = opponentCombatStats.TakedownsSucceeded;
        _opponentRoundStartTakedownsAttempted = opponentCombatStats.TakedownsAttempted;

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

        if (_combatRunner == null)
        {
            StopMatchByError("MatchCombatRunner 없음");
            return;
        }

        bool actionTimeReached = _combatRunner.UpdateCombatTime(passedSeconds);

        if (actionTimeReached == true)
        {
            RunNextCombatAction();
        }

        if (CurrentState != MatchState.RoundInProgress)
        {
            return;
        }

        ApplyGroundBottomStaminaLoss(passedSeconds);

        RoundRemainingSeconds = RoundRemainingSeconds - passedSeconds;

        if (RoundRemainingSeconds > 0f)
        {
            return;
        }

        RoundRemainingSeconds = 0f;
        EndCurrentRound();
    }

    private void ApplyGroundBottomStaminaLoss(float passedSeconds)
    {
        if (CombatModel == null)
        {
            return;
        }

        if (CombatModel.CurrentSituation != MatchSituation.Ground)
        {
            return;
        }

        GroundPositionData currentPositionData = GameDataManager.Instance.GetGroundPositionData(CombatModel.CurrentGroundPosition);

        if (currentPositionData == null)
        {
            return;
        }

        MatchFighterModel bottomFighter = GetMatchFighter(CombatModel.BottomSide);

        if (bottomFighter == null)
        {
            return;
        }

        float staminaLoss = currentPositionData.BottomStaminaLossPerSecond * passedSeconds;
        bottomFighter.UseStamina(staminaLoss);
    }

    private MatchFighterModel GetMatchFighter(MatchFighterSide fighterSide)
    {
        if (fighterSide == MatchFighterSide.Player)
        {
            return _playerMatchFighter;
        }

        if (fighterSide == MatchFighterSide.Opponent)
        {
            return _opponentMatchFighter;
        }

        return null;
    }

    private void RunNextCombatAction()
    {
        if (_combatRunner.IsTakedownInProgress())
        {
            CombatActionResult takedownResult;

            bool takedownCompleted = _combatRunner.TryCompleteTakedown(out takedownResult);

            if (takedownCompleted == false)
            {
                Debug.LogError("테이크다운 공방 처리 실패");
                return;
            }

            LogCombatActionResult(takedownResult);
            return;
        }

        MatchCombatAction selectedAction;

        bool actionSelected = _combatRunner.TryCreateNextAction(CurrentStrategyData, null, out selectedAction);

        if (actionSelected == false)
        {
            Debug.Log($"전투 행동 선택 실패 / 상황 {CombatModel.CurrentSituation} / 사용 가능한 기술 없음");
            return;
        }

        if (selectedAction == null || selectedAction.SelectedSkill == null)
        {
            Debug.LogError("전투 행동 선택 결과 없음");
            return;
        }

        CombatActionResult actionResult;

        bool actionRunSuccess = _combatRunner.TryRunAction(selectedAction, out actionResult);

        if (actionRunSuccess == false)
        {
            Debug.LogWarning($"전투 행동 실행 실패 / 기술 {selectedAction.SelectedSkill.Name}");
            return;
        }

        LogCombatActionResult(actionResult);
        
        bool matchCompleted = TryCompleteMatchByKnockOut();
        if (matchCompleted)
        {
            return;
        }
    }

    private void LogCombatActionResult(CombatActionResult actionResult)
    {
        if (actionResult == null || actionResult.Action == null || actionResult.Action.SelectedSkill == null)
        {
            Debug.LogError("전투 행동 결과 로그 실패. 결과 데이터 없음");
            return;
        }

        string resultText = GetCombatActionResultText(actionResult.ResultType);

        Debug.Log($"전투 행동 결과 / 사용자 {actionResult.Action.SkillUserSide} / 대상 {actionResult.Action.TargetSide} / 기술 {actionResult.Action.SelectedSkill.Name} / 결과 {resultText} / 성공 확률 {actionResult.SuccessChance:F1}% / 피해 {actionResult.Damage:F1} / 상황 {CombatModel.CurrentSituation}, 레슬링 {CombatModel.CurrentWrestlingSituation}, 그라운드 {CombatModel.CurrentGroundPosition}, 상위 {CombatModel.TopSide}, 하위 {CombatModel.BottomSide}, 컨트롤 {CombatModel.GroundControllerSide} / 플레이어 HP {PlayerCurrentHp:F1}, 스태미나 {_playerMatchFighter.CurrentStamina:F1} / 상대 HP {OpponentCurrentHp:F1}, 스태미나 {_opponentMatchFighter.CurrentStamina:F1}");
    }

    private string GetCombatActionResultText(CombatActionResultType resultType)
    {
        if (resultType == CombatActionResultType.StrikeHit)
        {
            return "명중";
        }

        if (resultType == CombatActionResultType.StrikeMissed)
        {
            return "적중 실패";
        }

        if (resultType == CombatActionResultType.TakedownStarted)
        {
            return "태클 시도";
        }

        if (resultType == CombatActionResultType.TakedownSucceeded)
        {
            return "테이크다운 성공";
        }

        if (resultType == CombatActionResultType.TakedownDefended)
        {
            return "테이크다운 방어";
        }

        if (resultType == CombatActionResultType.TakedownToClinch)
        {
            return "태클 공방에서 클린치 전환";
        }

        if (resultType == CombatActionResultType.ClinchStarted)
        {
            return "클린치 시작";
        }

        if (resultType == CombatActionResultType.ClinchReversed)
        {
            return "클린치 주도권 역전";
        }

        if (resultType == CombatActionResultType.ClinchEscaped)
        {
            return "클린치 이탈";
        }

        if (resultType == CombatActionResultType.ClinchEscapeFailed)
        {
            return "클린치 탈출 실패";
        }

        if (resultType == CombatActionResultType.GroundPositionChangeSucceeded)
        {
            return "그라운드 포지션 전환 성공";
        }

        if (resultType == CombatActionResultType.GroundPositionChangeFailed)
        {
            return "그라운드 포지션 전환 실패";
        }

        if (resultType == CombatActionResultType.GroundEscaped)
        {
            return "그라운드 탈출 성공";
        }

        if (resultType == CombatActionResultType.GroundEscapeFailed)
        {
            return "그라운드 탈출 실패";
        }

        return "결과 없음";
    }

    private bool TryCompleteMatchByKnockOut()
    {
        bool playerKO = PlayerCurrentHp <= 0f;
        bool opponentKO = OpponentCurrentHp <= 0f;

        if (playerKO == false && opponentKO == false)
        {
            return false;
        }

        if (playerKO && opponentKO)
        {
            CompleteMatch(MatchResult.Draw, MatchFinishType.Draw);
            Debug.Log("동시 KO 경기종료 무승부");
            return true;
        }

        MatchFinishType finishType = MatchFinishType.KO;

        if (CombatModel != null && CombatModel.CurrentSituation == MatchSituation.Ground)
        {
            finishType = MatchFinishType.TKO;
        }

        if (playerKO)
        {
            CompleteMatch(MatchResult.Lose, finishType);

            Debug.Log($"{PlayerFighter.Name} {CurrentRound}라운드 {finishType} 패배");

            return true;
        }

        CompleteMatch(MatchResult.Win, finishType);

        Debug.Log($"{PlayerFighter.Name} {CurrentRound}라운드 {finishType} 승리");
        return true;
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

        CombatModel.Reset();

        RoundBreakRemainingSeconds = CurrentRuleData.RoundBreakSeconds;

        CurrentState = MatchState.RoundBreak;

        Debug.Log($"{CurrentRound}라운드 종료 / 휴식시간 {RoundBreakRemainingSeconds}초");
    }

    private bool TryResolveCurrentRound()
    {
        if (_playerMatchFighter == null || _opponentMatchFighter == null)
        {
            StopMatchByError("경기 선수 런타임 모델 없음");
            return false;
        }

        MatchCombatStats playerCombatStats = _combatRunner.GetCombatStats(MatchFighterSide.Player);
        MatchCombatStats opponentCombatStats = _combatRunner.GetCombatStats(MatchFighterSide.Opponent);

        if (playerCombatStats == null || opponentCombatStats == null)
        {
            StopMatchByError("라운드 기록 실패. 경기 통계 없음");
            return false;
        }

        int playerRoundSignificantStrikes = playerCombatStats.SignificantStrikesSucceeded - _playerRoundStartSignificantStrikes;
        int opponentRoundSignificantStrikes = opponentCombatStats.SignificantStrikesSucceeded - _opponentRoundStartSignificantStrikes;

        int playerRoundTakedownsSucceeded = playerCombatStats.TakedownsSucceeded - _playerRoundStartTakedownsSucceeded;
        int playerRoundTakedownsAttempted = playerCombatStats.TakedownsAttempted - _playerRoundStartTakedownsAttempted;
        int opponentRoundTakedownsSucceeded = opponentCombatStats.TakedownsSucceeded - _opponentRoundStartTakedownsSucceeded;
        int opponentRoundTakedownsAttempted = opponentCombatStats.TakedownsAttempted - _opponentRoundStartTakedownsAttempted;

        float playerLostRate = HpCalculator.CalculateLostHpRate(_playerMatchFighter.MaxHp, _playerRoundStartHp, PlayerCurrentHp);
        float opponentLostRate = HpCalculator.CalculateLostHpRate(_opponentMatchFighter.MaxHp, _opponentRoundStartHp, OpponentCurrentHp);

        MatchRoundRecord roundRecord = new MatchRoundRecord(CurrentRound);
        roundRecord.SetHpLostRates(playerLostRate, opponentLostRate);
        roundRecord.SetSignificantStrikes(playerRoundSignificantStrikes, opponentRoundSignificantStrikes);
        roundRecord.SetTakedowns(playerRoundTakedownsSucceeded, playerRoundTakedownsAttempted, opponentRoundTakedownsSucceeded, opponentRoundTakedownsAttempted);
        _roundRecords.Add(roundRecord);

        Debug.Log($"{CurrentRound}라운드 종료 / {PlayerFighter.Name} HP {PlayerCurrentHp:F1}, 유효타 {playerRoundSignificantStrikes}, 테이크다운 {playerRoundTakedownsSucceeded}/" +
            $"{playerRoundTakedownsAttempted} / {OpponentData.Name} HP {OpponentCurrentHp:F1}, 유효타 {opponentRoundSignificantStrikes}, 테이크다운 {opponentRoundTakedownsSucceeded}/{opponentRoundTakedownsAttempted}");
        
        bool matchCompleted = TryCompleteMatchByKnockOut();
        if (matchCompleted)
        {
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

        MatchFinishType finishType = MatchFinishType.Decision;

        if (matchResult == MatchResult.Draw)
        {
            finishType = MatchFinishType.Draw;
        }

        CompleteMatch(matchResult, finishType);

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
        LastResultSummary = null;

        if (CombatModel != null)
        {
            CombatModel.Reset();
        }

        CurrentState = MatchState.Finished;
        Debug.LogError($"경기 처리 중단. {errorMessage}");
    }

    private bool TryCreateMatchResultSummary(MatchResult result, MatchFinishType finishType)
    {
        if (PlayerFighter == null)
        {
            Debug.LogError("경기 결과 요약 생성 실패. 플레이어 선수 없음");
            return false;
        }

        if (OpponentData == null)
        {
            Debug.LogError("경기 결과 요약 생성 실패. 상대 선수 없음");
            return false;
        }

        if (CurrentRuleData == null)
        {
            Debug.LogError("경기 결과 요약 생성 실패. 경기 규칙 없음");
            return false;
        }

        float passedRoundSeconds = CurrentRuleData.RoundSeconds - RoundRemainingSeconds;

        if (passedRoundSeconds < 0f)
        {
            passedRoundSeconds = 0f;
        }

        if (passedRoundSeconds > CurrentRuleData.RoundSeconds)
        {
            passedRoundSeconds = CurrentRuleData.RoundSeconds;
        }

        MatchResultSummary resultSummary = new MatchResultSummary(PlayerFighter.Name, OpponentData.Name);
        resultSummary.SetMatchResult(result, finishType, CurrentRound, passedRoundSeconds);

        if (_combatRunner != null)
        {
            MatchCombatStats playerCombatStats = _combatRunner.GetCombatStats(MatchFighterSide.Player);
            MatchCombatStats opponentCombatStats = _combatRunner.GetCombatStats(MatchFighterSide.Opponent);
            resultSummary.SetCombatStats(playerCombatStats, opponentCombatStats);
        }

        LastResultSummary = resultSummary;

        return true;
    }

    private void CompleteMatch(MatchResult result, MatchFinishType finishType)
    {
        if (result == MatchResult.None)
        {
            StopMatchByError("유효하지 않은 경기 결과가 전달됨");
            return;
        }

        if (finishType == MatchFinishType.None)
        {
            StopMatchByError("유효하지 않은 경기 종료 방식");
            return;
        }

        bool summaryCreateSuccess = TryCreateMatchResultSummary(result, finishType);

        if (summaryCreateSuccess == false)
        {
            StopMatchByError("경기 결과 요약 생성 실패");
            return;
        }

        if (CombatModel != null)
        {
            CombatModel.Reset();
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
        CombatModel = null;
        _combatRunner = null;

        _playerMatchFighter = null;
        _opponentMatchFighter = null;

        CurrentRound = 0;
        RoundRemainingSeconds = 0f;
        RoundBreakRemainingSeconds = 0f;

        _playerRoundStartHp = 0f;
        _opponentRoundStartHp = 0f;
        _playerRoundStartSignificantStrikes = 0;
        _opponentRoundStartSignificantStrikes = 0;
        _playerRoundStartTakedownsSucceeded = 0;
        _playerRoundStartTakedownsAttempted = 0;
        _opponentRoundStartTakedownsSucceeded = 0;
        _opponentRoundStartTakedownsAttempted = 0;
        LastResult = MatchResult.None;
        _roundRecords.Clear();

        CurrentState = MatchState.None;
    }
}
