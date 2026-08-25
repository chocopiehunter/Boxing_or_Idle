using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GymManagementUI : UIBase
{
    private const string DefaultBuildingType = "gym";

    [SerializeField] private UIButton Button_Close;
    [SerializeField] private UIButton Button_Upgrade_Gym;
    [SerializeField] private Text Text_UpgradeName;

    [SerializeField] private Transform Transform_FacilityContent;
    [SerializeField] private GymFacilityRowUI Prefab_FacilityRow;

    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
        Button_Upgrade_Gym.BindOnClickButtonEvent(OnClick_UpgradeGym);

        RefreshUI();
    }

    public void RefreshUI()
    {
        RefreshGymUI();
        RefreshFacilityListUI();
    }

    private void RefreshGymUI()
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

    private void RefreshFacilityListUI()
    {
        ClearFacilityRowUI();

        List<string> facilityTypes = GymManager.Instance.GetVisibleFacilityTypes();

        for (int i = 0; i < facilityTypes.Count; i++)
        {
            string facilityType = facilityTypes[i];
            TrainingFacilityData currentData = GymManager.Instance.GetCurrentFacilityData(facilityType);
            TrainingFacilityData nextData = GymManager.Instance.GetNextFacilityData(facilityType);

            GymFacilityRowUI rowUI = Instantiate(Prefab_FacilityRow, Transform_FacilityContent);
            rowUI.Setup(facilityType, currentData, nextData, OnClick_Facility);
        }
    }

    private void ClearFacilityRowUI()
    {
        for (int i = Transform_FacilityContent.childCount -1; i >= 0; i--)
        {
            Destroy(Transform_FacilityContent.GetChild(i).gameObject);
        }
    }

    private void OnClick_Facility(string facilityType)
    {
        UpgradeInfoUI upgradeInfoUI = UIManager.Instance.OpenPopupUI(UIType.UpgradeInfoUI) as UpgradeInfoUI;

        if (upgradeInfoUI == null)
        {
            Debug.LogError("UpgradeInfoUI 열기 실패");
            return;
        }

        upgradeInfoUI.OpenFacility(facilityType);
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
