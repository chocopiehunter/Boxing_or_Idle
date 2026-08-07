using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SeasonUI : UIBase
{
    [SerializeField] private Image Image_Season;
    [SerializeField] private Text Text_Week;
    [SerializeField] private SeasonIconTable IconTable;

    private const string WeekTextFormat = "{0}주차";

    private void OnEnable()
    {
        if (SeasonManager.Instance != null)
        {
            SeasonManager.Instance.OnWeekAdvanced += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (SeasonManager.Instance != null)
        {
            SeasonManager.Instance.OnWeekAdvanced -= Refresh;
        }
    }

    private void Refresh()
    {
        if (SeasonManager.Instance == null)
        {
            Debug.LogError("SeasonManager가 없습니다");
            return;
        }

        Text_Week.text = string.Format(WeekTextFormat, SeasonManager.Instance.CurrentWeek);
        SetSeasonIconAsync(SeasonManager.Instance.CurrentSeason).Forget();
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
