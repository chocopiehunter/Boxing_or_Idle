using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-100)] // 실행순서 강제로 테스트
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    [SerializeField] private AudioMixer Mixer;

    private const string Mixer_Param_Master_Volume = "MasterVolume";

    private const string Pref_Key_Master_Volume = "MasterVolume";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("SoundManager가 중복되어 파괴합니다");
            Destroy(gameObject);
            return;
        }

        InitVolume();
    }

    private void InitVolume()
    {
        float savedVolume = PlayerPrefs.HasKey(Pref_Key_Master_Volume)
            ? PlayerPrefs.GetFloat(Pref_Key_Master_Volume) : 1f;

        SetMasterVolume(savedVolume);
    }

    public void SetMasterVolume(float volume)
    {
        // 슬라이더값 0~1을 그대로 넣으면 안되고 데시벨값으로 변환해야함
        float dbVolume = (volume <= 0.0001f) ? -80f : Mathf.Log10(volume) * 20f;
        Mixer.SetFloat(Mixer_Param_Master_Volume, dbVolume);

        PlayerPrefs.SetFloat(Pref_Key_Master_Volume, volume);
    }

    public float GetMasterVolume()
    {
        return PlayerPrefs.HasKey(Pref_Key_Master_Volume)
            ? PlayerPrefs.GetFloat(Pref_Key_Master_Volume) : 1f;
    }
}
