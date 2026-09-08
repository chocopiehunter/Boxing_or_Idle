using System.Collections.Generic;
using UnityEngine;

public class InventoryModel
{
    private readonly Dictionary<string, int> _itemCounts = new Dictionary<string, int>();

    public int GetItemCount()
    {
        return 0;
    }

    public bool TryAddItem()
    {
        return true;
    }

    public bool TryRemoveItem()
    {
        return true;
    }

    public void Clear()
    {
        _itemCounts.Clear();
    }
}
