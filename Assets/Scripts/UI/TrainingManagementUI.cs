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
        
    }

    private void RefreshUI()
    {

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
