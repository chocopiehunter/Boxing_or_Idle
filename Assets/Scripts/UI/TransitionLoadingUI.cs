using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TransitionLoadingUI : UIBase
{
    [SerializeField] private float MinimumDisplaySeconds = 2f;

    public async UniTask WaitForSeconds()
    {
        if (MinimumDisplaySeconds <= 0f)
        {
            return;
        }

        CancellationToken cancellationToken = this.GetCancellationTokenOnDestroy();
        float passedSeconds = 0f;

        while (passedSeconds < MinimumDisplaySeconds)
        {
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
            passedSeconds = passedSeconds + Time.unscaledDeltaTime;
        }
    }
}
