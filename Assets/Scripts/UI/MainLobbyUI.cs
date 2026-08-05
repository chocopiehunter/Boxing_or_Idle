using UnityEngine;

public class MainLobbyUI : UIBase
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
        Debug.Log($"{Button_GameStart} 버튼이 눌렸습니다");
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
