using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchStrategySelectionUI : UIBase
{
    [SerializeField] private Text Text_StrategyDescription;

    [SerializeField] private UIButton Button_Strategy1;
    [SerializeField] private UIButton Button_Strategy2;
    [SerializeField] private UIButton Button_Strategy3;
    [SerializeField] private UIButton Button_Strategy4;

    [SerializeField] private Text Text_StrategyName1;
    [SerializeField] private Text Text_StrategyName2;
    [SerializeField] private Text Text_StrategyName3;
    [SerializeField] private Text Text_StrategyName4;

    [SerializeField] private MatchStrategyTooltipUI Tooltip_Strategy1;
    [SerializeField] private MatchStrategyTooltipUI Tooltip_Strategy2;
    [SerializeField] private MatchStrategyTooltipUI Tooltip_Strategy3;
    [SerializeField] private MatchStrategyTooltipUI Tooltip_Strategy4;

    private List<MatchStrategyOptionData> _visibleStrategyOptions = new List<MatchStrategyOptionData>();

    private void Awake()
    {
        if (Text_StrategyDescription != null)
        {
            Text_StrategyDescription.raycastTarget = false;
        }

        HideDescription();
    }

    private void OnEnable()
    {
        RefreshRootStrategyOptions();
    }

    public void ShowDescription(string description)
    {
        if (Text_StrategyDescription == null)
        {
            return;
        }

        Text_StrategyDescription.text = description;
        Text_StrategyDescription.gameObject.SetActive(true);
    }

    public void HideDescription()
    {
        if (Text_StrategyDescription == null)
        {
            return;
        }

        Text_StrategyDescription.text = "";
        Text_StrategyDescription.gameObject.SetActive(false);
    }

    private void RefreshRootStrategyOptions()
    {
        HideDescription();

        if (GameDataManager.Instance == null)
        {
            Debug.LogError("경기 전략 선택지 표시 실패 GameDataManager 없음");
            return;
        }

        _visibleStrategyOptions = GameDataManager.Instance.GetRootMatchStrategyOptions();

        RefreshStrategySlot(Button_Strategy1, Text_StrategyName1, Tooltip_Strategy1, 0);
        RefreshStrategySlot(Button_Strategy2, Text_StrategyName2, Tooltip_Strategy2, 1);
        RefreshStrategySlot(Button_Strategy3, Text_StrategyName3, Tooltip_Strategy3, 2);
        RefreshStrategySlot(Button_Strategy4, Text_StrategyName4, Tooltip_Strategy4, 3);
    }

    private void RefreshStrategySlot(UIButton button, Text nameText, MatchStrategyTooltipUI tooltip, int index)
    {
        if (index < 0 || index >= _visibleStrategyOptions.Count)
        {
            button.SetInteractable(false);
            nameText.text = "미등록";
            tooltip.Setup(this, "");
            return;
        }

        MatchStrategyOptionData optionData = _visibleStrategyOptions[index];

        if (optionData == null)
        {
            button.SetInteractable(false);
            nameText.text = "미등록";
            tooltip.Setup(this, "");
            return;
        }

        nameText.text = optionData.Name;

        tooltip.Setup(this, optionData.Description);

        bool interactable = true;

        if (optionData.ActionType == MatchStrategyOptionActionType.Disabled)
        {
            interactable = false;
        }

        button.SetInteractable(interactable);
    }
}
