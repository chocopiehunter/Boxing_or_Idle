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

    public bool TryUpgradeGym()
    {
        if (CurrentGym == null)
        {
            Debug.LogError("체육관이 없음");
            return false;
        }

        GymLevelData nextLevelData = GetNextLevelData();
        if (nextLevelData == null)
        {
            Debug.Log("체육관이 최대 레벨입니다");
            return false;
        }

        if (nextLevelData.RequiredUnlockIds != "None") 
        {
            Debug.Log($"체육관 승급 조건 미구현 : {nextLevelData.RequiredUnlockIds}");
            return false;
        }

        if (CurrentGym.TrySpendGold(nextLevelData.GoldCost) == false)
        {
            Debug.Log($"자금 부족. 필요: {nextLevelData.GoldCost} / 보유: {CurrentGym.Gold}");
            return false;
        }

        CurrentGym.ApplyLevelData(nextLevelData);
        Debug.Log($"체육관 업그레이드 {CurrentGym.Level} {nextLevelData.Name} / 남은 자금: {CurrentGym.Gold}");
        return true;
    }

    // 테스트 코드
    [ContextMenu("체육관 업그레이드 테스트")]
    private void DebugUpgrade()
    {
        TryUpgradeGym();
    }
}
