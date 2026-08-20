using System.Collections.Generic;
using UnityEngine;

public class FighterModel
{
    public string DataId { get; private set; }
    public string Name { get; private set; }
    public float Hp { get; set; }
    public float Atk { get; set; }
    public float Def { get; set; }
    public float Condition { get; set; }

    public string ActiveTrainingId { get; set; }
    public string CurrentTrainingId { get; set; }
    public string PortraitAddress { get; private set; }
    public string BodyAddress { get; private set; }

    public float TrainingHp { get; set; }
    public float TrainingHpMax { get; private set; }

    private Dictionary<string, float> _trainingProgressById;

    public FighterModel(FighterData data, string defaultTrainingId)
    {
        DataId = data.Id;
        Name = data.Name;
        Hp = data.Hp;
        Atk = data.Atk;
        Def = data.Def;
        Condition = 100f;

        if (data.TrainingHp > 0f)
        {
            TrainingHpMax = data.TrainingHp;
        }
        else
        {
            TrainingHpMax = 100f;
        }

        TrainingHp = TrainingHpMax;
        CurrentTrainingId = defaultTrainingId;
        PortraitAddress = data.PortraitAddress;
        BodyAddress = data.BodyAddress;
        _trainingProgressById = new Dictionary<string, float>();
    }

    public float GetTrainingProgress(string trainingId)
    {
        if (string.IsNullOrEmpty(trainingId) == true)
        {
            return 0f;
        }

        if (_trainingProgressById.ContainsKey(trainingId) == false)
        {
            return 0f;
        }

        return _trainingProgressById[trainingId];
    }

    public void SetTrainingProgress(string trainingId, float progress)
    {
        if (string.IsNullOrEmpty(trainingId) == true)
        {
            return;
        }

        _trainingProgressById[trainingId] = progress;
    }

    public void ResetTrainingProgress(string trainingId)
    {
        SetTrainingProgress(trainingId, 0f);
    }

    public bool AddTrainingProgress(string trainingId, float seconds, float duration)
    {
        if (string.IsNullOrEmpty (trainingId) == true)
        {
            return false;
        }

        if (duration <= 0f)
        {
            return false;
        }

        float current = GetTrainingProgress(trainingId);
        current = current + seconds;

        if (current >= duration)
        {
            ResetTrainingProgress(trainingId);
            return true;
        }

        SetTrainingProgress(trainingId, current);
        return false;
    }

    public void ApplyTrainingHpChange(float hpChange)
    {
        TrainingHp = TrainingHp + hpChange;

        if (TrainingHp < 0f)
        {
            TrainingHp = 0f;
        }

        if (TrainingHp > TrainingHpMax)
        {
            TrainingHp = TrainingHpMax;
        }
    }

    public bool IsTrainingHpFull()
    {
        return TrainingHp >= TrainingHpMax;
    }
}
