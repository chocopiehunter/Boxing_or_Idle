using UnityEngine;
using UnityEngine.UI;

public class GymManagementUI : UIBase
{
    private const string DefaultBuildingType = "gym";

    [SerializeField] private UIButton Button_Close;
    [SerializeField] private UIButton Button_Upgrade_Gym;
    [SerializeField] private Text Text_UpgradeName;

    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
        Button_Upgrade_Gym.BindOnClickButtonEvent(OnClick_UpgradeGym);

        RefreshUI();
    }

    public void RefreshUI()
    {
        GymLevelData currentData = GymManager.Instance.GetCurrentLevelData(DefaultBuildingType);
        GymLevelData nextData = GymManager.Instance.GetNextLevelData(DefaultBuildingType);

        if (currentData == null)
        {
            Text_UpgradeName.text = "기본 체육관 건축";
            Button_Upgrade_Gym.ChangeButtonText("건축");
        }
        else
        {
            Text_UpgradeName.text = currentData.Name;
            Button_Upgrade_Gym.ChangeButtonText("업그레이드");
        }

        if (nextData == null)
        {
            Button_Upgrade_Gym.ChangeButtonText("Max");
            Button_Upgrade_Gym.SetInteractable(false);
            return;
        }

        Button_Upgrade_Gym.SetInteractable(true);
    }

    private void OnClick_UpgradeGym()
    {
        UpgradeInfoUI upgradeInfoUI = UIManager.Instance.OpenPopupUI(UIType.UpgradeInfoUI) as UpgradeInfoUI;

        if (upgradeInfoUI == null)
        {
            Debug.LogError("UpgradeInfoUI 열기 실패");
            return;
        }

        upgradeInfoUI.OpenGym(DefaultBuildingType);
    }

    private void OnClick_Close()
    {
        UIManager.Instance.ClosePopupUI(UIType.GymManagementUI);
    }
}
