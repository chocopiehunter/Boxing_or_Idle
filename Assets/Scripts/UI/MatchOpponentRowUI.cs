using UnityEngine;
using UnityEngine.UI;

public class MatchOpponentRowUI : MonoBehaviour
{
    [SerializeField] private UIButton Button_Row;
    [SerializeField] private Text Text_Rank;
    [SerializeField] private Text Text_Name;
    [SerializeField] private Text Text_Stats;

    private MatchScheduleUI _owner;
    private int _index;

    public void Setup(MatchScheduleUI owner, int index, int rank, FighterData fighterData)
    {
        _owner = owner;
        _index = index;

        if (Text_Rank != null)
        {
            Text_Rank.text = $"{rank}";
        }

        if (Text_Name != null)
        {
            Text_Name.text = fighterData.Name;
        }

        if (Text_Stats != null)
        {
            Text_Stats.text = $"Hp: {fighterData.Hp} / Stamina: {fighterData.Stamina} / StandingOffense: {fighterData.StandingOffense} / StandingDefense: {fighterData.StandingDefense}";
        }

        Button_Row.UnBindAllOnClickButtonEvent();
        Button_Row.BindOnClickButtonEvent(OnClick_Row);
    }

    private void OnClick_Row()
    {
        if (_owner == null)
        {
            return;
        }

        _owner.SelectOpponentRow(_index);
    }
}
