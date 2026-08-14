using UnityEngine;
using UnityEngine.UI;

public class MatchUI : UIBase
{
    [SerializeField] private Text Text_FighterName_Left;
    [SerializeField] private Text Text_FighterName_Right;
    [SerializeField] private Text Text_Team_Left;
    [SerializeField] private Text Text_Team_Right;
    [SerializeField] private Text Text_WLD_Left;
    [SerializeField] private Text Text_WLD_Right;
    [SerializeField] private Text Text_Round;
    [SerializeField] private Text Text_Time;

    [SerializeField] private RawImage RawImage_Fighter_Left;
    [SerializeField] private RawImage RawImage_Fighter_Right;
    [SerializeField] private UIButton Button_Close;

    [SerializeField] private Text Text_MatchWinner;

    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
        RefreshUI();
    }

    private void RefreshUI()
    {
        FighterModel player = MatchManager.Instance.PlayerFighter;
        FighterData opponent = MatchManager.Instance.OpponentData;
        MatchResult result = MatchManager.Instance.LastResult;

        if(player != null)
        {
            Text_FighterName_Left.text = player.Name;
        }

        if(opponent != null)
        {
            Text_FighterName_Right.text = opponent.Name;
        }

        Text_Round.text = "경기 종료";
        Text_Time.text = "-";

        ApplyMatchWinnerText(result, player, opponent);
    }

    private void ApplyMatchWinnerText(MatchResult result, FighterModel player, FighterData opponent)
    {
        if(Text_MatchWinner == null)
        {
            return;
        }

        if (result == MatchResult.Draw)
        {
            Text_MatchWinner.text = "무승부";
            return;
        }

        if(result == MatchResult.Win)
        {
            if (player != null)
            {
                Text_MatchWinner.text = $"{player.Name} 승리";
            }
            return;
        }

        if (opponent  != null)
        {
            Text_MatchWinner.text = $"{opponent.Name} 승리";
        }

    }

    private void OnClick_Close()
    {
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.MatchUI);
        GameManager.Instance.GameState.ChangeState(GameFlowState.Play);
    }
}
