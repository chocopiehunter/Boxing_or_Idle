using System;
using UnityEngine;
using UnityEngine.UI;

public class MatchResultUI : UIBase
{
    [SerializeField] private Text Text_Winner;
    [SerializeField] private Text Text_FinishResult;

    [SerializeField] private Text Text_PlayerName;
    [SerializeField] private Text Text_OpponentName;

    [SerializeField] private Text Text_PlayerSignificantStrikes;
    [SerializeField] private Text Text_OpponentSignificantStrikes;

    [SerializeField] private Text Text_PlayerControlTime;
    [SerializeField] private Text Text_OpponentControlTime;

    [SerializeField] private UIButton Button_ReturnToGym;

    public void Show(MatchResultSummary resultSummary)
    {
        if (resultSummary == null)
        {
            Debug.LogError($"경기 결과 UI 출력 실패. MatchResultSummary 없음");
            return;
        }

        gameObject.SetActive(true);

        Text_Winner.text = GetWinnerText(resultSummary);
        Text_FinishResult.text = GetFinishResultText(resultSummary);

        Text_PlayerName.text = resultSummary.PlayerName;
        Text_OpponentName.text = resultSummary.OpponentName;

        Text_PlayerSignificantStrikes.text = resultSummary.PlayerStats.SignificantStrikesSucceeded.ToString();
        Text_OpponentSignificantStrikes.text = resultSummary.OpponentStats.SignificantStrikesSucceeded.ToString();

        Text_PlayerControlTime.text = FormatTime(resultSummary.PlayerStats.ControlSeconds);
        Text_OpponentControlTime.text = FormatTime(resultSummary.OpponentStats.ControlSeconds);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void BindReturnToGymButtonEvent(Action onClickCallback)
    {
        if (Button_ReturnToGym == null)
        {
            Debug.LogError("체육관 복귀 버튼 연결 실패. Button_ReturnToGym 없음");
            return;
        }

        Button_ReturnToGym.UnBindAllOnClickButtonEvent();

        if (onClickCallback == null)
        {
            Debug.LogError("체육관 복귀 버튼 연결 실패. 클릭 이벤트 없음");
            return;
        }

        Button_ReturnToGym.BindOnClickButtonEvent(onClickCallback);
    }

    private string GetWinnerText(MatchResultSummary resultSummary)
    {
        if (resultSummary.Result == MatchResult.Win)
        {
            return $"{resultSummary.PlayerName} 승리";
        }

        if (resultSummary.Result == MatchResult.Lose)
        {
            return $"{resultSummary.OpponentName} 승리";
        }

        if (resultSummary.Result == MatchResult.Draw)
        {
            return "무승부";
        }

        return "경기 결과 없음";
    }

    private string GetFinishResultText(MatchResultSummary resultSummary)
    {
        if (resultSummary.FinishType == MatchFinishType.Decision)
        {
            return "판정";
        }

        if (resultSummary.FinishType == MatchFinishType.UnanimousDecision)
        {
            return "만장일치 판정";
        }

        if (resultSummary.FinishType == MatchFinishType.SplitDecision)
        {
            return "2:1 판정";
        }

        if (resultSummary.FinishType == MatchFinishType.Draw)
        {
            return "무승부";
        }

        string finishTime = FormatTime(resultSummary.PassedRoundSeconds);
        return $"{resultSummary.FinishedRound}R {finishTime} {resultSummary.FinishType}";
    }

    private string FormatTime(float seconds)
    {
        if (seconds < 0f)
        {
            seconds = 0f;
        }

        int totalSeconds = (int)seconds;
        int minutes = totalSeconds / 60;
        int remainingSeconds = totalSeconds % 60;

        return $"{minutes:00}:{remainingSeconds:00}";
    }
}
