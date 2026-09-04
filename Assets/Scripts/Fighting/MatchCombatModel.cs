using UnityEngine;

public class MatchCombatModel
{
    public MatchSituation CurrentSituation { get; private set; }
    public WrestlingSituation CurrentWrestlingSituation { get; private set; }
    public GroundPosition CurrentGroundPosition { get; private set; }

    public MatchFighterSide Attacker { get; private set; }
    public MatchFighterSide Defender { get; private set; }

    public MatchFighterSide TopSide { get; private set; }
    public MatchFighterSide BottomSide { get; private set; }
    public MatchFighterSide GroundControllerSide { get; private set; }
    public bool IsSubmissionInProgress { get; private set; }
    public MatchFighterSide SubmissionAttackerSide { get; private set; }
    public MatchFighterSide SubmissionDefenderSide { get; private set; }
    public string CurrentSubmissionSkillId { get; private set; }
    public float MaxSubmissionResistHp { get; private set; }
    public float CurrentSubmissionResistHp { get; private set; }

    public MatchCombatModel()
    {
        Reset();
    }

    public void StartRound()
    {
        ChangeToStanding();
    }

    public void ChangeToStanding()
    {
        CurrentSituation = MatchSituation.Standing;
        CurrentWrestlingSituation = WrestlingSituation.None;
        CurrentGroundPosition = GroundPosition.None;

        ClearWrestlingRoles();
        ClearGroundRoles();
        ClearSubmission();
    }

    public bool ChangeToWrestling(WrestlingSituation wrestlingSituation, MatchFighterSide attacker)
    {
        if (wrestlingSituation == WrestlingSituation.None)
        {
            return false;
        }

        if (IsValidFighterSide(attacker) == false)
        {
            return false;
        }

        CurrentSituation = MatchSituation.Wrestling;
        CurrentWrestlingSituation = wrestlingSituation;
        CurrentGroundPosition = GroundPosition.None;

        Attacker = attacker;
        Defender = GetOpponentSide(attacker);

        ClearGroundRoles();
        ClearSubmission();

        return true;
    }

    public bool ChangeToGround(GroundPosition groundPosition, MatchFighterSide topSide)
    {
        if (groundPosition == GroundPosition.None)
        {
            return false;
        }

        if (IsValidFighterSide(topSide) == false)
        {
            return false;
        }

        CurrentSituation = MatchSituation.Ground;
        CurrentWrestlingSituation = WrestlingSituation.None;
        CurrentGroundPosition = groundPosition;

        ClearWrestlingRoles();

        TopSide = topSide;
        BottomSide = GetOpponentSide(topSide);
        GroundControllerSide = topSide;

        ClearSubmission();

        return true;
    }

    public bool ChangeGroundPosition(GroundPosition groundPosition, bool changeTopBottom)
    {
        if (CurrentSituation != MatchSituation.Ground)
        {
            return false;
        }

        if (groundPosition == GroundPosition.None)
        {
            return false;
        }

        if (IsValidFighterSide(TopSide) == false || IsValidFighterSide(BottomSide) == false)
        {
            return false;
        }

        CurrentGroundPosition = groundPosition;

        if (changeTopBottom == true)
        {
            MatchFighterSide previousTopSide = TopSide;

            TopSide = BottomSide;

            BottomSide = previousTopSide;
        }

        GroundControllerSide = TopSide;
        ClearSubmission();
        return true;
    }

    public bool ChangeGroundController(MatchFighterSide controller)
    {
        if (CurrentSituation != MatchSituation.Ground)
        {
            return false;
        }

        if (IsValidFighterSide(controller) == false)
        {
            return false;
        }

        GroundControllerSide = controller;
        return true;
    }

    public bool StartSubmission(MatchFighterSide attacker, MatchFighterSide defender, string submissionSkillId, float maxSubmissionResistHp)
    {
        if (CurrentSituation != MatchSituation.Ground)
        {
            return false;
        }

        if (IsValidFighterSide(attacker) == false || IsValidFighterSide(defender) == false)
        {
            return false;
        }

        if (attacker == defender)
        {
            return false;
        }

        if (string.IsNullOrEmpty(submissionSkillId) == true)
        {
            return false;
        }

        if (maxSubmissionResistHp <= 0f)
        {
            return false;
        }

        IsSubmissionInProgress = true;
        SubmissionAttackerSide = attacker;
        SubmissionDefenderSide = defender;
        CurrentSubmissionSkillId = submissionSkillId;
        MaxSubmissionResistHp = maxSubmissionResistHp;
        CurrentSubmissionResistHp = maxSubmissionResistHp;

        return true;
    }

    public void ApplySubmissionDamage(float damage)
    {
        if (IsSubmissionInProgress == false)
        {
            return;
        }

        if (damage <= 0f)
        {
            return;
        }

        CurrentSubmissionResistHp = CurrentSubmissionResistHp - damage;

        if (CurrentSubmissionResistHp < 0f)
        {
            CurrentSubmissionResistHp = 0f;
        }
    }

    public void ClearSubmission()
    {
        IsSubmissionInProgress = false;
        SubmissionAttackerSide = MatchFighterSide.None;
        SubmissionDefenderSide = MatchFighterSide.None;
        CurrentSubmissionSkillId = "";
        MaxSubmissionResistHp = 0f;
        CurrentSubmissionResistHp = 0f;
    }

    public void Reset()
    {
        CurrentSituation = MatchSituation.None;
        CurrentWrestlingSituation = WrestlingSituation.None;
        CurrentGroundPosition = GroundPosition.None;

        ClearWrestlingRoles();
        ClearGroundRoles();
        ClearSubmission();
    }

    private void ClearWrestlingRoles()
    {
        Attacker = MatchFighterSide.None;
        Defender = MatchFighterSide.None;
    }

    private void ClearGroundRoles()
    {
        TopSide = MatchFighterSide.None;
        BottomSide = MatchFighterSide.None;
        GroundControllerSide = MatchFighterSide.None;
    }

    private bool IsValidFighterSide(MatchFighterSide fighterSide)
    {
        if (fighterSide == MatchFighterSide.Player)
        {
            return true;
        }

        if (fighterSide == MatchFighterSide.Opponent)
        {
            return true;
        }

        return false;
    }

    private MatchFighterSide GetOpponentSide(MatchFighterSide fighterSide)
    {
        if (fighterSide == MatchFighterSide.Player)
        {
            return MatchFighterSide.Opponent;
        }

        if (fighterSide == MatchFighterSide.Opponent)
        {
            return MatchFighterSide.Player;
        }

        return MatchFighterSide.None;
    }
}
