using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;

        GameUtil.LoadFullData();
    }

    [Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> items;
    }

    // 1. 등록
    public Dictionary<string, FighterData> FighterDataList { get; private set; } = new Dictionary<string, FighterData>();
    public Dictionary<string, TrainingData> TrainingDataList { get; private set; } = new Dictionary<string, TrainingData>();
    public Dictionary<string, OrganizationData> OrganizationDataList { get; private set; } = new Dictionary<string, OrganizationData>();
    public Dictionary<string, NgAchievementData> NgAchievementDataList { get; private set; } = new Dictionary<string, NgAchievementData>();
    public Dictionary<string, GymLevelData> GymLevelDataList { get; private set; } = new Dictionary<string, GymLevelData>();
    public Dictionary<string, UnlockConditionData> UnlockConditionDataList { get; private set; } = new Dictionary<string, UnlockConditionData>();
    public Dictionary<string, TrainingFacilityData> TrainingFacilityDataList { get; private set; } = new Dictionary<string, TrainingFacilityData>();
    public Dictionary<string, TrainingPolicyData> TrainingPolicyDataList { get; private set; } = new Dictionary<string, TrainingPolicyData>();
    public Dictionary<string, SkillData> SkillDataList { get; private set; } = new Dictionary<string, SkillData>();
    public Dictionary<string, SkillUseConditionData> SkillUseConditionDataList { get; private set; } = new Dictionary<string, SkillUseConditionData>();
    public Dictionary<string, MatchRuleData> MatchRuleDataList { get; private set; } = new Dictionary<string, MatchRuleData>();

    private Dictionary<string, T> LoadData<T>(string tableName) where T : GameDataBase
    {
        // 경로설정
        // Resources/JsonOutput 폴더
        string resourcePath = $"JsonOutput/{tableName}";

        // 리소스 로드
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

        // 파일 존재 여부 체크
        if (textAsset == null)
        {
            Debug.LogError($"[Error] 리소스를 찾을 수 없습니다: Resources/{resourcePath}");
            return new Dictionary<string, T>();
        }

        try
        {
            string jsonString = textAsset.text;

            // Wrapper 트릭 적용
            string wrappedJson = "{\"items\":" + jsonString + "}";
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

            if (wrapper != null && wrapper.items != null)
            {
                Debug.Log($"{typeof(T).Name} 데이터를 {wrapper.items.Count}개 로드했습니다.");
                // ToDictionary를 사용하려면 각 클래스(T)에 Id 필드가 있어야 합니다
                return wrapper.items.ToDictionary(item => item.Id.ToString());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{typeof(T).Name} JSON 로드 오류] {ex.Message}");
        }

        return new Dictionary<string, T>();
    }

    // 3. 로드
    public void LoadAll()
    {
        FighterDataList = LoadData<FighterData>("FighterData");
        TrainingDataList = LoadData<TrainingData>("TrainingData");
        OrganizationDataList = LoadData<OrganizationData>("OrganizationData");
        NgAchievementDataList = LoadData<NgAchievementData>("NgAchievementData");
        GymLevelDataList = LoadData<GymLevelData>("GymLevelData");
        UnlockConditionDataList = LoadData<UnlockConditionData>("UnlockConditionData");
        TrainingFacilityDataList = LoadData<TrainingFacilityData>("TrainingFacilityData");
        TrainingPolicyDataList = LoadData<TrainingPolicyData>("TrainingPolicyData");
        SkillDataList = LoadData<SkillData>("SkillData");
        SkillUseConditionDataList = LoadData<SkillUseConditionData>("SkillUseConditionData");
        MatchRuleDataList = LoadData<MatchRuleData>("MatchRuleData");
    }

    // 2. 사용을 위한 메서드 정의
    
    public FighterData GetFighterData(string Id)
    {
        if (FighterDataList == null || string.IsNullOrEmpty(Id)) return null;

        return FighterDataList.TryGetValue(Id, out var item) ? item : null;
    }

    public TrainingData GetTrainingData(string Id)
    {
        if (TrainingDataList == null || string.IsNullOrEmpty(Id)) return null;

        return TrainingDataList.TryGetValue(Id, out var item) ? item : null;
    }

    public OrganizationData GetOrganizationData(string Id)
    {
        if (OrganizationDataList == null || string.IsNullOrEmpty(Id)) return null;

        return OrganizationDataList.TryGetValue(Id, out var item) ? item : null;
    }

    public NgAchievementData GetNgAchievementData(string Id)
    {
        if (NgAchievementDataList == null || string.IsNullOrEmpty(Id)) return null;

        return NgAchievementDataList.TryGetValue(Id, out var item) ? item : null;
    }

    public GymLevelData GetGymLevelData(string Id)
    {
        if (GymLevelDataList == null || string.IsNullOrEmpty(Id)) return null;

        return GymLevelDataList.TryGetValue(Id,out var item) ? item : null;
    }

    public GymLevelData GetGymLevelDataByTypeAndLevel(string type, int level)
    {
        if (GymLevelDataList == null || string.IsNullOrEmpty(type)) return null;

        foreach (KeyValuePair<string, GymLevelData> pair in GymLevelDataList)
        {
            GymLevelData data = pair.Value;
            if (data == null)
            {
                continue;
            }

            if(data.Type == type)
            {
                if (data.Level == level)
                {
                    return data;
                }
            }
        }

        return null;
    }

    public UnlockConditionData GetUnlockConditionData(string Id)
    {
        if (UnlockConditionDataList == null || string.IsNullOrEmpty(Id)) return null;

        return UnlockConditionDataList.TryGetValue(Id, out var item) ? item : null;
    }

    public TrainingFacilityData GetTrainingFacilityData(string Id)
    {
        if (TrainingFacilityDataList == null || string.IsNullOrEmpty(Id)) return null;

        return TrainingFacilityDataList.TryGetValue(Id, out var item) ? item : null;
    }

    public TrainingFacilityData GetTrainingFacilityDataByTypeAndLevel(string type, int level)
    {
        if (TrainingFacilityDataList == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(type) == true)
        {
            return null;
        }

        foreach (KeyValuePair<string, TrainingFacilityData> pair in TrainingFacilityDataList)
        {
            TrainingFacilityData data = pair.Value;
            if (data == null)
            {
                continue;
            }

            if (data.Type == type && data.Level == level)
            {
                return data;
            }
        }

        return null;
    }

    public TrainingFacilityData GetTrainingFacilityDataByTrainingId(string trainingDataId)
    {
        if (TrainingFacilityDataList == null || string.IsNullOrEmpty(trainingDataId) == true)
        {
            return null;
        }
        
        foreach (KeyValuePair<string, TrainingFacilityData> pair in TrainingFacilityDataList)
        {
            TrainingFacilityData data = pair.Value;
            if (data == null)
            {
                continue;
            }

            if (data.TrainingDataId == trainingDataId)
            {
                return data;
            }
        }

        return null;
    }

    public TrainingPolicyData GetTrainingPolicyData(string Id)
    {
        if (TrainingPolicyDataList == null || string.IsNullOrEmpty(Id)) return null;

        return TrainingPolicyDataList.TryGetValue(Id, out var item) ? item : null;
    }
    
    public TrainingPolicyData GetTrainingPolicyDataByCategoryAndFocus(string category, string focus)
    {
        if (TrainingPolicyDataList == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(category) == true || string.IsNullOrEmpty(focus) == true)
        {
            return null;
        }

        foreach(KeyValuePair<string, TrainingPolicyData> pair in TrainingPolicyDataList)
        {
            TrainingPolicyData policyData = pair.Value;

            if (policyData == null)
            {
                continue;
            }

            if (policyData.Category == category && policyData.Focus == focus)
            {
                return policyData;
            }
        }

        return null;
    }

    public SkillData GetSkillData(string id)
    {
        if (SkillDataList == null || string.IsNullOrEmpty(id)) return null;

        return SkillDataList.TryGetValue(id, out var item) ? item : null;
    }

    public SkillUseConditionData GetSkillUseConditionData(string id)
    {
        if (SkillUseConditionDataList == null || string.IsNullOrEmpty(id)) return null;

        return SkillUseConditionDataList.TryGetValue(id, out var item) ? item : null;
    }

    public MatchRuleData GetMatchRuleData(string id)
    {
        if (MatchRuleDataList == null || string.IsNullOrEmpty(id)) return null;

        return MatchRuleDataList.TryGetValue(id, out var item) ? item : null;
    }
}
