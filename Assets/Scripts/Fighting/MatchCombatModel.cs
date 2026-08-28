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

    public void Reset()
    {
        CurrentSituation = MatchSituation.None;
        CurrentWrestlingSituation = WrestlingSituation.None;
        CurrentGroundPosition = GroundPosition.None;

        ClearWrestlingRoles();
        ClearGroundRoles();
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
