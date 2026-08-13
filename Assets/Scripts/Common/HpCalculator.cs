using UnityEngine;

public static class HpCalculator
{
    private const float MinDamage = 1f;

    public static float CalculateRemainingHp(float maxHp, float defenderDef, float attackerAtk)
    {
        float damage = attackerAtk - defenderDef;

        if (damage < MinDamage)
        {
            return MinDamage;
        }

        float remainingHp = maxHp - damage;
        if (remainingHp < 0f)
        {
            remainingHp = 0f;
        }

        return remainingHp;
    }
}
