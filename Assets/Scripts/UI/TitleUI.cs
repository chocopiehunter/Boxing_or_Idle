using UnityEngine;

public class TitleUI : UIBase
{
    [SerializeField] private UIButton Button_GameStart;
    [SerializeField] private UIButton Button_GameQuit;
    [SerializeField] private UIButton Button_LoadGame;
    [SerializeField] private UIButton Button_Settings;

    private void OnEnable()
    {
        Button_GameStart.BindOnClickButtonEvent(OnClick_GameStart);
        Button_GameQuit.BindOnClickButtonEvent(OnClick_GameQuit);
        Button_Settings.BindOnClickButtonEvent(OnClick_Settings);
        //Button_LoadGame.BindOnClickButtonEvent(OnClick_LoadGame);
    }

    public void OnClick_GameStart()
    {
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.TitleUI);
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.MainUI);
        FighterManager.Instance.CreateStartingRoster();

        GameManager.Instance.GameState.ChangeState(GameFlowState.Play);
    }

    public void OnClick_GameQuit()
    {
        Application.Quit();
    }

    public void OnClick_Settings()
    {
        UIManager.Instance.OpenPopupUI(UIType.SettingUI);
    }

    // 로드는 나중에 구현
    public void OnClick_LoadGame()
    {

    }
}
