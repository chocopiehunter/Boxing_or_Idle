using UnityEngine;

public class MatchPresentationController : MonoBehaviour
{
    [SerializeField] private MatchFighterView PlayerView;
    [SerializeField] private MatchFighterView OpponentView;

    private void OnEnable()
    {
        RefreshFighterDirection();
        BindCombatActionEvent();
    }

    private void OnDisable()
    {
        UnbindCombatActionEvent();
    }

    private void BindCombatActionEvent()
    {
        if (MatchManager.Instance == null)
        {
            Debug.LogError("경기 연출 이벤트 연결 실패 MatchManager없음");
            return;
        }

        MatchManager.Instance.OnCombatActionResolved -= HandleCombatActionResolved;

        MatchManager.Instance.OnCombatActionResolved += HandleCombatActionResolved;
    }

    private void UnbindCombatActionEvent()
    {
        if (MatchManager.Instance == null)
        {
            return;
        }

        MatchManager.Instance.OnCombatActionResolved -= HandleCombatActionResolved;
    }

    private void HandleCombatActionResolved(CombatActionResult actionResult)
    {
        if (actionResult == null || actionResult.Action == null || actionResult.Action.SelectedSkill == null)
        {
            return;
        }

        if (actionResult.Action.SelectedSkill.ActionType != SkillActionType.Strike)
        {
            return;
        }

        RefreshFighterDirection();

        MatchFighterView skillUserView = GetFighterView(actionResult.Action.SkillUserSide);

        if (skillUserView == null)
        {
            return;
        }

        skillUserView.PlayJab();

        if (actionResult.ResultType != CombatActionResultType.StrikeHit)
        {
            return;
        }

        MatchFighterView targetView = GetFighterView(actionResult.Action.TargetSide);

        if (targetView == null)
        {
            return;
        }

        targetView.PlayHit();
    }

    private void RefreshFighterDirection()
    {
        if (PlayerView == null || OpponentView == null)
        {
            Debug.LogError("경기 선수 방향 설정 실패. PlayerView 또는 OpponentView가 연결되지 않음");

            return;
        }

        MatchFighterDirection playerDirection = CalculateDirection(PlayerView.transform.position, OpponentView.transform.position);
        MatchFighterDirection opponentDirection = CalculateDirection(OpponentView.transform.position, PlayerView.transform.position);

        PlayerView.Setup(playerDirection);
        OpponentView.Setup(opponentDirection);
    }

    private MatchFighterDirection CalculateDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        Vector2 difference = targetPosition - sourcePosition;

        if (Mathf.Abs(difference.x) >= Mathf.Abs(difference.y))
        {
            if (difference.x >= 0f)
            {
                return MatchFighterDirection.East;
            }

            return MatchFighterDirection.West;
        }

        if (difference.y >= 0f)
        {
            return MatchFighterDirection.North;
        }

        return MatchFighterDirection.South;
    }

    private MatchFighterView GetFighterView(MatchFighterSide fighterSide)
    {
        if (fighterSide == MatchFighterSide.Player)
        {
            return PlayerView;
        }

        if (fighterSide == MatchFighterSide.Opponent)
        {
            return OpponentView;
        }

        return null;
    }
}
