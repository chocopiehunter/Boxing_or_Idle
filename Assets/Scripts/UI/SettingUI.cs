using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Slider Slider_MasterVolume;
    [SerializeField] private UIButton Button_Close;

    private void OnEnable()
    {
        float currentVolume = SoundManager.Instance.GetMasterVolume();
        Slider_MasterVolume.SetValueWithoutNotify(currentVolume);

        Slider_MasterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
    }

    private void OnDisable()
    {
        Slider_MasterVolume.onValueChanged.RemoveListener(OnMasterVolumeChanged);
    }

    private void OnMasterVolumeChanged(float value)
    {
        Debug.Log("SettingUI 슬라이더 값 변경됨");
        SoundManager.Instance.SetMasterVolume(value);
    }

    // 나중에 UIManager나 Extension에서 OpenPopupUI/ClosePopupUI로 바꾸기
    private void OnClick_Close()
    {
        Debug.Log("SettingUI 닫기 버튼 누름");
        this.gameObject.SetActive(false);
    }
}
