using UnityEngine;

public static class SuccessChanceCalculator
{
    public static float Calculate(float baseSuccessChance, float offense, float defense)
    {
        float successChance = baseSuccessChance;
        float totalStat = offense + defense;

        if (totalStat > 0f)
        {
            float offenseRate = offense / totalStat;
            successChance = baseSuccessChance * offenseRate * 2f;
        }

        successChance = Mathf.Clamp(successChance, 0f, 100f);

        return successChance;
    }
}
