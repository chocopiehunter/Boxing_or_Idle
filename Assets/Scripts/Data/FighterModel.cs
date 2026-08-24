using System.Collections.Generic;
using UnityEngine;

public class FighterModel
{
    public string DataId { get; private set; }
    public string Name { get; private set; }
    public float Hp { get; set; }
    public float Stamina { get; set; }
    public float StandingOffense { get; set; }
    public float StandingDefense { get; set; }
    public float WrestlingOffense { get; set; }
    public float WrestlingDefense { get; set; }
    public float JiuJitsuOffense { get; set; }
    public float JiuJitsuDefense { get; set; }
    public float Condition { get; set; }

    public string LastCompletedTrainingId { get; set; }
    public string ActiveTrainingId { get; set; }
    public string CurrentTrainingId { get; set; }
    public string CurrentTrainingPolicyId { get; set; }
    public string PortraitAddress { get; private set; }
    public string BodyAddress { get; private set; }

    public float TrainingStamina { get; set; }
    public float TrainingStaminaMax { get; private set; }
    public FighterActivityState ActivityState { get; set; }
    public ITrainingSpot ActiveSpot { get; set; }
    public bool IsAttractionChanged { get; set; }

    private Dictionary<string, float> _trainingProgressById;

    public FighterModel(FighterData data, string defaultTrainingId, string defaultTrainingPolicyId)
    {
        DataId = data.Id;
        Name = data.Name;
        Hp = data.Hp;
        Stamina = data.Stamina;
        StandingOffense = data.StandingOffense;
        StandingDefense = data.StandingDefense;
        WrestlingOffense = data.WrestlingOffense;
        WrestlingDefense = data.WrestlingDefense;
        JiuJitsuOffense = data.JiuJitsuOffense;
        JiuJitsuDefense = data.JiuJitsuDefense;
        Condition = 100f;

        TrainingStamina = 100f;
        TrainingStaminaMax = TrainingStamina;

        if (data.TrainingStamina > 0f)
        {
            TrainingStaminaMax = data.TrainingStamina;
        }
        else
        {
            TrainingStaminaMax = 100f;
        }

        TrainingStamina = TrainingStaminaMax;
        CurrentTrainingId = defaultTrainingId;
        CurrentTrainingPolicyId = defaultTrainingPolicyId;
        LastCompletedTrainingId = null;
        PortraitAddress = data.PortraitAddress;
        BodyAddress = data.BodyAddress;
        _trainingProgressById = new Dictionary<string, float>();
        ActivityState = FighterActivityState.Idle;
        ActiveSpot = null;
        IsAttractionChanged = true;
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

    public void ApplyTrainingStaminaChange(float staminaChange)
    {
        TrainingStamina = TrainingStamina + staminaChange;

        if (TrainingStamina < 0f)
        {
            TrainingStamina = 0f;
        }

        if (TrainingStamina > TrainingStaminaMax)
        {
            TrainingStamina = TrainingStaminaMax;
        }
    }

    public bool IsTrainingStaminaFull()
    {
        return TrainingStamina >= TrainingStaminaMax;
    }
}
