using System.Collections.Generic;
using UnityEngine;

public class MatchFighterModel
{
    public MatchFighterSide FighterSide { get; private set; }
    public float MaxHp { get; private set; }
    public float CurrentHp { get; private set; }
    public float MaxStamina { get; private set; }
    public float CurrentStamina { get; private set; }
    public float StandingOffense { get; private set; }
    public float StandingDefense { get; private set; }
    public float WrestlingOffense { get; private set; }
    public float WrestlingDefense { get; private set; }
    public float JiuJitsuOffense { get; private set; }
    public float JiuJitsuDefense { get; private set; }

    private List<string> _ownedSkillIds;

    private MatchCoolTimeModel _coolTimeModel;

    public IReadOnlyList<string> OwnedSkillIds
    {
        get
        {
            return _ownedSkillIds;
        }
    }

    public MatchFighterModel(MatchFighterSide fighterSide, float hp, float stamina, float standingOffense, float standingDefense, float wrestlingOffense, float wrestlingDefense, 
                                float jiujitsuOffense, float jiujitsuDefense, IReadOnlyList<string> ownedSkillIds)
    {
        FighterSide = fighterSide;
        MaxHp = hp;
        CurrentHp = hp;
        MaxStamina = stamina;
        CurrentStamina = stamina;
        StandingOffense = standingOffense;
        StandingDefense = standingDefense;
        WrestlingOffense = wrestlingOffense;
        WrestlingDefense = wrestlingDefense;
        JiuJitsuOffense = jiujitsuOffense;
        JiuJitsuDefense = jiujitsuDefense;
        _ownedSkillIds = new List<string>();
        _coolTimeModel = new MatchCoolTimeModel();

        if (ownedSkillIds == null)
        {
            return;
        }

        for (int index = 0; index < ownedSkillIds.Count; index++)
        {
            string skillId = ownedSkillIds[index];

            if (string.IsNullOrEmpty(skillId))
            {
                continue;
            }

            if (skillId == "None")
            {
                continue;
            }

            if (_ownedSkillIds.Contains(skillId))
            {
                continue;
            }

            _ownedSkillIds.Add(skillId);
        }

    }

    public bool HasSkill(string skillId)
    {
        if (string.IsNullOrEmpty(skillId))
        {
            return false;
        }

        return _ownedSkillIds.Contains(skillId);
    }

    public bool IsSkillReady(string skillId)
    {
        if (HasSkill(skillId) == false)
        {
            return false;
        }

        return _coolTimeModel.IsSkillReady(skillId);
    }

    public bool TryStartSkillCooldown(SkillData skillData)
    {
        if (skillData == null)
        {
            return false;
        }

        if (HasSkill(skillData.Id) == false)
        {
            return false;
        }

        return _coolTimeModel.TryStartSkillCooldown(skillData);
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f)
        {
            return;
        }

        CurrentHp = CurrentHp - damage;

        if (CurrentHp < 0f)
        {
            CurrentHp = 0f;
        }
    }

    public void UseStamina(float staminaCost)
    {
        if (staminaCost <= 0f)
        {
            return;
        }

        CurrentStamina = CurrentStamina - staminaCost;

        if (CurrentStamina < 0f)
        {
            CurrentStamina = 0f;
        }
    }

    public void UpdateCooldown(float passedSeconds)
    {
        _coolTimeModel.UpdateCooldown(passedSeconds);
    }

    public float GetRemainingCoolTimeSeconds(string skillId)
    {
        return _coolTimeModel.GetRemainingSeconds(skillId);
    }

    public void ResetCoolTimes()
    {
        _coolTimeModel.Reset();
    }
}
