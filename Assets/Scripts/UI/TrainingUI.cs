using System;
using UnityEngine;

public class TrainingUI : UIBase
{
    [SerializeField] private UIButton Button_Training;
    [SerializeField] private UIButton Button_Rest;

    public event Action<ActionType> OnActionSelected;

    private void OnEnable()
    {
        Button_Training.BindOnClickButtonEvent(OnClick_Training);
        Button_Rest.BindOnClickButtonEvent(OnClick_Rest);
    }

    private void OnClick_Training()
    {
        SelectAction(ActionType.Training);
    }

    private void OnClick_Rest()
    {
        SelectAction(ActionType.Rest);
    }

    private void SelectAction(ActionType actionType)
    {
        Debug.Log($"행동 선택 : {actionType}");

        OnActionSelected?.Invoke(actionType);

        if (SeasonManager.Instance != null)
        {
            SeasonManager.Instance.AdvanceWeek();
        }
    }
}
