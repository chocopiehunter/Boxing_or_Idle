using System.Collections.Generic;
using UnityEngine;

public class FighterManager : MonoBehaviour
{
    public static FighterManager Instance { get; private set; }

    private const float RestTrainingHpMin = 0f;
    private const string RestTrainingType = "Rest";
    private const float ArriveDistance = 0.15f;

    [SerializeField] private PlayerFighter Prefab_PlayerFighter;
    [SerializeField] private TrainingSpot Prefab_Sandbag;
    [SerializeField] private TrainingSpot Prefab_Rest;
    [SerializeField] private Transform Transform_TrainingSpotRoot;
    [SerializeField] private Vector3 Position_Sandbag = new Vector3(-5f, 0f, 0f);
    [SerializeField] private Vector3 Position_Rest = new Vector3(5f, 0f, 0f);

    private List<TrainingSpot> _spawnedTrainingSpots = new List<TrainingSpot>();
    private List<ITrainingSpot> _trainingSpots = new List<ITrainingSpot>();

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

        SpawnTrainingSpots();
        RefreshTrainingSpotList();
    }

    private void SpawnTrainingSpots()
    {
        ClearTrainingSpots();

        if (Prefab_Sandbag != null)
        {
            TrainingSpot sandbag = Instantiate(Prefab_Sandbag);
            sandbag.transform.position = Position_Sandbag;
            if (Transform_TrainingSpotRoot != null)
            {
                sandbag.transform.SetParent(Transform_TrainingSpotRoot, false);
            }

            _spawnedTrainingSpots.Add(sandbag);
        }

        if (Prefab_Rest != null)
        {
            TrainingSpot rest = Instantiate(Prefab_Rest);
            rest.transform.position = Position_Rest;
            if (Transform_TrainingSpotRoot != null)
            {
                rest.transform.SetParent(Transform_TrainingSpotRoot);
            }

            _spawnedTrainingSpots.Add(rest);
        }

    }

    private void RefreshTrainingSpotList()
    {
        _trainingSpots.Clear();

        for (int i = 0; i < _spawnedTrainingSpots.Count; i++)
        {
            if (_spawnedTrainingSpots[i] == null)
            {
                continue;
            }

            _trainingSpots.Add(_spawnedTrainingSpots[i]);
        }
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
        ClearTrainingSpots();
    }

    public void ClearTrainingSpots()
    {
        for (int i = 0; i < _spawnedTrainingSpots.Count; i++)
        {
            if (_spawnedTrainingSpots[i] != null)
            {
                Destroy(_spawnedTrainingSpots[i].gameObject);
            }
        }
        _spawnedTrainingSpots.Clear();
        _trainingSpots.Clear();
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

    public void NotifyAttractionChanged(FighterModel fighter)
    {
        if (fighter == null)
        {
            return;
        }

        fighter.IsAttractionChanged = true;
    }

    private void ProgressTraining(FighterModel fighter, float seconds)
    {
        if (fighter == null)
        {
            return;
        }

        PlayerFighter playerView = FindPlayerView(fighter);
        if (playerView == null)
        {
            return;
        }

        float trainingHpBefore = fighter.TrainingHp;
        string previousId = fighter.ActiveTrainingId;

        if (fighter.IsAttractionChanged == true || fighter.ActiveSpot == null)
        {
            ITrainingSpot bestSpot = SelectBestSpot(fighter);
            fighter.ActiveSpot = bestSpot;
            fighter.IsAttractionChanged = false;

            if (bestSpot != null)
            {
                fighter.ActiveTrainingId = bestSpot.TrainingDataId;
            }
            else
            {
                fighter.ActiveTrainingId = fighter.CurrentTrainingId;
            }
        }

        string trainingId = fighter.ActiveTrainingId;

        if (previousId != trainingId)
        {
            TrainingData previousData = GameDataManager.Instance.GetTrainingData(previousId);
            if (previousData != null && previousData.TrainingType == RestTrainingType)
            {
                fighter.ResetTrainingProgress(previousId);
            }
        }

        TrainingData trainingData = GameDataManager.Instance.GetTrainingData(trainingId);
        if (trainingData == null)
        {
            Debug.LogError($"TrainingData 없음 {trainingId}");
            return;
        }

        ITrainingSpot spot = fighter.ActiveSpot;
        if (spot == null)
        {
            spot = FindSpotByTrainingId(trainingId);
            fighter.ActiveSpot = spot;
        }

        if (spot != null)
        {
            Vector3 targetPos = spot.GetTargetSpot().position;
            float distance = Vector3.Distance(playerView.transform.position, targetPos);

            if (distance > ArriveDistance)
            {
                fighter.ActivityState = FighterActivityState.Moving;
                return;
            }
        }

        if (trainingData.TrainingType == RestTrainingType)
        {
            fighter.ActivityState = FighterActivityState.Resting;
        }
        else
        {
            fighter.ActivityState = FighterActivityState.Training;
        }

        fighter.ApplyTrainingHpChange(trainingData.TrainingHpPerSecond * seconds);

        if (trainingHpBefore > RestTrainingHpMin && fighter.TrainingHp <= RestTrainingHpMin)
        {
            fighter.IsAttractionChanged = true;
        }

        if (fighter.ActivityState == FighterActivityState.Resting && fighter.IsTrainingHpFull() == true)
        {
            fighter.IsAttractionChanged = true;
        }

        bool completed = fighter.AddTrainingProgress(trainingId, seconds, trainingData.Time);
        if (completed == false)
        {
            return;
        }

        ApplyTraining(fighter, trainingData);
        fighter.IsAttractionChanged = true;
    }

    private PlayerFighter FindPlayerView(FighterModel fighter)
    {
        for (int i = 0; i < _playerFighters.Count; i++)
        {
            if (_playerFighters[i] == null)
            {
                continue;
            }

            if (_playerFighters[i].Model == fighter)
            {
                return _playerFighters[i];
            }
        }

        return null;
    }

    private ITrainingSpot FindSpotByTrainingId(string trainingId)
    {
        if (string.IsNullOrEmpty(trainingId) == true)
        {
            return null;
        }

        for (int i = 0; i < _trainingSpots.Count; i++)
        {
            ITrainingSpot spot = _trainingSpots[i];
            if (spot == null || spot.IsUnlocked == false)
            {
                continue;
            }

            if (spot.TrainingDataId == trainingId)
            {
                return spot;
            }
        }

        return null;
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
        for (int i = 0; i < _trainingSpots.Count; i++)
        {
            ITrainingSpot spot = _trainingSpots[i];
            if (spot == null || spot.IsUnlocked == false)
            {
                continue;
            }

            string trainingId = spot.TrainingDataId;
            TrainingData data = GameDataManager.Instance.GetTrainingData(trainingId);
            if (data == null)
            {
                continue;
            }

            if (data.TrainingType == RestTrainingType)
            {
                return trainingId;
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

    private ITrainingSpot SelectBestSpot(FighterModel fighter)
    {
        ITrainingSpot bestSpot = null;
        float bestScore = float.MinValue;

        for (int i = 0; i < _trainingSpots.Count; i++)
        {
            ITrainingSpot spot = _trainingSpots[i];
            if (spot == null || spot.IsUnlocked == false)
            {
                continue;
            }

            float score = spot.GetAttractionScore(fighter);
            if (score > bestScore)
            {
                bestScore = score;
                bestSpot = spot;
            }
        }

        return bestSpot;
    }
}
