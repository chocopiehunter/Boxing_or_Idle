using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UIBase
{
    [SerializeField] private Slider Slider_MasterVolume;
    [SerializeField] private UIButton Button_Close;
    [SerializeField] private UIButton Button_ToTitleMenu;

    private void OnEnable()
    {
        float currentVolume = SoundManager.Instance.GetMasterVolume();
        Slider_MasterVolume.SetValueWithoutNotify(currentVolume);

        Slider_MasterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
        Button_Close.BindOnClickButtonEvent(OnClick_Close);

        Button_ToTitleMenu.BindOnClickButtonEvent(OnClick_ToTitleMenu);
    }

    private void OnDisable()
    {
        Slider_MasterVolume.onValueChanged.RemoveListener(OnMasterVolumeChanged);
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

    private void OnClick_ToTitleMenu()
    {
        Debug.Log("SettingUI에서 '타이틀로 돌아가기' 버튼 누름");

        UIManager.Instance.ClosePopupUI(UIType.SettingUI);
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.MainLobbyUI);
    }
}
