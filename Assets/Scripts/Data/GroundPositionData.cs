using UnityEngine;

[System.Serializable]
public class GroundPositionData : GameDataBase
{
    public string Position;
    public float GroundStrikeDamageMultiplier;
    public float PositionEntrySuccessMultiplier;
    public float GroundEscapeSuccessMultiplier;
    public float BottomStaminaLossPerSecond;
    public float SubmissionSuccessMultiplier;
}
