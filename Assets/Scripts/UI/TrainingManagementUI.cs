using UnityEngine;
using UnityEngine.UI;

public class TrainingManagementUI : UIBase
{
    [SerializeField] private Text Text_Name;
    [SerializeField] private Text Text_CurrentTraining;
    [SerializeField] private Text Text_Stats;

    [SerializeField] private UIButton_Check Button_Rest;
    [SerializeField] private UIButton_Check Button_HpTraining;
    [SerializeField] private UIButton_Check Button_AtkTraining;
    [SerializeField] private UIButton_Check Button_DefTraining;
    [SerializeField] private UIButton Button_Close;

    private FighterModel _targetFighter;

    private void OnEnable()
    {
        Button_Rest.BindOnClickButtonEvent(OnClick_Rest);
        Button_HpTraining.BindOnClickButtonEvent(OnClick_HpTraining);
        Button_AtkTraining.BindOnClickButtonEvent(OnClick_AtkTraining);
        Button_DefTraining.BindOnClickButtonEvent(OnClick_DefTraining);
        Button_Close.BindOnClickButtonEvent(OnClick_Close);

        _targetFighter = FighterManager.Instance.GetFirstFighter();
        RefreshUI();
    }



    private void RefreshUI()
    {
        if (_targetFighter == null)
        {
            Text_Name.text = "선수 없음";
            Text_CurrentTraining.text = "-";
            Text_Stats.text = "-";
            return;
        }

        Text_Name.text = _targetFighter.Name;

        TrainingData trainingData = GameDataManager.Instance.GetTrainingData(_targetFighter.CurrentTrainingId);

        string trainingName;
        if (trainingData != null)
        {
            trainingName = trainingData.Name;
        }
        else
        {
            trainingName = _targetFighter.CurrentTrainingId;
        }

        Text_CurrentTraining.text = $"현재 훈련 정책: {trainingName}";

        Text_Stats.text = $"Hp {_targetFighter.Hp} / Atk {_targetFighter.Atk} / Def {_targetFighter.Def} / Condition {_targetFighter.Condition}";
        
        RefreshCheckUI();
    }

    private void ChangeTraining(string trainingId)
    {
        if (_targetFighter == null)
        {
            Debug.LogError("선수가 없습니다");
            return;
        }

        _targetFighter.CurrentTrainingId = trainingId;
        Debug.Log($"{_targetFighter.Name} 훈련 정책 변경 -> {trainingId}");
        RefreshUI();
    }

    private void RefreshCheckUI()
    {
        if (_targetFighter == null)
        {
            Button_Rest.SetChecked(false);
            Button_HpTraining.SetChecked(false);
            Button_AtkTraining.SetChecked(false);
            Button_DefTraining.SetChecked(false);
            return;
        }

        string currentId = _targetFighter.CurrentTrainingId;

        Button_Rest.SetChecked(currentId == "training_00");
        Button_HpTraining.SetChecked(currentId == "training_01");
        Button_AtkTraining.SetChecked(currentId == "training_02");
        Button_DefTraining.SetChecked(currentId == "training_03");
    }

    private void OnClick_Rest()
    {
        ChangeTraining("training_00");
    }

    private void OnClick_HpTraining()
    {
        ChangeTraining("training_01");
    }
    private void OnClick_AtkTraining()
    {
        ChangeTraining("training_02");
    }

    private void OnClick_DefTraining()
    {
        ChangeTraining("training_03");
    }

    private void OnClick_Close()
    {
        UIManager.Instance.ClosePopupUI(UIType.TrainingManagementUI);
    }
}
