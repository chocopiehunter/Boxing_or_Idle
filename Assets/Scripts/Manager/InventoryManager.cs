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

    }

    public int GetItemCount()
    {
        return 0;
    }

    public bool HasItem()
    {
        return true;
    }

    public List<InventoryEntry> GetAllEntries()
    {
        return CurrentInventory.GetAllEntries();
    }

    public bool TryAddItem()
    {
        return true;
    }

    public bool TryRemoveItem()
    {
        return true;
    }
}
