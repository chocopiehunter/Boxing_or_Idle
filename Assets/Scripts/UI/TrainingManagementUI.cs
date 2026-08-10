using UnityEngine;
using UnityEngine.UI;

public class TrainingManagementUI : UIBase
{
    [SerializeField] private Text Text_Name;
    [SerializeField] private Text Text_CurrentTraining;
    [SerializeField] private Text Text_Stats;

    [SerializeField] private UIButton Button_Rest;
    [SerializeField] private UIButton Button_HpTraining;
    [SerializeField] private UIButton Button_AtkTraining;
    [SerializeField] private UIButton Button_DefTraining;
    [SerializeField] private UIButton Button_Close;

    private FighterModel _targetFighter;

    private void OnEnable()
    {
        Button_Rest.BindOnClickButtonEvent(OnClick_Rest);
        Button_HpTraining.BindOnClickButtonEvent(OnClick_HpTraining);
        Button_AtkTraining.BindOnClickButtonEvent(OnClick_AtkTraining);
        Button_DefTraining.BindOnClickButtonEvent(OnClick_DefTraining);
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
    }

    private void RefreshUI()
    {
        if(_targetFighter == null)
        {
            Text_Name.text = "선수 없음";
            Text_CurrentTraining.text = "-";
            Text_Stats.text = "-";
            return;
        }

        Text_Name.text = _targetFighter.Name;

    }

    private void OnClick_Rest()
    {

    }

    private void OnClick_HpTraining()
    {

    }
    private void OnClick_AtkTraining()
    {

    }

    private void OnClick_DefTraining()
    {

    }

    private void OnClick_Close()
    {
        UIManager.Instance.ClosePopupUI(UIType.TrainingManagementUI);
    }
}
