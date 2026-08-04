using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
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
        PlayLoadingBarAsync(token).Forget();
    }

    private async UniTaskVoid LoadAndSetLoadingImgAsync(CancellationToken token)
    {
        Texture texture = await ResourceManager.Inst.LoadAsset<Texture>(Address_LoadingImg);

        if (texture != null && token.IsCancellationRequested == false)
        {
            RawImage_LoadingImg.texture = texture;
        }
    }

    private async UniTaskVoid PlayLoadingBarAsync(CancellationToken token)
    {
        Slider_LoadingBar.value = 0f;

        foreach (var stage in LoadingStages)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(stage.DelaySeconds), cancellationToken: token);
            Slider_LoadingBar.value = stage.TargetProgress;
        }
    }
}
