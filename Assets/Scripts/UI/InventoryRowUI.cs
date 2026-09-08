using UnityEngine;
using UnityEngine.UI;

public class InventoryRowUI : MonoBehaviour
{
    [SerializeField] private Text Text_Name;
    [SerializeField] private Text Text_Description;
    [SerializeField] private Text Text_Count;

    public void Setup(ItemData itemData, int count)
    {
        Text_Name.text = itemData.Name;
        Text_Description.text = itemData.Description;
        Text_Count.text = $"{count}";
    }
}
