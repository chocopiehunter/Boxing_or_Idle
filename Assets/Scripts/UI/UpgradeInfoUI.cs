using UnityEngine;
using UnityEngine.UI;

public class UpgradeInfoUI : UIBase
{
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

    private void OnEnable()
    {
        Button_Confirm.BindOnClickButtonEvent(OnClick_Confirm);
        Button_Cancel.BindOnClickButtonEvent(OnClick_Cancel);
    }

    public void Open(string type)
    {
        _targetType = type;
        RefreshUI();
    }

    private void RefreshUI()
    {
        GymLevelData currentData = GymManager.Instance.GetCurrentLevelData(_targetType);
        GymLevelData nextData = GymManager.Instance.GetNextLevelData(_targetType);

        if (nextData  == null)
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
            Button_Confirm.SetInteractable(false);
            return;
        }

        if (currentData == null)
        {
            Text_UpgradeName_Before.text = "없음";
            Text_UpgradeInfo_Before.text = EmptyLabel;
            Text_Level_Before.text = EmptyLabel;
        }
        else
        {
            Text_UpgradeName_Before.text = currentData.Name;
            Text_UpgradeInfo_Before.text = currentData.Description;
            Text_Level_Before.text = currentData.Level.ToString();
        }

        Text_UpgradeName_After.text = nextData.Name;
        Text_UpgradeInfo_After.text = nextData.Description;
        Text_Level_After.text = nextData.Level.ToString();

        int ownedGold = GymManager.Instance.CurrentGym.Gold;
        bool enoughGold = true;
        if (ownedGold < nextData.GoldCost)
        {
            enoughGold = false;
        }

        Text_Required_Gold.text = $"필요 {nextData.GoldCost} / 보유 {ownedGold}";
        if (enoughGold == true)
        {
            Text_Required_Gold.color = Color_Enough;
        }
        else
        {
            Text_Required_Gold.color = Color_NotEnough;
        }

        bool hasUnlockCondition = false;
        if (nextData.RequiredUnlockIds != NoneId)
        {
            hasUnlockCondition = true;
        }

        Text_Required_UnlockIds.text = GetUnlockText(nextData.RequiredUnlockIds);
        if (hasUnlockCondition == true)
        {
            Text_Required_UnlockIds.color = Color_NotEnough;
        }
        else
        {
            Text_Required_UnlockIds.color = Color_Enough;
        }

        bool canConfirm = false;
        if (enoughGold == true)
        {
            if (hasUnlockCondition == false)
            {
                canConfirm = true;
            }
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
                result = result + "|n" + line;
            }
        }

        return result;
    }

    private void OnClick_Confirm()
    {
        bool success = GymManager.Instance.TryUpgrade(_targetType);
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
