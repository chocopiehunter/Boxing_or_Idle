using System.Collections.Generic;
using UnityEngine;

public class FighterManager : MonoBehaviour
{
    public static FighterManager Instance { get; private set; }

    [SerializeField] private PlayerFighter Prefab_PlayerFighter;

    public List<FighterModel> PlayerFighters { get; private set; } = new List<FighterModel>();

    private List<PlayerFighter> _playerFighters = new List<PlayerFighter>();

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

    private void OnMonthAdvanced()
    {
        ApplyTrainingToFighter();
    }

    private void ApplyTraining(FighterModel fighter)
    {
        if (fighter == null)
        {
            return;
        }

        TrainingData trainingData = GameDataManager.Instance.GetTrainingData(fighter.CurrentTrainingId);
        if(trainingData == null)
        {
            Debug.LogError($"TrainingData 없음 {fighter.CurrentTrainingId}");
            return;
        }

        fighter.Hp = fighter.Hp + trainingData.HpUp - trainingData.HpDown;
        fighter.Atk = fighter.Atk + trainingData.AtkUp - trainingData.AtkDown;
        fighter.Def = fighter.Def + trainingData.DefUp - trainingData.DefDown;
        fighter.Condition = fighter.Condition + trainingData.ConditionUp - trainingData.ConditionDown;

        if (fighter.Condition < 0f)
        {
            fighter.Condition = 0f;
        }
    }

    private void ApplyTrainingToFighter()
    {
        for (int i = 0; i < PlayerFighters.Count; i++)
        {
            ApplyTraining(PlayerFighters[i]);
        }
    }
}
