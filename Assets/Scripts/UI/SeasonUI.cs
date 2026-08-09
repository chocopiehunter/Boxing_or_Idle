using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SeasonUI : UIBase
{
    [SerializeField] private Image Image_Season;
    [SerializeField] private Text Text_Year;
    [SerializeField] private Text Text_Month;
    [SerializeField] private Text Text_Week;
    [SerializeField] private Slider Slider_MonthProgress;
    [SerializeField] private SeasonIconTable IconTable;

    private const string YearTextFormat = "{0}년";
    private const string MonthTextFormat = "{0}월";
    private const string WeekTextFormat = "{0}주";

    public void SetSeason(Season season,int year, int month, int week)
    {
        Text_Year.text = string.Format("{0}년", year);
        Text_Month.text = string.Format("{0}월", month);
        Text_Week.text = string.Format("{0}주", week);
        SetSeasonIconAsync(season).Forget();
    }

    public void SetMonthProgress(float ratio)
    {
        Slider_MonthProgress.value = Mathf.Clamp01(ratio);
    }

    private async UniTaskVoid SetSeasonIconAsync(Season season)
    {
        string address = IconTable.GetIconAddress(season);

        if (string.IsNullOrEmpty(address) == true)
        {
            return;
        }

        await GameUtil.LoadAndSetSpriteImage(Image_Season, address);
    }
}
