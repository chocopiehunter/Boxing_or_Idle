using UnityEngine;

public class MatchCombatModel
{
    public MatchSituation CurrentSituation { get; private set; }
    public WrestlingSituation CurrentWrestlingSituation { get; private set; }
    public GroundPosition CurrentGroundPosition { get; private set; }

    public MatchFighterSide Attacker { get; private set; }
    public MatchFighterSide Defender { get; private set; }

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

    }

    public void ChangeToWrestling()
    {

    }

    public void ChangeToGround()
    {

    }

    public void Reset()
    {
        CurrentSituation = MatchSituation.None;
        CurrentWrestlingSituation = WrestlingSituation.None;
        CurrentGroundPosition = GroundPosition.None;
    }
}
