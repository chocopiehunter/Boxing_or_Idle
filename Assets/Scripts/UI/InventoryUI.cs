using UnityEngine;

public class InventoryUI : UIBase
{
    [SerializeField] private UIButton Button_Consumable;
    [SerializeField] private UIButton Button_KeyItem;
    [SerializeField] private UIButton Button_Close;

    [SerializeField] private Transform Transform_ItemContent;
    [SerializeField] private InventoryRowUI Prefab_ItemRow;

    private ItemType _selectedItemType = ItemType.Consumable;

    private void OnEnable()
    {
        Button_Consumable.BindOnClickButtonEvent(OnClick_Comsumable);
        Button_KeyItem.BindOnClickButtonEvent(OnClick_KeyItem);
        Button_Close.BindOnClickButtonEvent(OnClick_Close);

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager 없음");

            return;
        }

        InventoryManager.Instance.OnItemCountChanged += OnItemCountChanged;

        SelectItemType(ItemType.Consumable);
    }

    private void OnDisable()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        InventoryManager.Instance.OnItemCountChanged -= OnItemCountChanged;
    }

    private void SelectItemType(ItemType itemType)
    {
        _selectedItemType = itemType;

        Button_Consumable.SetInteractable(itemType != ItemType.Consumable);

        Button_KeyItem.SetInteractable(itemType != ItemType.KeyItem);

        RefreshItemListUI();
    }

    private void RefreshItemListUI()
    {

    }

    private void ClearItemRowUI()
    {

    }

    private void OnItemCountChanged(string itemId, int previousCount, int currentCount)
    {
        RefreshItemListUI();
    }

    private void OnClick_Comsumable()
    {
        SelectItemType(ItemType.Consumable);
    }

    private void OnClick_KeyItem()
    {
        SelectItemType(ItemType.KeyItem);
    }

    private void OnClick_Close()
    {
        UIManager.Instance.CloseContentUI(UIType.InventoryUI);
    }
}
