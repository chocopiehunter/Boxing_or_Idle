using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; set; }

    public GameState GameState { get; private set; } = new GameState();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // LoadSaveData();
    }

    public void StartGame()
    {

    }

    public void SaveData()
    {

    }

    public void SaveAndEndGame()
    {

    }

    private void LoadSaveData()
    {

    }

    public void TryReceiveNgAchievement(string id)
    {
        NgAchievementData data = GameDataManager.Instance.GetNgAchievementData(id);
        if (data == null)
        {
            Debug.LogError($"NgAchievementData 없음 {id}");
            return;
        }

        bool isFirstTimeOnly = false;
        if (data.AchievementType == "FirstTimeOnly")
        {
            isFirstTimeOnly = true;
        }

        if (isFirstTimeOnly == true)
        {
            if (GameState.HasReceivedNgAchievement(id) == true)
            {
                return;
            }
        }

        GameState.AddNgPlusPoints(data.Point);
        GameState.AddReceivedNgAchievement(id);

        if (id == "ng_001")
        {
            GameState.SetFirstGameCleared(true);
        }

        Debug.Log($"{data.Description} / + {data.Point} 다회차 포인트 획득 / 총 {GameState.NgPlusPoints}");
    }
}
