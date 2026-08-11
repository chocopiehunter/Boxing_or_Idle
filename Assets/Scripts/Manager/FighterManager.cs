using System.Collections.Generic;
using UnityEngine;

public class FighterManager : MonoBehaviour
{
    public static FighterManager Instance { get; private set; }

    public List<FighterModel> PlayerFighters { get; private set; } = new List<FighterModel>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (SeasonManager.Instance != null)
        {
            SeasonManager.Instance.OnMonthAdvanced -= OnMonthAdvanced;
            SeasonManager.Instance.OnMonthAdvanced += OnMonthAdvanced;
        }
    }

    private void OnDestroy()
    {
        if(SeasonManager.Instance != null)
        {
            SeasonManager.Instance.OnMonthAdvanced -= OnMonthAdvanced;
        }
    }

    public void CreateStartingRoster()
    {
        PlayerFighters.Clear();

        FighterData data = GameDataManager.Instance.GetFighterData("fighter_01");
        if (data == null)
        {
            Debug.LogError("fighter_01 데이터 로드 실패");
            return;
        }

        FighterModel fighter = new FighterModel(data, "training_00");
        PlayerFighters.Add(fighter);

        Debug.Log($"시작 선수 생성 {fighter.Name}, 훈련방침: {fighter.CurrentTrainingId}");
    }

    public FighterModel GetFirstFighter()
    {
        if (PlayerFighters.Count == 0)
        {
            return null;
        }

        return PlayerFighters[0];
    }

    private void OnMonthAdvanced()
    {

    }

    private void ApplyTraining(FighterModel fighter)
    {
        if (fighter == null)
        {
            return;
        }


    }

    private void ApplyTrainingToFighter()
    {
        for (int i = 0; i < PlayerFighters.Count; i++)
        {

        }
    }
}
