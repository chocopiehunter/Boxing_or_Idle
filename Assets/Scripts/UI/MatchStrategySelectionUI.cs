using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchStrategySelectionUI : UIBase
{
    [SerializeField] private Text Text_StrategyDescription;

    [SerializeField] private RectTransform RectTransform_StrategyDescription;
    [SerializeField] private Vector2 TooltipOffset = new Vector2(20f, -20f);

    private bool _isDescriptionVisible;

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

    private void Update()
    {
        if (_isDescriptionVisible == false)
        {
            return;
        }

        UpdateDescriptionPosition();
    }

    private void OnEnable()
    {
        BindStrategyButtonEvents();
        RefreshRootStrategyOptions();
    }

    private void BindStrategyButtonEvents()
    {
        Button_Strategy1.UnBindAllOnClickButtonEvent();
        Button_Strategy2.UnBindAllOnClickButtonEvent();
        Button_Strategy3.UnBindAllOnClickButtonEvent();
        Button_Strategy4.UnBindAllOnClickButtonEvent();

        Button_Strategy1.BindOnClickButtonEvent(OnClick_Strategy1);
        Button_Strategy2.BindOnClickButtonEvent(OnClick_Strategy2);
        Button_Strategy3.BindOnClickButtonEvent(OnClick_Strategy3);
        Button_Strategy4.BindOnClickButtonEvent(OnClick_Strategy4);
    }

    public void ShowDescription(string description)
    {
        if (Text_StrategyDescription == null || RectTransform_StrategyDescription == null)
        {
            return;
        }

        Text_StrategyDescription.text = description;
        RectTransform_StrategyDescription.gameObject.SetActive(true);
        _isDescriptionVisible = true;
        UpdateDescriptionPosition();
    }

    public void HideDescription()
    {
        if (Text_StrategyDescription != null)
        {
            Text_StrategyDescription.text = "";
        }

        _isDescriptionVisible = false;

        if (RectTransform_StrategyDescription != null)
        {
            RectTransform_StrategyDescription.gameObject.SetActive(false);
        }
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
        RefreshVisibleStrategyOptions();
    }

    private void RefreshVisibleStrategyOptions()
    {
        RefreshStrategySlot(Button_Strategy1, Text_StrategyName1, Tooltip_Strategy1, 0);
        RefreshStrategySlot(Button_Strategy2, Text_StrategyName2, Tooltip_Strategy2, 1);
        RefreshStrategySlot(Button_Strategy3, Text_StrategyName3, Tooltip_Strategy3, 2);
        RefreshStrategySlot(Button_Strategy4, Text_StrategyName4, Tooltip_Strategy4, 3);
    }

    private void TrySelectOption(int index)
    {
        HideDescription();

        if (index < 0 || index >= _visibleStrategyOptions.Count)
        {
            return;
        }

        MatchStrategyOptionData optionData = _visibleStrategyOptions[index];

        if (optionData == null)
        {
            Debug.LogError($"경기 전략 선택 실패함. 선택지 데이터가 없음");
            return;
        }

        if (optionData.ActionType == MatchStrategyOptionActionType.OpenSubOptions)
        {
            OpenSubOptions(optionData.Id);
            return;
        }

        if (optionData.ActionType == MatchStrategyOptionActionType.ApplyStrategy)
        {
            ApplyStrategy(optionData);
            return;
        }

        if (optionData.ActionType == MatchStrategyOptionActionType.KeepCurrent)
        {
            KeepCurrentStrategy();
            return;
        }

        if (optionData.ActionType == MatchStrategyOptionActionType.Disabled)
        {
            return;
        }

        Debug.LogError($"경기 전략 선택 실패. ActionType : {optionData.ActionType}");
    }

    private void OpenSubOptions(string parentOptionId)
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("하위 경기 전략 표시 실패. GameDataManager 없음");
            return;
        }

        _visibleStrategyOptions = GameDataManager.Instance.GetMatchStrategyOptionsByParent(parentOptionId);

        if (_visibleStrategyOptions.Count == 0)
        {
            Debug.LogError($"하위 경기 전략 표시 실패. 하위 선택지 없음 ParentOptionId : {parentOptionId}");
            RefreshRootStrategyOptions();
            return;
        }

        RefreshVisibleStrategyOptions();
    }

    private void ApplyStrategy(MatchStrategyOptionData optionData)
    {
        if (MatchManager.Instance == null)
        {
            Debug.LogError("경기 전략 적용 실패. MatchManager 없음");
            return;
        }

        bool changeSuccess = MatchManager.Instance.TryChangeMatchStrategy(optionData.StrategyId);

        if (changeSuccess == false)
        {
            return;
        }

        RefreshRootStrategyOptions();
    }

    private void KeepCurrentStrategy()
    {
        if (MatchManager.Instance == null)
        {
            Debug.LogError("경기 전략 유지 실패. MatchManager 없음");
            return;
        }

        if (MatchManager.Instance.CurrentStrategyData == null)
        {
            Debug.LogError("경기 전략 유지 실패. 현재 전략 없음");
            return;
        }

        Debug.Log($"현재 경기 전략 유지: {MatchManager.Instance.CurrentStrategyData.Name}");
        RefreshRootStrategyOptions();
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

    private void UpdateDescriptionPosition()
    {
        if (RectTransform_StrategyDescription == null)
        {
            return;
        }

        RectTransform parentRectTransform = RectTransform_StrategyDescription.parent as RectTransform;

        if (parentRectTransform == null)
        {
            return;
        }

        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPosition;

        bool positionSuccess = RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, mousePosition, null, out localPosition);

        if (positionSuccess == false)
        {
            return;
        }

        RectTransform_StrategyDescription.anchoredPosition = localPosition + TooltipOffset;
    }

    private void OnClick_Strategy1()
    {
        TrySelectOption(0);
    }

    private void OnClick_Strategy2()
    {
        TrySelectOption(1);
    }

    private void OnClick_Strategy3()
    {
        TrySelectOption(2);
    }

    private void OnClick_Strategy4()
    {
        TrySelectOption(3);
    }
}
