using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField] private List<InventoryEntry> StartingItems = new List<InventoryEntry>();

    public InventoryModel CurrentInventory { get; private set; } = new InventoryModel();

    public event Action<string, int, int> OnItemCountChanged;

    private void Awake()
    {
        Instance = this;
    }

    public void CreateStartingInventory()
    {
        CurrentInventory.Clear();

        for (int index = 0; index < StartingItems.Count; index++)
        {
            InventoryEntry entry = StartingItems[index];

            if (entry == null)
            {
                Debug.LogError($"스타팅 인벤토리 Entry가 없음 Index : {index}");
                continue;
            }

            if (TryAddItem(entry.ItemId, entry.Count) == false)
            {
                continue;
            }

            Debug.Log($"스타팅 아이템 추가 완료");
        }

        Debug.Log($"스타팅 인벤토리 생성 완료");
    }

    public int GetItemCount(string itemId)
    {
        return CurrentInventory.GetItemCount(itemId);
    }

    public bool HasItem(string itemId, int count = 1)
    {

        return CurrentInventory.HasItem(itemId, count);
    }

    public List<InventoryEntry> GetAllEntries()
    {
        return CurrentInventory.GetAllEntries();
    }

    public bool TryAddItem(string itemId, int count)
    {
        if (IsValidItemId(itemId) == false)
        {
            return false;
        }

        if (count <= 0)
        {
            Debug.LogError($"추가할 아이템 수량이 음수임 ItemId : {itemId}, Count : {count}");
            return false;
        }

        int previousCount = CurrentInventory.GetItemCount(itemId);

        if (CurrentInventory.TryAddItem(itemId, count) == false)
        {
            Debug.LogError($"아이템 추가 실패 ItemId : {itemId}, Count : {count}");
            return false;
        }

        int currentCount = CurrentInventory.GetItemCount(itemId);

        OnItemCountChanged?.Invoke(itemId, previousCount, currentCount);

        return true;
    }

    public bool TryRemoveItem(string itemId, int count)
    {
        if (IsValidItemId(itemId) == false)
        {
            return false;
        }

        if (count <= 0)
        {
            Debug.LogError($"아이템 수량이 음수임 ItemId : {itemId}, Count : {count}");
            return false;
        }

        int previousCount = CurrentInventory.GetItemCount(itemId);

        if (CurrentInventory.TryRemoveItem(itemId, count) == false)
        {
            Debug.Log($"아이템 수량 부족 Item : {itemId}, 필요 : {count}, 보유 : {previousCount}");
            return false;
        }

        int currentCount = CurrentInventory.GetItemCount(itemId);

        OnItemCountChanged?.Invoke(itemId, previousCount, currentCount);

        return true;
    }

    private bool IsValidItemId(string itemId)
    {
        if (string.IsNullOrEmpty(itemId) == true)
        {
            Debug.LogError("ItemId 비어있음");
            return false;
        }

        if (GameDataManager.Instance == null)
        {
            Debug.LogError("GameDataManager가 없어서 ItemData 확인안됨");
            return false;
        }

        if (GameDataManager.Instance.GetItemData(itemId) == null)
        {
            Debug.LogError($"ItemData 없음 ItemId : {itemId}");
            return false;
        }

        return true;
    }
}
