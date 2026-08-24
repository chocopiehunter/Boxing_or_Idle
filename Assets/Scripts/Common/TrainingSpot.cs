using UnityEngine;

public class TrainingSpot : MonoBehaviour, ITrainingSpot
{
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

        return AttractionCalculator.Caculate(BaseAttraction, RestMinBonus, TrainingDataIdValue, fighter);
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
