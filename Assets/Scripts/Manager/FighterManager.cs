using System.Collections.Generic;
using UnityEngine;

public class FighterManager : MonoBehaviour
{
    public static FighterManager Instance { get; private set; }

    private const float RestTrainingHpMin = 0f;
    private const string RestTrainingType = "Rest";

    [SerializeField] private PlayerFighter Prefab_PlayerFighter;

    public List<FighterModel> PlayerFighters { get; private set; } = new List<FighterModel>();

    private List<PlayerFighter> _playerFighters = new List<PlayerFighter>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.GameState.CurrentState != GameFlowState.Play)
        {
            return;
        }

        if (SeasonManager.Instance == null)
        {
            return;
        }

        float seconds = UnityEngine.Time.deltaTime * SeasonManager.Instance.GetCurrentSpeedMultiplier();

        for (int i = 0; i < PlayerFighters.Count; i++)
        {
            ProgressTraining(PlayerFighters[i], seconds);
        }
    }

    public void CreateStartingRoster()
    {
        ClearRoster();

        FighterData data = GameDataManager.Instance.GetFighterData("fighter_01");
        if (data == null)
        {
            Debug.LogError("fighter_01 데이터 로드 실패");
            return;
        }

        FighterModel fighter = new FighterModel(data, "training_00");
        PlayerFighters.Add(fighter);

        SpawnPlayerFighter(fighter);

        Debug.Log($"시작 선수 생성 {fighter.Name}, 훈련방침: {fighter.CurrentTrainingId}");
    }

    public void ClearRoster()
    {
        for (int i = 0; i < _playerFighters.Count; i++)
        {
            if (_playerFighters[i] != null)
            {
                Destroy(_playerFighters[i].gameObject);
            }
        }

        _playerFighters.Clear();
        PlayerFighters.Clear();
    }

    private void SpawnPlayerFighter(FighterModel fighter)
    {
        if (Prefab_PlayerFighter == null)
        {
            Debug.LogError("PlayerFighter 프리팹이 연결되지 않음");
            return;
        }

        PlayerFighter player = Instantiate(Prefab_PlayerFighter);
        player.transform.position = new Vector3(0f, 0f, 0f);
        player.Bind(fighter);
        _playerFighters.Add(player);
    }

    public FighterModel GetFirstFighter()
    {
        if (PlayerFighters.Count == 0)
        {
            return null;
        }

        return PlayerFighters[0];
    }

    private void ProgressTraining(FighterModel fighter, float seconds)
    {
        if (fighter == null)
        {
            return;
        }

        string trainingId = GetActiveTrainingId(fighter);
        string previousId = fighter.ActiveTrainingId;

        fighter.ActiveTrainingId = trainingId;

        TrainingData trainingData = GameDataManager.Instance.GetTrainingData(trainingId);
        if (trainingData == null)
        {
            Debug.LogError($"TrainingData 없음 {trainingId}");
            return;
        }

        if (trainingData.TrainingType == RestTrainingType)
        {
            fighter.ActivityState = FighterActivityState.Resting;
        }
        else
        {
            fighter.ActivityState = FighterActivityState.Training;
        }

        if (fighter.ActivityState != FighterActivityState.Training && fighter.ActivityState != FighterActivityState.Resting)
        {
            return;
        }

        if (previousId != trainingId)
        {
            TrainingData previousData = GameDataManager.Instance.GetTrainingData(previousId);
            if (previousData != null && previousData.TrainingType == RestTrainingType)
            {
                fighter.ResetTrainingProgress(previousId);
            }
        }

        fighter.ApplyTrainingHpChange(trainingData.TrainingHpPerSecond * seconds);


        bool completed = fighter.AddTrainingProgress(trainingId, seconds, trainingData.Time);
        if (completed == false)
        {
            return;
        }

        ApplyTraining(fighter, trainingData);
    }

    private string GetActiveTrainingId(FighterModel fighter)
    {
        string restId = GetRestTrainingId();

        if (fighter.TrainingHp <= RestTrainingHpMin)
        {
            if (string.IsNullOrEmpty(restId) == false)
            {
                return restId;
            }
        }

        bool isResting = fighter.ActiveTrainingId == restId;
        if (isResting == true && fighter.IsTrainingHpFull() == false)
        {
            if (string.IsNullOrEmpty (restId) == false)
            {
                return restId;
            }
        }

        return fighter.CurrentTrainingId;
    }

    private string GetRestTrainingId()
    {
        Dictionary<string, TrainingData> list = GameDataManager.Instance.TrainingDataList;
        if (list == null)
        {
            return null;
        }

        foreach (KeyValuePair<string, TrainingData> pair in list)
        {
            if (pair.Value.TrainingType == RestTrainingType)
            {
                return pair.Value.Id;
            }
        }
        
        return null;
    }

    private void ApplyTraining(FighterModel fighter, TrainingData trainingData)
    {
        if (fighter == null || trainingData == null)
        {
            return;
        }

        float hpPlus = trainingData.HpUp - trainingData.HpDown;
        float atkPlus = trainingData.AtkUp - trainingData.AtkDown;
        float defPlus = trainingData.DefUp - trainingData.DefDown;
        float conditionPlus = trainingData.ConditionUp - trainingData.ConditionDown;

        fighter.Hp = fighter.Hp + hpPlus;
        fighter.Atk = fighter.Atk + atkPlus;
        fighter.Def = fighter.Def + defPlus;
        fighter.Condition = fighter.Condition + conditionPlus;

        if (fighter.Condition < 0f)
        {
            fighter.Condition = 0f;
        }

        Debug.Log($"{fighter.Name} 훈련 완료 {trainingData.Name} : Hp {fighter.Hp} / Atk {fighter.Atk} / Def {fighter.Def}");
    }
}
