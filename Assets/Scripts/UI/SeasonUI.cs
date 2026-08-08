using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SeasonUI : UIBase
{
    [SerializeField] private Image Image_Season;
    [SerializeField] private Text Text_Month;
    [SerializeField] private SeasonIconTable IconTable;

    private const string MonthTextFormat = "{0}월";

    public void SetSeason(Season season, int month)
    {
        Text_Month.text = string.Format(MonthTextFormat, month);
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
