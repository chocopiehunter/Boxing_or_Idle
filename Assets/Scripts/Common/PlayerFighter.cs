using UnityEngine;
using UnityEngine.UI;

public class PlayerFighter : MonoBehaviour
{
    [SerializeField] private Slider Slider_TrainingProgress;
    public FighterModel Model { get; private set; }

    public void Bind(FighterModel model)
    {
        Model = model;
        RefreshTrainingProgress();
    }

    private void Update()
    {
        RefreshTrainingProgress();
    }

    private void RefreshTrainingProgress()
    {
        if (Slider_TrainingProgress == null)
        {
            return;
        }

        if (Model == null)
        {
            Slider_TrainingProgress.value = 0;
            return;
        }

        if (string.IsNullOrEmpty(Model.ActiveTrainingId) == true)
        {
            Slider_TrainingProgress.value = 0f;
            return;
        }

        TrainingData trainingData = GameDataManager.Instance.GetTrainingData(Model.ActiveTrainingId);
        if (trainingData == null || trainingData.Time <= 0f)
        {
            Slider_TrainingProgress.value = 0f;
            return;
        }

        float progress = Model.GetTrainingProgress(Model.ActiveTrainingId);
        Slider_TrainingProgress.minValue = 0f;
        Slider_TrainingProgress.maxValue = 1f;
        Slider_TrainingProgress.value = progress / trainingData.Time;
    }

    // 나중에 HUD넣을 곳
}
