using UnityEngine;

public enum CombatActionResultType
{
    None,

    StrikeHit,
    StrikeMissed,

    TakedownStarted,
    TakedownSucceeded,
    TakedownDefended,
    TakedownToClinch,

    ClinchStarted,
    ClinchReversed,
    ClinchEscaped
}
