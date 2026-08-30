using UnityEngine;

public class CombatActionResult
{
    public MatchCombatAction Action { get; private set; }
    public bool IsSuccess { get; private set; }
    public float SuccessChance { get; private set; }

    public CombatActionResult(MatchCombatAction action, bool isSuccess, float successChance)
    {
        Action = action;
        IsSuccess = isSuccess;
        SuccessChance = successChance;
    }
}
