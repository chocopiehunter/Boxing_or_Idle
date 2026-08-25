using UnityEngine;

[System.Serializable]
public class FighterData : GameDataBase
{
    public string Name;
    public string Description;
    public string PortraitAddress;
    public string BodyAddress;

    public float Hp;
    public float Stamina;
    public float StandingOffense;
    public float StandingDefense;
    public float WrestlingOffense;
    public float WrestlingDefense;
    public float JiuJitsuOffense;
    public float JiuJitsuDefense;
    public float TrainingStamina;

}
