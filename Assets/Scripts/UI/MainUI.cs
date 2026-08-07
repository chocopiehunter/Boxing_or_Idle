using UnityEngine;

public class MainUI : UIBase
{
    [SerializeField] private SeasonUI UI_Season;

    private void OnEnable()
    {
        if(SeasonManager.Instance == null)
        {
            Debug.LogError("SeasonManager가 씬에 없습니다");
            return;
        }

        SeasonManager.Instance.OnWeekAdvanced += RefreshSeasonUI;
        RefreshSeasonUI();
    }

    private void OnDisable()
    {
        if(SeasonManager.Instance != null)
        {
            SeasonManager.Instance.OnWeekAdvanced -= RefreshSeasonUI;
        }
    }

    private void RefreshSeasonUI()
    {
        UI_Season.SetSeason(SeasonManager.Instance.CurrentSeason, SeasonManager.Instance.CurrentWeek);
    }
}
