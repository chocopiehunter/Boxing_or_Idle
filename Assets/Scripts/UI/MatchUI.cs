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

    [SerializeField] private MatchStrategySelectionUI StrategySelectionUI;
    [SerializeField] private MatchResultUI ResultUI;

    private const string NoneId = "None";
    private bool _hasDisplayedMatchProgress;
    private MatchState _displayedState;
    private int _displayedRound = -1;
    private int _displayedSeconds = 1;
    private bool _isReturningToGym;

    private void OnEnable()
    {
        Button_Close.SetInteractable(false);

        _hasDisplayedMatchProgress = false;
        _isReturningToGym = false;

        if (ResultUI != null)
        {
            ResultUI.BindReturnToGymButtonEvent(OnClick_ReturnToGym);
            ResultUI.Hide();
        }

        RefreshUI();
    }

    private void Update()
    {
        RefreshMatchProgressUI();
    }

    private void RefreshUI()
    {
        FighterModel player = MatchManager.Instance.PlayerFighter;
        FighterData opponent = MatchManager.Instance.OpponentData;

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

        RefreshMatchProgressUI();
    }

    private void RefreshMatchProgressUI()
    {
        if (MatchManager.Instance == null)
        {
            return;
        }

        MatchState currentState = MatchManager.Instance.CurrentState;

        RefreshMatchStrategySelectionUI(currentState);

        int displaySeconds = GetDisplaySeconds();

        if (_hasDisplayedMatchProgress == true && _displayedState == currentState && _displayedRound == MatchManager.Instance.CurrentRound && _displayedSeconds == displaySeconds)
        {
            return;
        }

        _hasDisplayedMatchProgress = true;
        _displayedState = currentState;
        _displayedRound = MatchManager.Instance.CurrentRound;
        _displayedSeconds = displaySeconds;

        if (currentState == MatchState.RoundInProgress)
        {
            int totalRoundCount = 0;
            if (MatchManager.Instance.CurrentRuleData != null)
            {
                totalRoundCount = MatchManager.Instance.CurrentRuleData.RoundCount;
            }

            Text_Round.text = $"{MatchManager.Instance.CurrentRound} / {totalRoundCount} 라운드";
            Text_Time.text = FormatTime(displaySeconds);
            ClearMatchWinnerText();
            return;
        }

        if (currentState == MatchState.RoundBreak)
        {
            Text_Round.text = $"{MatchManager.Instance.CurrentRound} 라운드 종료";
            Text_Time.text = FormatTime(displaySeconds);
            ClearMatchWinnerText();
            return;
        }

        if (currentState == MatchState.Finished)
        {
            Text_Round.text = "경기 종료";
            Text_Time.text = "-";
            ApplyMatchWinnerText(MatchManager.Instance.LastResult, MatchManager.Instance.PlayerFighter, MatchManager.Instance.OpponentData);
            ShowMatchResultUI();
            return;
        }

        Text_Round.text = "경기 준비";
        Text_Time.text = "-";
        ClearMatchWinnerText();
    }

    private void ShowMatchResultUI()
    {
        if (ResultUI == null)
        {
            Debug.LogError("경기 결과 UI 출력 실패. MatchResultUI 없음");
            return;
        }

        MatchResultSummary resultSummary = MatchManager.Instance.LastResultSummary;

        if (resultSummary == null)
        {
            Debug.LogError("경기 결과 UI 출력 실패. MatchResultSummary 없음");
            ResultUI.Hide();
            return;
        }

        ResultUI.Show(resultSummary);
    }

    private void RefreshMatchStrategySelectionUI(MatchState currentState)
    {
        if (StrategySelectionUI == null)
        {
            return;
        }

        bool shouldShow = false;

        if (currentState == MatchState.RoundBreak)
        {
            shouldShow = true;
        }

        if (StrategySelectionUI.gameObject.activeSelf == shouldShow)
        {
            return;
        }

        StrategySelectionUI.gameObject.SetActive(shouldShow);
    }

    private int GetDisplaySeconds()
    {
        if (MatchManager.Instance == null)
        {
            return 0;
        }

        if (MatchManager.Instance.CurrentState == MatchState.RoundInProgress)
        {
            return Mathf.CeilToInt(Mathf.Max(0f, MatchManager.Instance.RoundRemainingSeconds));
        }

        if (MatchManager.Instance.CurrentState == MatchState.RoundBreak)
        {
            return Mathf.CeilToInt(Mathf.Max(0f, MatchManager.Instance.RoundBreakRemainingSeconds));
        }

        return 0;
    }

    private string FormatTime(int totalSeconds)
    {
        if (totalSeconds < 0)
        {
            totalSeconds = 0;
        }

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        return $"{minutes:00}:{seconds:00}";
    }

    private void ClearMatchWinnerText()
    {
        if (Text_MatchWinner == null)
        {
            return;
        }

        Text_MatchWinner.text = "";
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

        if (result == MatchResult.None)
        {
            Text_MatchWinner.text = "경기 결과 없음";
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

    private void OnClick_ReturnToGym()
    {
        if (_isReturningToGym == true)
        {
            return;
        }

        if (MatchManager.Instance == null)
        {
            Debug.LogError("체육관 복귀 실패. MatchManager 없음");
            return;
        }

        if (MatchManager.Instance.CurrentState != MatchState.Finished)
        {
            Debug.LogWarning($"체육관 복귀 실패. 경기가 종료되지 않음 CurrentState={MatchManager.Instance.CurrentState}");
            return;
        }

        _isReturningToGym = true;
        ReturnToGymAsync().Forget();
    }

    private async UniTask ReturnToGymAsync()
    {
        if (UIManager.Instance == null)
        {
            Debug.LogError("체육관 복귀 실패. UIManager 없음");
            _isReturningToGym = false;
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("체육관 복귀 실패. GameManager 없음");
            _isReturningToGym = false;
            return;
        }

        UIBase openedUI = UIManager.Instance.OpenUI(UIRootType.VeryFrontUI, UIType.TransitionLoadingUI);
        TransitionLoadingUI transitionLoadingUI = openedUI as TransitionLoadingUI;

        if (transitionLoadingUI == null)
        {
            Debug.LogError("체육관 복귀 실패. TransitionLoadingUI 없음");
            _isReturningToGym = false;
            return;
        }

        await UniTask.Yield(PlayerLoopTiming.Update);

        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.MatchUI);

        MatchManager.Instance.ClearMatch();
        GameManager.Instance.GameState.ChangeState(GameFlowState.Play);

        await transitionLoadingUI.WaitForSeconds();

        UIManager.Instance.CloseUI(UIRootType.VeryFrontUI, UIType.TransitionLoadingUI);
    }
}
