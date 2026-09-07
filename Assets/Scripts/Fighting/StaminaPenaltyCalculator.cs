using UnityEngine;

public static class StaminaPenaltyCalculator
{
    private const float MinStatRate = 0.5f;

    public static float ApplyStaminaPenalty(float baseStat, MatchFighterModel fighter)
    {
        if (baseStat <= 0f)
        {
            return 0f;
        }

        if (fighter == null)
        {
            return baseStat;
        }

        if (fighter.MaxStamina <= 0f)
        {
            return baseStat;
        }

        float staminaRate = fighter.CurrentStamina / fighter.MaxStamina;
        staminaRate = Mathf.Clamp01(staminaRate);

        float statRate = MinStatRate + ((1f - MinStatRate) * staminaRate);

        return baseStat * statRate;
    }
}
