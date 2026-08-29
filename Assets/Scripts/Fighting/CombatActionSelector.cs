using System.Collections.Generic;
using UnityEngine;

public class CombatActionSelector
{
    public bool TrySelectAction(List<SkillData> playerUsableSkills, List<SkillData> opponentUsableSkills, out MatchCombatAction selectedAction)
    {
        selectedAction = null;

        bool playerCanAct = playerUsableSkills != null && playerUsableSkills.Count > 0;
        bool opponentCanAct = opponentUsableSkills != null && opponentUsableSkills.Count > 0;

        if (playerCanAct == false && opponentCanAct == false)
        {
            return false;
        }

        MatchFighterSide skillUserSide = SelectSkillUserSide(playerCanAct, opponentCanAct);
        List<SkillData> usableSkills = GetUsableSkills(skillUserSide, playerUsableSkills, opponentUsableSkills);
        SkillData selectedSkill = SelectSkill(usableSkills);

        if (selectedSkill == null)
        {
            return false;
        }

        MatchFighterSide targetSide = GetTargetSide(skillUserSide);

        if (targetSide == MatchFighterSide.None)
        {
            return false;
        }

        selectedAction = new MatchCombatAction(skillUserSide, targetSide, selectedSkill);

        return true;
    }

    private MatchFighterSide SelectSkillUserSide(bool playerCanAct, bool opponentCanAct)
    {
        if (playerCanAct && opponentCanAct)
        {
            int selectedSideNumber = Random.Range(0, 2);

            if (selectedSideNumber == 0)
            {
                return MatchFighterSide.Player;
            }

            return MatchFighterSide.Opponent;
        }

        if (playerCanAct)
        {
            return MatchFighterSide.Player;
        }

        if (opponentCanAct)
        {
            return MatchFighterSide.Opponent;
        }

        return MatchFighterSide.None;
    }

    private List<SkillData> GetUsableSkills(MatchFighterSide fighterSide, List<SkillData> playerUsableSkills, List<SkillData> opponentUsableSkills)
    {
        if (fighterSide == MatchFighterSide.Player)
        {
            return playerUsableSkills;
        }

        if (fighterSide == MatchFighterSide.Opponent)
        {
            return opponentUsableSkills;
        }

        return null;
    }

    private SkillData SelectSkill(List<SkillData> usableSkills)
    {
        if (usableSkills == null)
        {
            return null;
        }

        if (usableSkills.Count == 0)
        {
            return null;
        }

        int selectedSkillIndex = Random.Range(0, usableSkills.Count);

        return usableSkills[selectedSkillIndex];
    }

    private MatchFighterSide GetTargetSide(MatchFighterSide skillUserSide)
    {
        if (skillUserSide == MatchFighterSide.Player)
        {
            return MatchFighterSide.Opponent;
        }

        if (skillUserSide == MatchFighterSide.Opponent)
        {
            return MatchFighterSide.Player;
        }

        return MatchFighterSide.None;
    }
}
