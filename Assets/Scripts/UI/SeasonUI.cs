using UnityEngine;
using UnityEngine.UI;

public class SeasonUI : UIBase
{
    [SerializeField] private Image Image_SeasonIcon;
    [SerializeField] private Text Text_Week;

    private const string WeekFromat = "{0}주차";

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    private void Refresh()
    {

    }
}
