using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MatchFighterView : MonoBehaviour
{
    private static readonly int IdleEastHash = Animator.StringToHash("Idle_East");
    private static readonly int IdleWestHash = Animator.StringToHash("Idle_West");
    private static readonly int JabEastHash = Animator.StringToHash("Jab_East");
    private static readonly int JabWestHash = Animator.StringToHash("Jab_West");
    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int StepEastHash = Animator.StringToHash("Step_East");
    private static readonly int StepWestHash = Animator.StringToHash("Step_West");
    private static readonly int BackStepEastHash = Animator.StringToHash("BackStep_East");
    private static readonly int BackStepWestHash = Animator.StringToHash("BackStep_West");
    private static readonly int HighKickEastHash = Animator.StringToHash("Kick_High_East");
    private static readonly int HighKickWestHash = Animator.StringToHash("Kick_High_West");

    [SerializeField] private Animator Anim_Fighter;
    [SerializeField] private SpriteRenderer SpriteRenderer_Fighter;
    [SerializeField] private float JabDuration = 0.25f;
    [SerializeField] private float HighKickDuration = 0.85f;
    [SerializeField] private float HitDuration = 0.52f;
    [SerializeField] private float StepMoveDuration = 0.35f;

    public MatchFighterDirection CurrentDirection { get; private set; } = MatchFighterDirection.None;
    private int _actionVersion;
    private int _moveVersion;
    private bool _isActionPlaying;
    private bool _isMoving;
    private MatchStepType _currentStepType = MatchStepType.None;

    private void Awake()
    {
        if (Anim_Fighter == null)
        {
            Debug.LogError($"{name} 선수에 Animator가 연결되지 않음");
            return;
        }

        if (SpriteRenderer_Fighter == null)
        {
            Debug.LogError($"{name} 선수에 SpriteRenderer가 연결되지 않음");
            return;
        }

        if (StepMoveDuration <= 0f)
        {
            Debug.LogError($"{name} 선수의 Step 이동 시간이 0 이하임");
        }

        Anim_Fighter.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void Setup(MatchFighterDirection direction)
    {
        if (Anim_Fighter == null)
        {
            return;
        }

        if (direction == MatchFighterDirection.None)
        {
            Debug.LogError($"{name} 선수의 방향이 지정되지 않음");
            return;
        }

        CurrentDirection = direction;

        if (_isActionPlaying == false && _isMoving == false)
        {
            PlayIdle();
        }
    }

    public void PlayJab()
    {
        if (Anim_Fighter == null)
        {
            return;
        }

        _isActionPlaying = true;
        _actionVersion = _actionVersion + 1;

        CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();

        PlayJabAsync(_actionVersion, cancellationToken).Forget();
    }

    private async UniTask PlayJabAsync(int actionVersion, CancellationToken cancellationToken)
    {
        PlayDirectionalJab();

        float passedSeconds = 0f;

        while (passedSeconds < JabDuration)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

            passedSeconds = passedSeconds + Time.unscaledDeltaTime;
        }

        if (actionVersion != _actionVersion)
        {
            return;
        }

        _isActionPlaying = false;
        PlayStepOrIdle();
    }

    private void PlayDirectionalJab()
    {
        if (Anim_Fighter == null || SpriteRenderer_Fighter == null)
        {
            return;
        }

        if (CurrentDirection == MatchFighterDirection.West)
        {
            SpriteRenderer_Fighter.flipX = true;
            Anim_Fighter.Play(JabWestHash, 0, 0f);
            return;
        }

        SpriteRenderer_Fighter.flipX = false;
        Anim_Fighter.Play(JabEastHash, 0, 0f);
    }

    public void PlayHighKick()
    {
        if (Anim_Fighter == null)
        {
            return;
        }

        _isActionPlaying = true;
        _actionVersion = _actionVersion + 1;

        CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();

        PlayHighKickAsync(_actionVersion, cancellationToken).Forget();
    }

    private async UniTask PlayHighKickAsync(int actionVersion, CancellationToken cancellationToken)
    {
        PlayDirectionalHighKick();

        float passedSeconds = 0f;

        while (passedSeconds < HighKickDuration)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

            passedSeconds = passedSeconds + Time.unscaledDeltaTime;
        }

        if (actionVersion != _actionVersion)
        {
            return;
        }

        _isActionPlaying = false;
        PlayStepOrIdle();
    }

    private void PlayDirectionalHighKick()
    {
        if (Anim_Fighter == null || SpriteRenderer_Fighter == null)
        {
            return;
        }

        SpriteRenderer_Fighter.flipX = false;

        if (CurrentDirection == MatchFighterDirection.West)
        {
            Anim_Fighter.Play(HighKickWestHash, 0, 0f);
            return;
        }

        Anim_Fighter.Play(HighKickEastHash, 0, 0f);
    }

    public void PlayHit()
    {
        if (Anim_Fighter == null)
        {
            return;
        }

        _isActionPlaying = true;
        _actionVersion = _actionVersion + 1;

        CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();

        PlayHitAsync(_actionVersion, cancellationToken).Forget();
    }

    private async UniTask PlayHitAsync(int actionVersion, CancellationToken cancellationToken)
    {
        SpriteRenderer_Fighter.flipX = false;
        Anim_Fighter.Play(HitHash, 0, 0f);

        float passedSeconds = 0f;

        while (passedSeconds < HitDuration)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

            passedSeconds = passedSeconds + Time.unscaledDeltaTime;
        }

        if (actionVersion != _actionVersion)
        {
            return;
        }

        _isActionPlaying = false;
        PlayStepOrIdle();
    }

    public void MoveTo(Vector2 targetLocalPosition, MatchStepType stepType)
    {
        Vector2 currentLocalPosition = transform.localPosition;

        if ((targetLocalPosition - currentLocalPosition).sqrMagnitude <= 0.000001f)
        {
            return;
        }

        _currentStepType = stepType;
        _moveVersion = _moveVersion + 1;

        CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();

        MoveToAsync(targetLocalPosition, _moveVersion, cancellationToken).Forget();
    }

    public void SetPositionImmediately(Vector2 targetLocalPosition)
    {
        _actionVersion = _actionVersion + 1;
        _moveVersion = _moveVersion + 1;
        _isActionPlaying = false;
        _isMoving = false;
        _currentStepType = MatchStepType.None;

        Vector3 currentPosition = transform.localPosition;
        transform.localPosition = new Vector3(targetLocalPosition.x, targetLocalPosition.y, currentPosition.z);

        PlayIdle();
    }

    private async UniTask MoveToAsync(Vector2 targetLocalPosition, int moveVersion, CancellationToken cancellationToken)
    {
        Vector3 startPosition = transform.localPosition;
        Vector3 targetPosition = new Vector3(targetLocalPosition.x, targetLocalPosition.y, startPosition.z);

        if (StepMoveDuration <= 0f)
        {
            transform.localPosition = targetPosition;
            _currentStepType = MatchStepType.None;
            return;
        }

        _isMoving = true;
        PlayStepOrIdle();

        float passedSeconds = 0f;

        while (passedSeconds < StepMoveDuration)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);

            if (moveVersion != _moveVersion)
            {
                return;
            }

            passedSeconds = passedSeconds + Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(passedSeconds / StepMoveDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, easedProgress);
        }

        transform.localPosition = targetPosition;
        _isMoving = false;
        _currentStepType = MatchStepType.None;
        PlayStepOrIdle();
    }

    private void PlayStepOrIdle()
    {
        if (Anim_Fighter == null || _isActionPlaying)
        {
            return;
        }

        SpriteRenderer_Fighter.flipX = false;

        if (_isMoving == false || _currentStepType == MatchStepType.None)
        {
            PlayIdle();
            return;
        }

        if (CurrentDirection == MatchFighterDirection.East)
        {
            if (_currentStepType == MatchStepType.Back)
            {
                Anim_Fighter.Play(BackStepEastHash, 0, 1f);
                return;
            }

            Anim_Fighter.Play(StepEastHash, 0, 0f);
            return;
        }

        if (CurrentDirection == MatchFighterDirection.West)
        {
            if (_currentStepType == MatchStepType.Back)
            {
                Anim_Fighter.Play(BackStepWestHash, 0, 1f);
                return;
            }

            Anim_Fighter.Play(StepWestHash, 0, 0f);
            return;
        }

        PlayIdle();
    }

    private void PlayIdle()
    {
        if (Anim_Fighter == null || SpriteRenderer_Fighter == null)
        {
            return;
        }

        SpriteRenderer_Fighter.flipX = false;

        if (CurrentDirection == MatchFighterDirection.West)
        {
            Anim_Fighter.Play(IdleWestHash, 0, 0f);
            return;
        }

        Anim_Fighter.Play(IdleEastHash, 0, 0f);
    }

    private void OnDisable()
    {
        _actionVersion = _actionVersion + 1;
        _moveVersion = _moveVersion + 1;
        _isActionPlaying = false;
        _isMoving = false;
        _currentStepType = MatchStepType.None;
    }
}
