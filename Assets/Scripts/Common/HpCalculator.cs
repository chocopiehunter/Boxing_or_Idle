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

    public static float CalculateLostHpRate(float maxHp, float remainingHp)
    {
        if (maxHp <= 0f)
        {
            Debug.LogError("최대 체력이 0 이하");
            return 1f;
        }

        float lostHp = maxHp - remainingHp;
        if (lostHp < 0f)
        {
            lostHp = 0f;
        }

        return lostHp / maxHp;
    }
}
