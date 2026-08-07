using System;
using UnityEngine;

public class TrainingUI : UIBase
{
    [SerializeField] private UIButton Button_Training;
    [SerializeField] private UIButton Button_Rest;

    public event Action<ActionType> OnActionSelected;

    private void OnClick_Training()
    {

    }

    private void OnClick_Rest()
    {

    }

    private void SelectAction(ActionType actionType)
    {

    }
}
