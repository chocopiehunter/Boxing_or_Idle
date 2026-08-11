using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UIBase
{
    [SerializeField] private Slider Slider_MasterVolume;
    [SerializeField] private UIButton Button_Close;
    [SerializeField] private UIButton Button_ToTitleMenu;

    [SerializeField] private UIButton Button_SpeedNormal;
    [SerializeField] private UIButton Button_SpeedFast;

    private void OnEnable()
    {
        float currentVolume = SoundManager.Instance.GetMasterVolume();
        Slider_MasterVolume.SetValueWithoutNotify(currentVolume);

        Slider_MasterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
        Button_ToTitleMenu.BindOnClickButtonEvent(OnClick_ToTitle);

        Button_SpeedNormal.BindOnClickButtonEvent(OnClick_SpeedNormal);
        Button_SpeedFast.BindOnClickButtonEvent(OnClick_SpeedFast);

        RefreshSpeedUI();
    }

    private void OnDisable()
    {
        Slider_MasterVolume.onValueChanged.RemoveListener(OnMasterVolumeChanged);
    }

    private void RefreshSpeedUI()
    {
        bool unlocked = GameManager.Instance.GameState.HasClearedFirstGame;


    }

    private void OnClick_SpeedNormal()
    {
        GameManager.Instance.GameState.ChangeSpeed(GameSpeedType.Normal);
        Debug.Log("배속 Normal 변경");
    }

    private void OnClick_SpeedFast()
    {
        GameManager.Instance.GameState.ChangeSpeed(GameSpeedType.Fast);
        Debug.Log("배속 Fast 변경");
    }

    private void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
    }

    // 나중에 UIManager나 Extension에서 OpenPopupUI/ClosePopupUI로 바꾸기
    private void OnClick_Close()
    {
        UIManager.Instance.ClosePopupUI(UIType.SettingUI);
    }

    private void OnClick_ToTitle()
    {
        Debug.Log("SettingUI에서 '타이틀로 돌아가기' 버튼 누름");

        UIManager.Instance.ClosePopupUI(UIType.SettingUI);
        UIManager.Instance.ClosePopupUI(UIType.MenuUI);
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.TitleUI);

        GameManager.Instance.GameState.ChangeState(GameFlowState.Title);
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.TitleUI);
    }
}
