using Cysharp.Threading.Tasks;
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

    [SerializeField] private Image Image_Fighter_Left;
    [SerializeField] private Image Image_Fighter_Right;
    [SerializeField] private Sprite DefaultFighterSprite_Left;
    [SerializeField] private Sprite DefaultFighterSprite_Right;
    [SerializeField] private UIButton Button_Close;

    [SerializeField] private Text Text_MatchWinner;

    private const string NoneId = "None";

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
            TrySetFighterBody(Image_Fighter_Left, player.BodyAddress, DefaultFighterSprite_Left);
        }
        else
        {
            SetDefaultBody(Image_Fighter_Left, DefaultFighterSprite_Left);
        }

        if (opponent != null)
        {
            Text_FighterName_Right.text = opponent.Name;
            TrySetFighterBody(Image_Fighter_Right, opponent.BodyAddress, DefaultFighterSprite_Right);
        }
        else
        {
            SetDefaultBody(Image_Fighter_Right, DefaultFighterSprite_Right);
        }

        Text_Round.text = "경기 종료";
        Text_Time.text = "-";

        ApplyMatchWinnerText(result, player, opponent);
    }

    private void TrySetFighterBody(Image fighterImage, string bodyAddress, Sprite defaultSprite)
    {
        SetDefaultBody(fighterImage, defaultSprite);

        if (HasBodyAddress(bodyAddress) == false)
        {
            return;
        }

        SetFighterBodyAsync(fighterImage, bodyAddress).Forget();
    }

    private void SetDefaultBody(Image fighterImage, Sprite defaultSprite)
    {
        if (fighterImage == null)
        {
            return;
        }

        fighterImage.sprite = defaultSprite;
    }

    private async UniTaskVoid SetFighterBodyAsync(Image fighterImage, string bodyAddress)
    {
        if (fighterImage == null)
        {
            return;
        }

        await GameUtil.LoadAndSetSpriteImage(fighterImage, bodyAddress);
    }

    private bool HasBodyAddress(string address)
    {
        if (string.IsNullOrEmpty(address) == true)
        {
            return false;
        }

        if (address == NoneId)
        {
            return false;
        }

        return true;
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
