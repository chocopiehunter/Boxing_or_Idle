using UnityEngine;
using UnityEngine.UI;

public class UpgradeInfoUI : UIBase
{
    private enum UpgradeTargetType
    {
        None,
        Gym,
        Facility
    }

    private const string NoneId = "None";
    private const string EmptyLabel = "-";

    [SerializeField] private UIButton Button_Confirm;
    [SerializeField] private UIButton Button_Cancel;

    [SerializeField] private Text Text_UpgradeName_Before;
    [SerializeField] private Text Text_UpgradeName_After;
    [SerializeField] private Text Text_UpgradeInfo_Before;
    [SerializeField] private Text Text_UpgradeInfo_After;
    [SerializeField] private Text Text_Level_Before;
    [SerializeField] private Text Text_Level_After;

    [SerializeField] private Text Text_Required_Gold;
    [SerializeField] private Text Text_Required_UnlockIds;

    [SerializeField] private Color Color_Enough = Color.green;
    [SerializeField] private Color Color_NotEnough = Color.red;

    private string _targetType;
    private UpgradeTargetType _upgradeTargetType;

    private void OnEnable()
    {
        Button_Confirm.BindOnClickButtonEvent(OnClick_Confirm);
        Button_Cancel.BindOnClickButtonEvent(OnClick_Cancel);
    }

    public void OpenGym(string type)
    {
        _targetType = type;
        _upgradeTargetType = UpgradeTargetType.Gym;
        RefreshUI();
    }

    public void OpenFacility(string facilityType)
    {
        _targetType = facilityType;
        _upgradeTargetType = UpgradeTargetType.Facility;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (_upgradeTargetType == UpgradeTargetType.Facility)
        {
            RefreshFacilityUI();
            return;
        }

        RefreshGymUI();
    }

    private void RefreshGymUI()
    {
        GymLevelData currentData = GymManager.Instance.GetCurrentLevelData(_targetType);
        GymLevelData nextData = GymManager.Instance.GetNextLevelData(_targetType);

        if (nextData == null)
        {
            ApplyMaxLevelUI();
            return;
        }

        if (currentData == null)
        {
            ApplyUpgradeUI(false, null, null, 0, nextData.Name, nextData.Description, nextData.Level, nextData.GoldCost, nextData.RequiredUnlockIds);
            return;
        }

        ApplyUpgradeUI(true, currentData.Name, currentData.Description, currentData.Level, nextData.Name, nextData.Description, nextData.Level, nextData.GoldCost, nextData.RequiredUnlockIds);
    }

    private void RefreshFacilityUI()
    {
        TrainingFacilityData currentData = GymManager.Instance.GetCurrentFacilityData(_targetType);
        TrainingFacilityData nextData = GymManager.Instance.GetNextFacilityData(_targetType);

        if (nextData == null)
        {
            ApplyMaxLevelUI();
            return;
        }

        if (currentData == null)
        {
            ApplyUpgradeUI(false, null, null, 0, nextData.Name, nextData.Description, nextData.Level, nextData.GoldCost, nextData.RequiredUnlockIds);
            return;
        }

        ApplyUpgradeUI(true, currentData.Name, currentData.Description, currentData.Level, nextData.Name, nextData.Description, nextData.Level, nextData.GoldCost, nextData.RequiredUnlockIds);
    }

    private void ApplyMaxLevelUI()
    {
        Text_UpgradeName_Before.text = EmptyLabel;
        Text_UpgradeName_After.text = EmptyLabel;
        Text_UpgradeInfo_Before.text = EmptyLabel;
        Text_UpgradeInfo_After.text = EmptyLabel;
        Text_Level_Before.text = EmptyLabel;
        Text_Level_After.text = EmptyLabel;
        Text_Required_Gold.text = "최대 레벨";
        Text_Required_UnlockIds.text = EmptyLabel;
        Text_Required_Gold.color = Color_Enough;
        Text_Required_UnlockIds.color = Color_Enough;
        Button_Confirm.SetInteractable(false);
    }

    private void ApplyUpgradeUI(bool hasCurrentData, string currentName, string currentDescription, int currentLevel, string nextName, string nextDescription, int nextLevel, int goldCost, string requiredUnlockIds)
    {
        if (hasCurrentData == false)
        {
            Text_UpgradeName_Before.text = "없음";
            Text_UpgradeInfo_Before.text = EmptyLabel;
            Text_Level_Before.text = EmptyLabel;
        }
        else
        {
            Text_UpgradeName_Before.text = currentName;
            Text_UpgradeInfo_Before.text = currentDescription;
            Text_Level_Before.text = currentLevel.ToString();
        }

        Text_UpgradeName_After.text = nextName;
        Text_UpgradeInfo_After.text = nextDescription;
        Text_Level_After.text = nextLevel.ToString();

        int ownedGold = GymManager.Instance.CurrentGym.Gold;
        bool enoughGold = ownedGold >= goldCost;

        Text_Required_Gold.text = $"필요 {goldCost} / 보유 {ownedGold}";

        if (enoughGold == true)
        {
            Text_Required_Gold.color = Color_Enough;
        }
        else
        {
            Text_Required_Gold.color = Color_NotEnough;
        }

        bool canUnlock = UnlockConditionChecker.CanUnlockAll(requiredUnlockIds);

        Text_Required_UnlockIds.text = GetUnlockText(requiredUnlockIds);

        if (canUnlock == true)
        {
            Text_Required_UnlockIds.color = Color_Enough;
        }
        else
        {
            Text_Required_UnlockIds.color = Color_NotEnough;
        }

        bool canConfirm = false;

        if (enoughGold == true && canUnlock == true)
        {
            canConfirm = true;
        }

        Button_Confirm.SetInteractable(canConfirm);
    }

    private string GetUnlockText(string requiredUnlockIds)
    {
        if (string.IsNullOrEmpty(requiredUnlockIds) == true)
        {
            return "없음";
        }

        if (requiredUnlockIds == NoneId)
        {
            return "없음";
        }

        string result = "";
        string[] splitIds = requiredUnlockIds.Split(',');
        for (int i = 0; i < splitIds.Length; i++)
        {
            string id = splitIds[i].Trim();
            if (string.IsNullOrEmpty(id) == true)
            {
                continue;
            }

            UnlockConditionData unlockConditionData = GameDataManager.Instance.GetUnlockConditionData(id);
            string line = id;
            if (unlockConditionData != null)
            {
                line = unlockConditionData.Description;
            }

            if (string.IsNullOrEmpty(result) == true)
            {
                result = line;
            }
            else
            {
                result = result + "\n" + line;
            }
        }

        return result;
    }

    private void OnClick_Confirm()
    {
        bool success = false;
        
        if (_upgradeTargetType == UpgradeTargetType.Facility)
        {
            success = GymManager.Instance.TryUpgradeFacility(_targetType);
        }
        else
        {
            success = GymManager.Instance.TryUpgrade(_targetType);
        }

        if (success == false)
        {
            RefreshUI();
            return;
        }

        GymManagementUI gymUI = UIManager.Instance.GetOpenedUI(UIRootType.PopupUI, UIType.GymManagementUI) as GymManagementUI;

        if (gymUI != null)
        {
            gymUI.RefreshUI();
        }

        OnClick_Cancel();
    }

    private void OnClick_Cancel()
    {
        UIManager.Instance.ClosePopupUI(UIType.UpgradeInfoUI);
    }
}
