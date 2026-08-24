using UnityEngine;

public static class AttractionCalculator
{
    private const string RestTrainingType = "Rest";
    private const float RestTrainingStaminaMin = 0f;

    public static float Calculate(float baseAttraction, float restMinBonus, string trainingDataId, FighterModel fighter)
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
        attractionScore = AddRestAttractionBonus(attractionScore, restMinBonus, trainingData, fighter);

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

    private static float AddRestAttractionBonus(float attractionScore, float restMinBonus, TrainingData trainingData, FighterModel fighter)
    {
        if (trainingData.TrainingType != RestTrainingType)
        {
            return attractionScore;
        }

        if (fighter.TrainingStamina > RestTrainingStaminaMin)
        {
            return attractionScore;
        }

        return attractionScore + restMinBonus;
    }
}
