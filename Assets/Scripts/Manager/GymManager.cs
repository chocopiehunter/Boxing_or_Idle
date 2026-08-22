using System.Collections.Generic;
using UnityEngine;

public class GymManager : MonoBehaviour
{
    public static GymManager Instance { get; private set; }

    private const string NoneId = "None";
    private const int FirstLevel = 1;
    private const string DefaultBuildingType = "gym";

    [SerializeField] private string StartingLevelId = "gym_level_01";
    [SerializeField] private int StartingGold = 300;
    [SerializeField] private List<string> StartingFacilityIds = new List<string>();

    public GymModel CurrentGym {  get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void CreateStartingGym()
    {
        GymLevelData startData = GameDataManager.Instance.GetGymLevelData(StartingLevelId);

        if (startData == null)
        {
            Debug.LogError($"시작 체육관 데이터 없음. {StartingLevelId}");
            return;
        }

        if (TrainingSpotManager.Instance != null)
        {
            TrainingSpotManager.Instance.ClearAll();
        }

        CurrentGym = new GymModel(StartingGold);
        CurrentGym.ApplyLevelData(startData);

        CreateStartingFacility();

        Debug.Log($"시작 체육관 생성완료. Type: {startData.Type} / 레벨: {startData.Level} / 자금: {CurrentGym.Gold}");
    }

    private void CreateStartingFacility()
    {
        List<string> unlockedIds = GetUnlockedFacilityIds();

        for (int i = 0; i < StartingFacilityIds.Count; i++)
        {
            string facilityId = StartingFacilityIds[i];

            if (unlockedIds.Contains(facilityId) == false)
            {
                Debug.LogError($"시작 시설이 현재 체육관에서 해금되지 않음 {facilityId}");
                continue;
            }

            TrainingFacilityData facilityData = GameDataManager.Instance.GetTrainingFacilityData(facilityId);

            if (facilityData == null)
            {
                Debug.LogError($"시작 시설 데이터 없음 {facilityId}");
                continue;
            }

            CurrentGym.ApplyFacilityData(facilityData);

            if (TrainingSpotManager.Instance == null)
            {
                Debug.LogError("TrainingSpotManager가 없음");
                continue;
            }

            TrainingSpotManager.Instance.SpawnOrUpdate(facilityData);
        }
    }

    public List<string> GetUnlockedFacilityIds()
    {
        List<string> result = new List<string>();

        GymLevelData currentData = GetCurrentLevelData(DefaultBuildingType);

        GymLevelData levelData = GameDataManager.Instance.GetGymLevelDataByTypeAndLevel(DefaultBuildingType, FirstLevel);

        while (levelData != null)
        {
            AddFacilityIds(result, levelData.FacilityIds);

            if (currentData != null && levelData.Id == currentData.Id)
            {
                break;
            }

            if (string.IsNullOrEmpty(levelData.NextLevelId) == true)
            {
                break;
            }

            if (levelData.NextLevelId == NoneId)
            {
                break;
            }

            levelData = GameDataManager.Instance.GetGymLevelData(levelData.NextLevelId);
        }

        return result;
    }

    private void AddFacilityIds(List<string> result, string facilityIds)
    {
        if (string.IsNullOrEmpty (facilityIds) == true)
        {
            return;
        }

        if (facilityIds == NoneId)
        {
            return;
        }

        string[] splitIds = facilityIds.Split(',');

        for (int i = 0; i < splitIds.Length; i++)
        {
            string facilityId = splitIds[i].Trim();

            if (string.IsNullOrEmpty (facilityId) == true)
            {
                continue;
            }

            if (result.Contains (facilityId) == false)
            {
                result.Add (facilityId);
            }
        }
    }

    public void ClearGym()
    {
        if (TrainingSpotManager.Instance != null)
        {
            TrainingSpotManager.Instance.ClearAll();
        }

        CurrentGym = null;
    }

    public GymLevelData GetCurrentLevelData(string type)
    {
        if (CurrentGym == null)
        {
            return null;
        }

        string levelId = CurrentGym.GetLevelId(type);
        if(string.IsNullOrEmpty(levelId) == true)
        {
            return null;
        }

        return GameDataManager.Instance.GetGymLevelData(levelId);
    }

    public GymLevelData GetNextLevelData(string type)
    {
        GymLevelData currentData = GetCurrentLevelData(type);
        if(currentData == null)
        {
            return GameDataManager.Instance.GetGymLevelDataByTypeAndLevel(type, FirstLevel);
        }

        if (string.IsNullOrEmpty(currentData.NextLevelId) == true)
        {
            return null;
        }

        if (currentData.NextLevelId == NoneId)
        {
            return null;
        }

        return GameDataManager.Instance.GetGymLevelData(currentData.NextLevelId);
    }

    public bool TryUpgrade(string type)
    {
        if (CurrentGym == null)
        {
            Debug.LogError("체육관이 없음");
            return false;
        }

        GymLevelData nextLevelData = GetNextLevelData(type);
        if (nextLevelData == null)
        {
            Debug.Log($"{type} 최대 레벨 입니다");
            return false;
        }

        if (nextLevelData.RequiredUnlockIds != NoneId)
        {
            Debug.Log($"승급 조건 미구현 {nextLevelData.RequiredUnlockIds}");
            return false;
        }

        if (CurrentGym.TrySpendGold(nextLevelData.GoldCost) == false)
        {
            Debug.Log($"자금 부족. 필요 {nextLevelData.GoldCost} / 보유 {CurrentGym.Gold}");
            return false;
        }

        CurrentGym.ApplyLevelData(nextLevelData);
        Debug.Log($"{type} 업그레이드 레벨 {nextLevelData.Level} {nextLevelData.Name} / 남은 자금: {CurrentGym.Gold}");
        return true;
    }

    public bool TryUpgradeGym()
    {
        return TryUpgrade(DefaultBuildingType);
    }

    // 테스트 코드
    [ContextMenu("체육관 업그레이드 테스트")]
    private void DebugUpgrade()
    {
        TryUpgradeGym();
    }
}
