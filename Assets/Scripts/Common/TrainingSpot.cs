using UnityEngine;

public class TrainingSpot : MonoBehaviour, ITrainingSpot
{
    private const string RestTrainingType = "Rest";
    private const float RestTrainingStaminaMin = 0f;

    [SerializeField] private string TrainingDataIdValue;
    [SerializeField] private bool IsUnlockedValue = true;
    [SerializeField] private float BaseAttraction = 10f;
    [SerializeField] private float RestMinBonus = 1000f;
    [SerializeField] private Transform TargetSpot;

    public string TrainingDataId
    {
        get { return TrainingDataIdValue; }
    }

    public bool IsUnlocked
    {
        get { return IsUnlockedValue; }
    }

    public void Bind(string trainingDataId, bool isUnlocked)
    {
        TrainingDataIdValue = trainingDataId;
        IsUnlockedValue = isUnlocked;
    }

    public float GetAttractionScore(FighterModel fighter)
    {
        if (IsUnlocked == false)
        {
            return float.MinValue;
        }

        if (fighter == null)
        {
            return BaseAttraction;
        }

        float score = BaseAttraction;

        TrainingData trainingData = GameDataManager.Instance.GetTrainingData(TrainingDataIdValue);

        if (trainingData == null)
        {
            return score;
        }

        TrainingPolicyData policyData = GameDataManager.Instance.GetTrainingPolicyData(fighter.CurrentTrainingPolicyId);

        if (policyData != null)
        {
            if (trainingData.Category == policyData.Category && trainingData.Focus == policyData.Focus)
            {
                score = score + policyData.AttractionBonus;
            }
        }

        if (trainingData.TrainingType == RestTrainingType)
        {
            if (fighter.TrainingStamina <= RestTrainingStaminaMin)
            {
                score = score + RestMinBonus;
            }
        }

        return score;
    }

    public Transform GetTargetSpot()
    {
        if (TargetSpot != null)
        {
            return TargetSpot;
        }

        return transform;
    }
}
