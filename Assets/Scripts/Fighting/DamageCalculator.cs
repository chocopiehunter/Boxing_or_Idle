using UnityEngine;

public static class DamageCalculator
{
    private const float MinBaseDamage = 1f;

    public static float Calculate(float offense, float defense, float damageMultiplier)
    {
        if (damageMultiplier <= 0f)
        {
            return 0f;
        }

        float baseDamage = offense - defense;

        if (baseDamage < MinBaseDamage)
        {
            baseDamage = MinBaseDamage;
        }

        float damage = baseDamage * damageMultiplier;

        return damage;
    }
}
