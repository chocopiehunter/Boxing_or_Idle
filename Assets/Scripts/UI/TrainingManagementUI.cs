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
    private const string RestFacilityType = "rest";
    private const string CardioFacilityType = "cardio";
    private const string StandingOffenseFacilityType = "standing_offense";
    private const string StandingDefenseFacilityType = "standing_defense";

    private void OnEnable()
    {
        Button_Rest.BindOnClickButtonEvent(OnClick_Rest);
        Button_HpTraining.BindOnClickButtonEvent(OnClick_HpTraining);
        Button_AtkTraining.BindOnClickButtonEvent(OnClick_AtkTraining);
        Button_DefTraining.BindOnClickButtonEvent(OnClick_DefTraining);
        Button_Close.BindOnClickButtonEvent(OnClick_Close);

        if (_targetFighter == null)
        {
            _targetFighter = FighterManager.Instance.GetFirstFighter();

        }

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

        TrainingPolicyData policyData = GameDataManager.Instance.GetTrainingPolicyData(_targetFighter.CurrentTrainingPolicyId);

        string policyName;

        if (policyData != null)
        {
            policyName = policyData.Name;
        }
        else
        {
            policyName = _targetFighter.CurrentTrainingPolicyId;
        }

        Text_CurrentTraining.text = $"현재 훈련 정책: {policyName}";

        Text_Stats.text = $"Hp {_targetFighter.Hp} / Atk {_targetFighter.StandingOffense} / Def {_targetFighter.StandingDefense} / Condition {_targetFighter.Condition}";
        
        RefreshCheckUI();
    }

    private void ChangeTraining(string facilityType)
    {
        if (_targetFighter == null)
        {
            Debug.LogError("선수가 없습니다");
            return;
        }

        TrainingPolicyData policyData = GetTrainingPolicyByFacilityType(facilityType);

        if (policyData == null)
        {
            Debug.LogError($"시설과 연결된 훈련 정책 데이터 없음 Type: {facilityType}");
            return;
        }

        _targetFighter.CurrentTrainingPolicyId = policyData.Id;
        FighterManager.Instance.NotifyAttractionChanged(_targetFighter);

        Debug.Log($"{_targetFighter.Name} 훈련 정책 변경 -> {policyData.Id}");
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

        Button_Rest.SetChecked(IsCurrentFacilityPolicy(RestFacilityType));
        Button_HpTraining.SetChecked(IsCurrentFacilityPolicy(CardioFacilityType));
        Button_AtkTraining.SetChecked(IsCurrentFacilityPolicy(StandingOffenseFacilityType));
        Button_DefTraining.SetChecked(IsCurrentFacilityPolicy(StandingDefenseFacilityType));
    }

    private bool IsCurrentFacilityPolicy(string facilityType)
    {
        TrainingPolicyData policyData = GetTrainingPolicyByFacilityType(facilityType);

        if (policyData == null)
        {
            return false;
        }

        return _targetFighter.CurrentTrainingPolicyId == policyData.Id;
    }

    private TrainingPolicyData GetTrainingPolicyByFacilityType(string facilityType)
    {
        TrainingFacilityData facilityData = GymManager.Instance.GetCurrentFacilityData(facilityType);

        if (facilityData == null)
        {
            return null;
        }

        TrainingData trainingData = GameDataManager.Instance.GetTrainingData(facilityData.TrainingDataId);

        if (trainingData == null)
        {
            return null;
        }

        return GameDataManager.Instance.GetTrainingPolicyDataByCategoryAndFocus(trainingData.Category, trainingData.Focus);
    }

    public void SetTargetFighter(FighterModel fighter)
    {
        _targetFighter = fighter;
        RefreshUI();
    }

    private void OnClick_Rest()
    {
        ChangeTraining(RestFacilityType);
    }

    private void OnClick_HpTraining()
    {
        ChangeTraining(CardioFacilityType);
    }
    private void OnClick_AtkTraining()
    {
        ChangeTraining(StandingOffenseFacilityType);
    }

    private void OnClick_DefTraining()
    {
        ChangeTraining(StandingDefenseFacilityType);
    }

    private void OnClick_Close()
    {
        UIManager.Instance.ClosePopupUI(UIType.TrainingManagementUI);
    }
}
