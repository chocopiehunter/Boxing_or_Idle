using UnityEngine;

public class MenuUI : UIBase
{
    [SerializeField] private UIButton Button_Management;
    [SerializeField] private UIButton Button_MatchSchedule;
    [SerializeField] private UIButton Button_Settings;
    [SerializeField] private UIButton Button_Close;
    [SerializeField] private UIButton Button_ToTitle;

    // 나중에 구현할 것
    // [SerializeField] private UIButton Button_Inventory;
    // [SerializeField] private UIButton Button_History;

    private void OnEnable()
    {
        Button_Management.BindOnClickButtonEvent(OnClick_Management);
        Button_MatchSchedule.BindOnClickButtonEvent(OnClick_MatchSchedule);
        Button_Settings.BindOnClickButtonEvent(OnClick_Settings);
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
    }

    private void OnClick_Management()
    {
        Debug.Log("선수관리 버튼 누름. 미구현");
    }

    private void OnClick_MatchSchedule()
    {
        Debug.Log("경기관리 버튼 누름. 미구현");
    }

    private void OnClick_Settings()
    {
        UIManager.Instance.OpenPopupUI(UIType.SettingUI);
    }

    private void OnClick_Close()
    {
        UIManager.Instance.ClosePopupUI(UIType.MenuUI);

        GameManager.Instance.GameState.ChangeState(GameFlowState.Play);
    }
}
