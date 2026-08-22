using System;
using System.Collections.Generic;
using UnityEngine;

public class TrainingSpotManager : MonoBehaviour
{
    [Serializable]
    private class SpawnSetting
    {
        [SerializeField] private string FacilityType;
        [SerializeField] private TrainingSpot Prefab;
        [SerializeField] private Transform SpawnPoint;

        public string Type { get { return FacilityType; } }


        public TrainingSpot SpotPrefab { get { return Prefab; } }

        public Transform Point { get { return SpawnPoint; } }

    }

    public static TrainingSpotManager Instance { get; private set; }

    [SerializeField] private Transform Transform_SpotRoot;
    [SerializeField] private List<SpawnSetting> SpawnSettings = new List<SpawnSetting>();

    private readonly Dictionary<string, TrainingSpot> _spawnedSpots = new Dictionary<string, TrainingSpot>();

    private readonly List<ITrainingSpot> _trainingSpots = new List<ITrainingSpot>();

    public IReadOnlyList<ITrainingSpot> TrainingSpots { get { return _trainingSpots; } }

    public event Action OnTrainingSpotsChanged;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool SpawnOrUpdate(TrainingFacilityData facilityData)
    {
        if (facilityData == null)
        {
            return false;
        }

        if (_spawnedSpots.TryGetValue(facilityData.Type, out TrainingSpot existingSpot))
        {
            existingSpot.Bind(facilityData.TrainingDataId, true);
            OnTrainingSpotsChanged?.Invoke();
            return true;
        }

        SpawnSetting setting = FindSetting(facilityData.Type);
        if (setting == null)
        {
            Debug.LogError($"훈련 시설 스폰 설정 없음. Type: {facilityData.Type}");
            return false;
        }

        if (setting.SpotPrefab == null)
        {
            Debug.LogError(
                $"훈련 시설 프리팹 연결 안 됨. Type: {facilityData.Type}");
            return false;
        }

        if (setting.Point == null)
        {
            Debug.LogError(
                $"훈련 시설 생성 위치 연결 안 됨. Type: {facilityData.Type}");
            return false;
        }

        TrainingSpot spot = Instantiate(setting.SpotPrefab, Transform_SpotRoot);

        spot.transform.SetPositionAndRotation(setting.Point.position, setting.Point.rotation);

        spot.Bind(facilityData.TrainingDataId, true);

        _spawnedSpots.Add(facilityData.Type, spot);
        _trainingSpots.Add(spot);

        OnTrainingSpotsChanged?.Invoke();
        return true;
    }

    public void ClearAll()
    {
        foreach (TrainingSpot spot in _spawnedSpots.Values)
        {
            if (spot != null)
            {
                Destroy(spot.gameObject);
            }
        }

        _spawnedSpots.Clear();
        _trainingSpots.Clear();

        OnTrainingSpotsChanged?.Invoke();
    }

    private SpawnSetting FindSetting(string facilityType)
    {
        for (int i = 0; i < SpawnSettings.Count; i++)
        {
            SpawnSetting setting = SpawnSettings[i];

            if (setting != null && setting.Type == facilityType)
            {
                return setting;
            }
        }

        return null;
    }
}
