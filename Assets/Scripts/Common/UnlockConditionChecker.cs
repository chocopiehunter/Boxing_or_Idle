using UnityEngine;

public static class UnlockConditionChecker
{
    private const string NoneId = "None";
    private const string TotalWinCountType = "TotalWinCount";
    private const string WinStreakType = "WinStreak";
    private const string HasItemType = "HasItem";

    public static bool CanUnlockAll(string requiredUnlockIds)
    {
        if (string.IsNullOrEmpty(requiredUnlockIds) == true)
        {
            return true;
        }

        if (requiredUnlockIds == NoneId)
        {
            return true;
        }

        string[] splitIds = requiredUnlockIds.Split(',');

        for (int i = 0; i < splitIds.Length; i++)
        {
            string conditionId = splitIds[i].Trim();

            if (string.IsNullOrEmpty(conditionId) == true)
            {
                continue;
            }

            if (CanUnlock(conditionId) == false)
            {
                return false;
            }
        }
 
        return true;
    }

    public static bool CanUnlock(string conditionId)
    {
        if (string.IsNullOrEmpty(conditionId) == true || conditionId == NoneId)
        {
            return true;
        }

        if (GameDataManager.Instance == null)
        {
            Debug.LogError($"GameDataManager가 없어 해금 조건 확인할수 없음");
            return false;
        }

        UnlockConditionData conditionData = GameDataManager.Instance.GetUnlockConditionData(conditionId);

        if (conditionData == null)
        {
            Debug.LogError($"해금 조건 데이터 없음 {conditionId}");
            return false;
        }

        return CanUnlock(conditionData);
    }

    private static bool CanUnlock(UnlockConditionData conditionData)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError($"GameManager가 없어 해금 조건 확인할수 없음");
            return false;
        }

        GameState gameState = GameManager.Instance.GameState;

        if (conditionData.Type == TotalWinCountType)
        {
            return gameState.TotalWinCount >= conditionData.Count;
        }

        if (conditionData.Type == WinStreakType)
        {
            return gameState.CurrentWinStreak >= conditionData.Count;
        }

        if (conditionData.Type == HasItemType)
        {
            return false;
        }
        
        Debug.LogError($"지원하지 않는 해금 조건 Type {conditionData.Type}");
        return false;
    }
}
