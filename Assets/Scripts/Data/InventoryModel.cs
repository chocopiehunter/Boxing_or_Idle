using System.Collections.Generic;
using UnityEngine;

public class InventoryModel
{
    private readonly Dictionary<string, int> _itemCounts = new Dictionary<string, int>();

    public int GetItemCount(string itemId)
    {
        if (string.IsNullOrEmpty(itemId) == true)
        {
            return 0;
        }

        if (_itemCounts.TryGetValue(itemId, out int count) == true)
        {
            return count;
        }

        return 0;
    }

    public bool HasItem(string itemId, int count = 1)
    {
        if (count <= 0)
        {
            return false;
        }

        return GetItemCount(itemId) >= count;
    }

    public bool TryAddItem(string itemId, int count)
    {
        if (string.IsNullOrEmpty(itemId) == true)
        {
            return false;
        }

        if (count <= 0)
        {
            return false;
        }

        int currentCount = GetItemCount(itemId);

        if (currentCount > int.MaxValue - count)
        {
            return false;
        }

        _itemCounts[itemId] = currentCount + count;

        return true;
    }

    public bool TryRemoveItem(string itemId, int count)
    {
        if (string.IsNullOrEmpty(itemId) == true)
        {
            return false;
        }

        if (count <= 0)
        {
            return false;
        }

        int currentCount = GetItemCount(itemId);

        if (currentCount < count)
        {
            return false;
        }

        if (currentCount == count)
        {
            _itemCounts.Remove(itemId);
            return true;
        }

        _itemCounts[itemId] = currentCount - count;

        return true;
    }

    public List<InventoryEntry> GetAllEntries()
    {
        List<InventoryEntry> result = new List<InventoryEntry>();

        foreach(KeyValuePair<string, int> pair in _itemCounts)
        {
            InventoryEntry entry = new InventoryEntry();

            entry.ItemId = pair.Key;

            entry.Count = pair.Value;

            result.Add(entry);
        }

        result.Sort(CompareItemId);

        return result;
    }

    public void Clear()
    {
        _itemCounts.Clear();
    }

    private int CompareItemId(InventoryEntry left, InventoryEntry right)
    {
        return string.CompareOrdinal(left.ItemId, right.ItemId);
    }
}
