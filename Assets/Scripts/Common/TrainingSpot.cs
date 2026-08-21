using UnityEngine;

public class TrainingSpot : MonoBehaviour, ITrainingSpot
{
    private const string RestTrainingType = "Rest";
    private const float RestTrainingHpMin = 0f;

    [SerializeField] private string TrainingDataIdValue;
    [SerializeField] private bool IsUnlockedValue = true;
    [SerializeField] private float BaseAttraction = 10f;
    [SerializeField] private float PolicyBonus = 50f;
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

        if (fighter.CurrentTrainingId == TrainingDataIdValue)
        {
            score = score + PolicyBonus;
        }

        TrainingData trainingData = GameDataManager.Instance.GetTrainingData(TrainingDataIdValue);
        if (trainingData == null)
        {
            return score;
        }

        if (trainingData.TrainingType == RestTrainingType)
        {
            if (fighter.TrainingHp <= RestTrainingHpMin)
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
