using UnityEngine;

public class MainUI : UIBase
{
    [SerializeField] private SeasonUI UI_Season;
    [SerializeField] private TrainingUI UI_Training;

    private void OnEnable()
    {
        if(SeasonManager.Instance == null)
        {
            Debug.LogError("SeasonManager가 씬에 없습니다");
            return;
        }

        SeasonManager.Instance.OnMonthAdvanced += RefreshSeasonUI;
        RefreshSeasonUI();
    }

    private void OnDisable()
    {
        if(SeasonManager.Instance != null)
        {
            SeasonManager.Instance.OnMonthAdvanced -= RefreshSeasonUI;
        }
    }

    private void RefreshSeasonUI()
    {
        UI_Season.SetSeason(SeasonManager.Instance.CurrentSeason,
                            SeasonManager.Instance.Year,
                            SeasonManager.Instance.CurrentMonth, 1); // Week는 아직 없어서 1로 고정
    }
}
