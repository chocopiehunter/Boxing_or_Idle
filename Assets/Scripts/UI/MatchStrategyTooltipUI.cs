using UnityEngine;
using UnityEngine.EventSystems;

public class MatchStrategyTooltipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MatchStrategySelectionUI _owner;
    private string _description;

    public void Setup(MatchStrategySelectionUI owner, string description)
    {
        _owner = owner;
        _description = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_owner == null)
        {
            return;
        }

        _owner.ShowDescription(_description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_owner == null)
        {
            return;
        }

        _owner.HideDescription();
    }
}
