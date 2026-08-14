using UnityEngine;

public class MenuUI : UIBase
{
    [SerializeField] private UIButton Button_TrainingManagement;
    [SerializeField] private UIButton Button_MatchSchedule;
    [SerializeField] private UIButton Button_Settings;
    [SerializeField] private UIButton Button_Close;
    [SerializeField] private UIButton Button_ToTitle;

    // 나중에 구현할 것
    // [SerializeField] private UIButton Button_Inventory;
    // [SerializeField] private UIButton Button_History;

    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
        Button_ToTitle.BindOnClickButtonEvent(OnClick_ToTitle);
        Button_TrainingManagement.BindOnClickButtonEvent(OnClick_TrainingManagement);
        Button_MatchSchedule.BindOnClickButtonEvent(OnClick_MatchSchedule);
        Button_Settings.BindOnClickButtonEvent(OnClick_Settings);
    }

    private void OnClick_Close()
    {
        UIManager.Instance.ClosePopupUI(UIType.MenuUI);

        GameManager.Instance.GameState.ChangeState(GameFlowState.Play);
    }

    private void OnClick_ToTitle()
    {
        UIManager.Instance.ClosePopupUI(UIType.MenuUI);
        UIManager.Instance.CloseUI(UIRootType.MainUI, UIType.MainUI);

        FighterManager.Instance.ClearRoster();

        GameManager.Instance.GameState.ChangeState(GameFlowState.Title);

        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.TitleUI);
    }

    private void OnClick_TrainingManagement()
    {
        UIManager.Instance.OpenPopupUI(UIType.TrainingManagementUI);
    }

    private void OnClick_MatchSchedule()
    {
        UIManager.Instance.OpenPopupUI(UIType.MatchScheduleUI);
    }

    private void OnClick_Settings()
    {
        UIManager.Instance.OpenPopupUI(UIType.SettingUI);
    }
}
