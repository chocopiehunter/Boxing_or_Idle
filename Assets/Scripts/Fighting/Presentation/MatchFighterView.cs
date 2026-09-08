using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MatchFighterView : MonoBehaviour
{
    private static readonly int IdleStateHash = Animator.StringToHash("Idle");
    private static readonly int JabStateHash = Animator.StringToHash("Jab");

    [SerializeField] private Animator Anim_Fighter;
    [SerializeField] private SpriteRenderer SpriteRenderer_Fighter;
    [SerializeField] private float JabDuration = 0.25f;

    private int _actionVersion;

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

        Anim_Fighter.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void Setup(bool isFacingRight)
    {
        _actionVersion = _actionVersion + 1;

        if (Anim_Fighter == null || SpriteRenderer_Fighter == null)
        {
            return;
        }

        SpriteRenderer_Fighter.flipX = isFacingRight == false;

        PlayIdle();
    }

    public void PlayJab()
    {
        if (Anim_Fighter == null)
        {
            return;
        }

        _actionVersion = _actionVersion + 1;

        CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();

        PlayJabAsync(_actionVersion, cancellationToken).Forget();
    }

    private async UniTask PlayJabAsync(int actionVersion, CancellationToken cancellationToken)
    {
        Anim_Fighter.Play(JabStateHash, 0, 0f);

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

        PlayIdle();
    }

    private void PlayIdle()
    {
        if (Anim_Fighter == null)
        {
            return;
        }

        Anim_Fighter.Play(IdleStateHash, 0, 0f);
    }

    private void OnDisable()
    {
        _actionVersion = _actionVersion + 1;
    }
}
