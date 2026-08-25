using UnityEngine;
using UnityEngine.UI;

public class PlayerFighter : MonoBehaviour
{
    private const string RestTrainingType = "Rest";

    [SerializeField] private Slider Slider_TrainingProgress;
    [SerializeField] private Image Image_Fill;
    [SerializeField] private Color Color_Training = Color.red;
    [SerializeField] private Color Color_Rest = Color.green;
    [SerializeField] private float MoveSpeed = 3f;

    public FighterModel Model { get; private set; }

    public void Bind(FighterModel model)
    {
        Model = model;
        RefreshTrainingProgress();
    }

    private void Update()
    {
        RefreshTrainingProgress();
        UpdateMove();
    }

    private void UpdateMove()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.GameState.CurrentState != GameFlowState.Play)
        {
            return;
        }

        if (Model == null)
        {
            return;
        }

        if (Model.ActivityState != FighterActivityState.Moving)
        {
            return;
        }

        if (Model.ActiveSpot == null)
        {
            return;
        }

        Transform target = Model.ActiveSpot.GetTargetSpot();
        if (target == null)
        {
            return;
        }

        float speed = MoveSpeed;
        if (SeasonManager.Instance != null)
        {
            speed = MoveSpeed * SeasonManager.Instance.GetCurrentSpeedMultiplier();
        }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * UnityEngine.Time.deltaTime);
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
        if (trainingData == null)
        {
            Slider_TrainingProgress.value = 0f;
            return;
        }

        Slider_TrainingProgress.minValue = 0f;
        Slider_TrainingProgress.maxValue = 1f;

        if (trainingData.TrainingType == RestTrainingType)
        {
            if (Model.TrainingStaminaMax <= 0f)
            {
                Slider_TrainingProgress.value = 0f;
            }
            else
            {
                Slider_TrainingProgress.value = Model.TrainingStamina / Model.TrainingStaminaMax;
            }
        }
        else
        {
            if (trainingData.Time <= 0f)
            {
                Slider_TrainingProgress.value = 0f;
            }
            else
            {
                float progress = Model.GetTrainingProgress(Model.ActiveTrainingId);
                Slider_TrainingProgress.value = progress / trainingData.Time;
            }
        }

        ChangeSliderBarColor(trainingData);
    }

    private void ChangeSliderBarColor(TrainingData trainingData)
    {
        if (Image_Fill == null)
        {
            return;
        }

        if (trainingData.TrainingType == RestTrainingType)
        {
            Image_Fill.color = Color_Rest;
        }
        else
        {
            Image_Fill.color = Color_Training;
        }
    }
}
