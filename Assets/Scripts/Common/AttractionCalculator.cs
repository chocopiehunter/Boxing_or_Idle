using UnityEngine;

public static class AttractionCalculator
{
    private const string RestTrainingType = "Rest";
    private const float RestTrainingStaminaMin = 0f;

    public static float Caculate(float baseAttraction, float restMinBonus, string trainingDataId, FighterModel fighter)
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

        return Mathf.Max(0f, attractionScore);
    }

    private static float AddPolicyAttractionBonus(float attractionScore, TrainingData trainingData, FighterModel fighter)
    {
        return attractionScore;
    }

    private static float AddRestAttractionBonus(float attractionScore, float restMinBonus, TrainingData trainingData, FighterModel fighter)
    {
        return attractionScore + restMinBonus;
    }
}
