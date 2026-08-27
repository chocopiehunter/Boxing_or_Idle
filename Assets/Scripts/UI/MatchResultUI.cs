using UnityEngine;
using UnityEngine.UI;

public class MatchResultUI : UIBase
{
    [SerializeField] private Text Text_Result;

    [SerializeField] private Text Text_PlayerName;
    [SerializeField] private Text Text_OpponentName;

    [SerializeField] private Text Text_PlayerSignificantStrikes;
    [SerializeField] private Text Text_OpponentSignificantStrikes;

    [SerializeField] private Text Text_PlayerControlTime;
    [SerializeField] private Text Text_OpponentControlTime;

    public void Show(MatchResultSummary resultSummary)
    {
        if (resultSummary == null)
        {
            Debug.LogError($"경기 결과 UI 출력 실패. MatchResultSummary 없음");
            return;
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
