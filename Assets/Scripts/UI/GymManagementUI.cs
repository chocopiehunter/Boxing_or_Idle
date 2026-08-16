using UnityEngine;
using UnityEngine.UI;

public class GymManagementUI : UIBase
{
    [SerializeField] private UIButton Button_Close;
    [SerializeField] private UIButton Button_Dim;
    [SerializeField] private UIButton Button_Upgrade_Gym;
    [SerializeField] private Text Text_UpgradeName;

    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
    }

    private void RefreshUI()
    {

    }

    private void OnClick_UpgradeGym()
    {

    }

    private void OnClick_Close()
    {
        UIManager.Instance.ClosePopupUI(UIType.GymManagementUI);
    }
}
