using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchScheduleUI : UIBase
{
    private const string DefaultOrganizationId = "org_01";

    [SerializeField] private Text Text_OrganizationName;
    [SerializeField] private Transform Transform_Content;
    [SerializeField] private MatchOpponentRowUI Prefab_OpponentRow;
    [SerializeField] private UIButton Button_Close;

    [SerializeField] private UIButton Button_RequestYes;
    [SerializeField] private UIButton Button_RequestNo;

    
    private List<FighterData> _opponentList = new List<FighterData>();


    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
        Button_RequestYes.BindOnClickButtonEvent(OnClick_RequestYes);
        Button_RequestNo.BindOnClickButtonEvent(OnClick_RequestNo);

        LoadOpponents();
        RefreshListUI();
    }

    private void LoadOpponents()
    {

    }

    private void RefreshListUI()
    {

    }

    public void SelectOpponentRow()
    {

    }

    private void ShowRequestPanel(FighterData opponent)
    {

    }

    private void HideRequestPanel()
    {

    }

    private void OnClick_Close()
    {

    }

    private void OnClick_RequestYes()
    {

    }

    private void OnClick_RequestNo()
    {

    }
}
