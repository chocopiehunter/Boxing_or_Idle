using UnityEngine;

public class GymManager : MonoBehaviour
{
    public static GymManager Instance { get; private set; }

    [SerializeField] private int StartingLevel = 1;
    [SerializeField] private int StartingGold = 300;

    public GymModel CurrentGym {  get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void CreateStartingGym()
    {
        CurrentGym = new GymModel(StartingLevel, StartingGold);
        Debug.Log($"시작 체육관 생성완료. 레벨: {CurrentGym.Level} / 자금: {CurrentGym.Gold}");
    }

    public void ClearGym()
    {
        CurrentGym = null;
    }

    //public GymLevelData GetCurrentLevelData()
    //{

    //}

    //public GymLevelData GetNextLevelData()
    //{

    //}

    //public bool TryUpgradeGym()
    //{

    //}
}
