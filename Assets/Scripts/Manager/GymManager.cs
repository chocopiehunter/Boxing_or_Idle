using UnityEngine;

public class GymManager : MonoBehaviour
{
    public static GymManager Instance { get; private set; }

    [SerializeField] private string StartingLevelId = "gym_level_01";
    [SerializeField] private int StartingGold = 300;

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

        CurrentGym = new GymModel(startData, StartingGold);
        Debug.Log($"시작 체육관 생성완료. 레벨: {CurrentGym.Level} / 자금: {CurrentGym.Gold}");
    }

    public void ClearGym()
    {
        CurrentGym = null;
    }

    public GymLevelData GetCurrentLevelData()
    {
        if (CurrentGym == null)
        {
            return null;
        }

        return GameDataManager.Instance.GetGymLevelData(CurrentGym.LevelId);
    }

    public GymLevelData GetNextLevelData()
    {
        GymLevelData currentData = GetCurrentLevelData();
        if(currentData == null)
        {
            return null;
        }

        if (string.IsNullOrEmpty(currentData.NextLevelId) == true)
        {
            return null;
        }

        return GameDataManager.Instance.GetGymLevelData(currentData.NextLevelId);
    }

    //public bool TryUpgradeGym()
    //{

    //}
}
