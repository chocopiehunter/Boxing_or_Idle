using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Slider Slider_MasterVolume;

    private void OnEnable()
    {
        float currentVolume = SoundManager.Instance.GetMasterVolume();
        Slider_MasterVolume.SetValueWithoutNotify(currentVolume);

        Slider_MasterVolume.onValueChanged.AddListener(OnMasterVolumeChanged);
    }

    private void OnDisable()
    {
        Slider_MasterVolume.onValueChanged.RemoveListener(OnMasterVolumeChanged);
    }

    private void OnMasterVolumeChanged(float value)
    {
        SoundManager.Instance.SetMasterVolume(value);
    }
}
