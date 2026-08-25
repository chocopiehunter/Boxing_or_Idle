using UnityEngine;

public interface ITrainingSpot
{
    string TrainingDataId { get; }
    bool IsUnlocked { get; }
    float GetAttractionScore(FighterModel fighter);
    Transform GetTargetSpot();
}
