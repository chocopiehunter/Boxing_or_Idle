using UnityEngine;

public static class AttractionCalculator
{
    public static float Calculate(float baseAttraction, float repeatPenalty, string trainingDataId, FighterModel fighter)
    {
        float attractionScore = baseAttraction;

        if (fighter == null)
        {
            return attractionScore;
        }

        if (GameDataManager.Instance == null)
        {
            return attractionScore;
        }

        TrainingData trainingData = GameDataManager.Instance.GetTrainingData(trainingDataId);

        if (trainingData == null)
        {
            return attractionScore;
        }

        attractionScore = AddPolicyAttractionBonus(attractionScore, trainingData, fighter);
        attractionScore = AddRepeatPenalty(attractionScore, repeatPenalty, trainingDataId, fighter);

        return Mathf.Max(0f, attractionScore);
    }

    private static float AddPolicyAttractionBonus(float attractionScore, TrainingData trainingData, FighterModel fighter)
    {
        TrainingPolicyData policyData = GameDataManager.Instance.GetTrainingPolicyData(fighter.CurrentTrainingPolicyId);

        if (policyData == null)
        {
            return attractionScore;
        }

        if (trainingData.Category != policyData.Category || trainingData.Focus != policyData.Focus)
        {
            return attractionScore;
        }

        return attractionScore + policyData.AttractionBonus;
    }

    private static float AddRepeatPenalty(float attractionScore, float repeatPenalty, string trainingDataId, FighterModel fighter)
    {
        if (fighter.LastCompletedTrainingId != trainingDataId)
        {
            return attractionScore;
        }

        return Mathf.Max(0.01f, attractionScore - Mathf.Max(0f, repeatPenalty));
    }
}
