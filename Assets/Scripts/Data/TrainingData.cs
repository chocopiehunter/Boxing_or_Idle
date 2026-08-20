using UnityEngine;

[System.Serializable]
public class TrainingData : GameDataBase
{
    public string Name;
    public string Description;
    public string TrainingType;
    public float Time;
    public float TrainingHpPerSecond;
    public float HpUp;
    public float HpDown;
    public float AtkUp;
    public float AtkDown;
    public float DefUp;
    public float DefDown;
    public float ConditionUp;
    public float ConditionDown;
}
