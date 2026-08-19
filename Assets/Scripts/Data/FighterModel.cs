using UnityEngine;

public class FighterModel
{
    public string DataId { get; private set; }
    public string Name { get; private set; }
    public float Hp { get; set; }
    public float Atk { get; set; }
    public float Def { get; set; }
    public float Condition { get; set; }

    public string CurrentTrainingId { get; set; }
    public string PortraitAddress { get; private set; }
    public string BodyAddress { get; private set; }

    public float TrainingHp { get; set; }
    public float TrainingHpMax { get; private set; }

    public FighterModel(FighterData data, string defaultTrainingId)
    {
        DataId = data.Id;
        Name = data.Name;
        Hp = data.Hp;
        Atk = data.Atk;
        Def = data.Def;
        Condition = 100f;
        TrainingHpMax = 100f;
        TrainingHp = TrainingHpMax;
        CurrentTrainingId = defaultTrainingId;
        PortraitAddress = data.PortraitAddress;
        BodyAddress = data.BodyAddress;
    }
}
