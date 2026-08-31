using UnityEngine;

public class MatchCombatAction
{
    public MatchFighterSide SkillUserSide { get; private set; }
    public MatchFighterSide TargetSide { get; private set; }
    public SkillData SelectedSkill { get; private set; }

    public MatchCombatAction(MatchFighterSide skillUserSide, MatchFighterSide targetSide, SkillData selectedSkill)
    {
        SkillUserSide = skillUserSide;
        TargetSide = targetSide;
        SelectedSkill = selectedSkill;
    }
}
