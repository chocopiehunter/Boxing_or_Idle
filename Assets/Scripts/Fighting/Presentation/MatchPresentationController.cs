using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MatchPresentationController : MonoBehaviour
{
    [SerializeField] private MatchFighterView PlayerView;
    [SerializeField] private MatchFighterView OpponentView;
    [SerializeField] private float StrikeImpactDelay = 0.12f;

    private int _presentationVersion;

    private void OnEnable()
    {
        _presentationVersion = _presentationVersion + 1;

        RefreshFighterDirection();
        BindCombatActionEvent();
    }

    private void OnDisable()
    {
        _presentationVersion = _presentationVersion + 1;

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
        MatchManager.Instance.OnMatchStepResolved -= HandleMatchStepResolved;

        MatchManager.Instance.OnCombatActionResolved += HandleCombatActionResolved;
        MatchManager.Instance.OnMatchStepResolved += HandleMatchStepResolved;
    }

    private void UnbindCombatActionEvent()
    {
        if (MatchManager.Instance == null)
        {
            return;
        }

        MatchManager.Instance.OnCombatActionResolved -= HandleCombatActionResolved;
        MatchManager.Instance.OnMatchStepResolved -= HandleMatchStepResolved;
    }

    private void HandleMatchStepResolved(MatchStepResult stepResult)
    {
        if (stepResult == null || PlayerView == null || OpponentView == null)
        {
            return;
        }

        MatchFighterDirection playerDirection = CalculateDirection(stepResult.PlayerPosition, stepResult.OpponentPosition);
        MatchFighterDirection opponentDirection = CalculateDirection(stepResult.OpponentPosition, stepResult.PlayerPosition);

        PlayerView.Setup(playerDirection);
        OpponentView.Setup(opponentDirection);

        PlayerView.MoveTo(stepResult.PlayerPosition);
        OpponentView.MoveTo(stepResult.OpponentPosition);
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

        CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();

        PlayHitAfterDelayAsync(targetView, _presentationVersion, cancellationToken).Forget();
    }

    private async UniTask PlayHitAfterDelayAsync(MatchFighterView targetView, int presentationVersion, CancellationToken cancellationToken)
    {
        float passedSeconds = 0f;

        while (passedSeconds < StrikeImpactDelay)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

            passedSeconds = passedSeconds + Time.unscaledDeltaTime;
        }

        if (presentationVersion != _presentationVersion || isActiveAndEnabled == false)
        {
            return;
        }

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
