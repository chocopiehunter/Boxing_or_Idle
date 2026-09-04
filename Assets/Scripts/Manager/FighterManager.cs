using System.Collections.Generic;
using UnityEngine;

public class FighterManager : MonoBehaviour
{
    public static FighterManager Instance { get; private set; }

    private const float RestTrainingStaminaMin = 0f;
    private const string RestTrainingType = "Rest";
    private const float ArriveDistance = 0.15f;

    [SerializeField] private PlayerFighter Prefab_PlayerFighter;
    [SerializeField] private Transform Transform_GymFighterRoot;
    [SerializeField] private string StartingTrainingPolicyId = "policy_rest";

    public List<FighterModel> PlayerFighters { get; private set; } = new List<FighterModel>();

    private List<PlayerFighter> _playerFighters = new List<PlayerFighter>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (TrainingSpotManager.Instance == null)
        {
            Debug.LogError("TrainingSpotManager가 없음");
            return;
        }

        TrainingSpotManager.Instance.OnTrainingSpotsChanged += HandleTrainingSpotChanged;
    }

    private void OnDestroy()
    {
        if (TrainingSpotManager.Instance == null)
        {
            return;
        }

        TrainingSpotManager.Instance.OnTrainingSpotsChanged -= HandleTrainingSpotChanged;
    }

    private void HandleTrainingSpotChanged()
    {
        for (int i = 0; i < PlayerFighters.Count; i++)
        {
            FighterModel fighter = PlayerFighters[i];
            if (fighter == null)
            {
                continue;
            }

            fighter.ActiveSpot = null;
            fighter.IsAttractionChanged = true;
        }
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

        TrainingPolicyData policyData = GameDataManager.Instance.GetTrainingPolicyData(StartingTrainingPolicyId);

        if (policyData == null)
        {
            Debug.LogError($"스타팅 훈련 정책 데이터 없음 {StartingTrainingPolicyId}");
            return;
        }

        List<string> startingSkillIds = GameDataManager.Instance.GetStartingSkillIds(data);

        FighterModel fighter = new FighterModel(data, policyData.Id, startingSkillIds);
        PlayerFighters.Add(fighter);

        SpawnPlayerFighter(fighter);

        Debug.Log($"시작 선수 생성 {fighter.Name}, 훈련 정책: {fighter.CurrentTrainingPolicyId}, 보유 기술: {fighter.OwnedSkillIds.Count}개");
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

        if (Transform_GymFighterRoot == null)
        {
            Debug.LogError("체육관 선수 생성 Root가 연결되지 않음");
            return;
        }

        PlayerFighter player = Instantiate(Prefab_PlayerFighter, Transform_GymFighterRoot);
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

        float trainingStaminaBefore = fighter.TrainingStamina;
        string previousId = fighter.ActiveTrainingId;

        if (fighter.IsAttractionChanged == true)
        {
            ITrainingSpot selectedSpot = SelectTrainingSpot(fighter);
            fighter.ActiveSpot = selectedSpot;
            fighter.IsAttractionChanged = false;

            if (selectedSpot != null)
            {
                fighter.ActiveTrainingId = selectedSpot.TrainingDataId;
            }
            else
            {
                fighter.ActiveTrainingId = null;
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

        ITrainingSpot spot = fighter.ActiveSpot;
        if (spot == null)
        {
            fighter.ActivityState = FighterActivityState.Idle;
            return;
        }

        TrainingData trainingData = GameDataManager.Instance.GetTrainingData(trainingId);
        if (trainingData == null)
        {
            Debug.LogError($"TrainingData 없음 {trainingId}");
            return;
        }

        Vector3 targetPos = spot.GetTargetSpot().position;
        float distance = Vector3.Distance(playerView.transform.position, targetPos);

        if (distance > ArriveDistance)
        {
            fighter.ActivityState = FighterActivityState.Moving;
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

        float staminaPerSecond = GetTrainingStaminaPerSecond(trainingId);
        fighter.ApplyTrainingStaminaChange(staminaPerSecond * seconds);

        if (trainingStaminaBefore > RestTrainingStaminaMin && fighter.TrainingStamina <= RestTrainingStaminaMin)
        {
            fighter.IsAttractionChanged = true;
        }

        if (fighter.ActivityState == FighterActivityState.Resting && fighter.IsTrainingStaminaFull() == true)
        {
            fighter.IsAttractionChanged = true;
        }

        bool completed = fighter.AddTrainingProgress(trainingId, seconds, trainingData.Time);
        if (completed == false)
        {
            return;
        }

        ApplyTraining(fighter, trainingData, trainingId);
        fighter.IsAttractionChanged = true;
    }

    private float GetTrainingStaminaPerSecond(string trainingId)
    {
        TrainingFacilityData facilityData = GameDataManager.Instance.GetTrainingFacilityDataByTrainingId(trainingId);
        if (facilityData == null)
        {
            Debug.LogError($"TrainingFacilityData 없음. TrainingDataId : {trainingId}");
            return 0f;
        }

        return facilityData.TrainingStaminaPerSecond;
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

    private IReadOnlyList<ITrainingSpot> GetTrainingSpots()
    {
        if (TrainingSpotManager.Instance == null)
        {
            return null;
        }

        return TrainingSpotManager.Instance.TrainingSpots;
    }

    private ITrainingSpot FindSpotByTrainingId(string trainingId)
    {
        if (string.IsNullOrEmpty(trainingId) == true)
        {
            return null;
        }

        IReadOnlyList<ITrainingSpot> spots = GetTrainingSpots();
        if (spots == null)
        {
            return null;
        }

        for (int i = 0; i < spots.Count; i++)
        {
            ITrainingSpot spot = spots[i];
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

    private string GetRestTrainingId()
    {
        IReadOnlyList<ITrainingSpot> spots = GetTrainingSpots();
        if (spots == null)
        {
            return null;
        }

        for (int i = 0; i < spots.Count; i++)
        {
            ITrainingSpot spot = spots[i];
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

    private void ApplyTraining(FighterModel fighter, TrainingData trainingData, string trainingId)
    {
        if (fighter == null || trainingData == null)
        {
            return;
        }
        
        TrainingFacilityData facilityData = GameDataManager.Instance.GetTrainingFacilityDataByTrainingId(trainingId);
        if (facilityData == null)
        {
            Debug.LogError($"TrainingFacilityData 없음 {trainingId}");
            return;
        }

        TrainingStatValue finalValue = TrainingResultCalculator.Calculate(fighter, trainingData, facilityData);
        if (finalValue == null)
        {
            Debug.LogError($"훈련 최종 상승치 계산 실패 {trainingId}");
            return;
        }

        fighter.ApplyTrainingStatValue(finalValue);

        if (trainingData.TrainingType != RestTrainingType)
        {
            fighter.LastCompletedTrainingId = trainingId;
        }

        Debug.Log($"{fighter.Name} 훈련 완료 {trainingData.Name} : Hp {fighter.Hp} / Stamina {fighter.Stamina} / StandingOffense {fighter.StandingOffense} / StandingDefense {fighter.StandingDefense}");
    }

    private ITrainingSpot SelectTrainingSpot(FighterModel fighter)
    {
        if (fighter == null)
        {
            return null;
        }

        if (ShouldForceRest(fighter) == true)
        {
            string restTrainingId = GetRestTrainingId();
            return FindSpotByTrainingId(restTrainingId);
        }

        return SelectSpotByAttractionScore(fighter);
    }

    private bool ShouldForceRest(FighterModel fighter)
    {
        if (fighter.TrainingStamina <= RestTrainingStaminaMin)
        {
            return true;
        }

        if (fighter.ActivityState != FighterActivityState.Resting)
        {
            return false;
        }

        return fighter.IsTrainingStaminaFull() == false;
    }

    private ITrainingSpot SelectSpotByAttractionScore(FighterModel fighter)
    {
        IReadOnlyList<ITrainingSpot> spots = GetTrainingSpots();
        if (spots == null)
        {
            return null;
        }

        float totalAttractionScore = 0f;

        for (int i = 0; i < spots.Count; i++)
        {
            ITrainingSpot spot = spots[i];
            if(spot == null || spot.IsUnlocked == false)
            {
                continue;
            }

            float attractionScore = spot.GetAttractionScore(fighter);

            if(attractionScore <= 0f)
            {
                continue;
            }

            totalAttractionScore = totalAttractionScore + attractionScore;
        }

        if (totalAttractionScore <= 0f)
        {
            return null;
        }

        float randomScore = Random.Range(0f, totalAttractionScore);

        float currentScoreSum = 0f;
        ITrainingSpot lastSpot = null;

        for (int i = 0; i < spots.Count; i++)
        {
            ITrainingSpot spot = spots[i];
            if(spot == null || spot.IsUnlocked == false)
            {
                continue;
            }

            float attractionScore = spot.GetAttractionScore(fighter);

            if (attractionScore <= 0f)
            {
                continue;
            }

            lastSpot = spot;
            currentScoreSum = currentScoreSum + attractionScore;

            if (randomScore <= currentScoreSum)
            {
                return spot;
            }
        }

        return lastSpot;
    }
}
