using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchStrategySelectionUI : UIBase
{
    [SerializeField] private Text Text_StrategyDescription;

    private void Awake()
    {
        if (Text_StrategyDescription != null)
        {
            Text_StrategyDescription.raycastTarget = false;
        }

        HideDescription();
    }

    public void ShowDescription(string description)
    {
        if (Text_StrategyDescription == null)
        {
            return;
        }

        Text_StrategyDescription.text = description;
        Text_StrategyDescription.gameObject.SetActive(true);
    }

    public void HideDescription()
    {
        if (Text_StrategyDescription == null)
        {
            return;
        }

        Text_StrategyDescription.text = "";
        Text_StrategyDescription.gameObject.SetActive(false);
    }
}
