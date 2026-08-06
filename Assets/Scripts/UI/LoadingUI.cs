using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : UIBase
{
    [SerializeField] private RawImage RawImage_LoadingImg;
    [SerializeField] private Slider Slider_LoadingBar;

    private const string Address_LoadingImg = "Texture_LoadingImg";

    // 로딩연출
    [Serializable] 
    private class LoadingStage
    {
        public float DelaySeconds;
        [Range(0f, 1f)] public float TargetProgress;
    }

    [SerializeField]
    private LoadingStage[] LoadingStages = new LoadingStage[]
    {
        new LoadingStage{DelaySeconds = 0.5f, TargetProgress = 0.3f},
        new LoadingStage{DelaySeconds = 0.3f, TargetProgress = 0.5f},
        new LoadingStage{DelaySeconds = 0.3f, TargetProgress = 0.7f},
        new LoadingStage{DelaySeconds = 0.4f, TargetProgress = 1f}
    };

    private void OnEnable()
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        LoadAndSetLoadingImgAsync(token).Forget();
    }

    private async UniTaskVoid LoadAndSetLoadingImgAsync(CancellationToken token)
    {
        Texture texture = await ResourceManager.Instance.LoadAsset<Texture>(Address_LoadingImg);

        if (texture != null && token.IsCancellationRequested == false)
        {
            RawImage_LoadingImg.texture = texture;
        }
    }

    public async UniTask PlayLoadingBarAsync()
    {
        CancellationToken token = this.GetCancellationTokenOnDestroy();
        float currentValue = 0f;
        Slider_LoadingBar.value = currentValue;

        foreach (var stage in LoadingStages)
        {
            float startValue = currentValue;
            float timePassed = 0f;

            while(timePassed < stage.DelaySeconds)
            {
                timePassed += Time.deltaTime;
                float progressRatio = Mathf.Clamp01(timePassed / stage.DelaySeconds);
                Slider_LoadingBar.value = Mathf.Lerp(startValue, stage.TargetProgress, progressRatio);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            currentValue = stage.TargetProgress;
            Slider_LoadingBar.value = currentValue;
        }
    }
}
