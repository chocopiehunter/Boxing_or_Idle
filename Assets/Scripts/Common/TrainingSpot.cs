using UnityEngine;

public class TrainingSpot : MonoBehaviour, ITrainingSpot
{
    [SerializeField] private string TrainingDataIdValue;
    [SerializeField] private bool IsUnlockedValue = true;
    [SerializeField] private float BaseAttraction = 10f;
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

        return BaseAttraction;
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
