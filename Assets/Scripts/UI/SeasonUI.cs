using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SeasonUI : UIBase
{
    [SerializeField] private Image Image_Season;
    [SerializeField] private Text Text_Week;
    [SerializeField] private SeasonIconTable IconTable;

    private const string WeekTextFormat = "{0}주차";

    public void SetSeason(Season season, int week)
    {
        Text_Week.text = string.Format(WeekTextFormat, week);
        SetSeasonIconAsync(season).Forget();
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
