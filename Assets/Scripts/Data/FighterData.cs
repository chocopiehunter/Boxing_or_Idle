using System.Collections.Generic;
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
    public string StartingSkillIds;

    public List<string> GetStartingSkillIdList()
    {
        List<string> skillIds = new List<string>();

        if (string.IsNullOrEmpty(StartingSkillIds))
        {
            return skillIds;
        }

        if (StartingSkillIds == "None")
        {
            return skillIds;
        }

        string[] splitSkillIds = StartingSkillIds.Split(',');

        for (int index = 0; index < splitSkillIds.Length; index++)
        {
            string skillId = splitSkillIds[index].Trim();

            if (string.IsNullOrEmpty(skillId))
            {
                continue;
            }

            if (skillId == "None")
            {
                continue;
            }

            skillIds.Add(skillId);
        }

        return skillIds;
    }
}
