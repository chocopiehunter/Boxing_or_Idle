using UnityEngine;

public class MainUI : UIBase
{
    [SerializeField] private SeasonUI UI_Season;
    [SerializeField] private TrainingUI UI_Training;
    [SerializeField] private UIButton Button_Menu;

    private void OnEnable()
    {
        if(SeasonManager.Instance == null)
        {
            Debug.LogError("SeasonManager가 씬에 없습니다");
            return;
        }

        SeasonManager.Instance.OnMonthAdvanced += RefreshSeasonUI;
        SeasonManager.Instance.OnMonthProgressChanged += RefreshMonthProgress;
        Button_Menu.BindOnClickButtonEvent(OnClick_Menu);

        RefreshSeasonUI();
        RefreshMonthProgress(SeasonManager.Instance.MonthProgress);
    }

    private void OnDisable()
    {
        if(SeasonManager.Instance != null)
        {
            SeasonManager.Instance.OnMonthAdvanced -= RefreshSeasonUI;
            SeasonManager.Instance.OnMonthProgressChanged -= RefreshMonthProgress;
        }
    }

    private void RefreshSeasonUI()
    {
        UI_Season.SetSeason(SeasonManager.Instance.CurrentSeason,
                            SeasonManager.Instance.Year,
                            SeasonManager.Instance.CurrentMonth, 1); // Week는 아직 없어서 1로 고정
    }

    private void RefreshMonthProgress(float ratio)
    {
        UI_Season.SetMonthProgress(ratio);
    }

    private void OnClick_Menu()
    {
        GameManager.Instance.GameState.ChangeState(GameFlowState.Pause);

        UIManager.Instance.OpenPopupUI(UIType.MenuUI);
    }
}
