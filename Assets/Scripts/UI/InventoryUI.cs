using UnityEngine;

public class InventoryUI : UIBase
{
    [SerializeField] private UIButton Button_Consumable;
    [SerializeField] private UIButton Button_KeyItem;
    [SerializeField] private UIButton Button_Close;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void SelectItemType()
    {

    }

    private void RefreshItemListUI()
    {

    }

    private void ClearItemRowUI()
    {

    }

    private void OnClick_Comsumable()
    {

    }

    private void OnClick_KeyItem()
    {

    }

    private void OnClick_Close()
    {
        UIManager.Instance.CloseContentUI(UIType.InventoryUI);
    }
}
