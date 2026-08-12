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
    [SerializeField] private GameObject Panel_Request;
    [SerializeField] private Text Text_RequestMessage;

    
    private List<FighterData> _opponentList = new List<FighterData>();
    private int _selectedIndex = -1;

    private void OnEnable()
    {
        Button_Close.BindOnClickButtonEvent(OnClick_Close);
        Button_RequestYes.BindOnClickButtonEvent(OnClick_RequestYes);
        Button_RequestNo.BindOnClickButtonEvent(OnClick_RequestNo);

        HideRequestPanel();
        LoadOpponents();
        RefreshListUI();
    }

    private void LoadOpponents()
    {
        OrganizationData orgData = GameDataManager.Instance.GetOrganizationData(DefaultOrganizationId);
        if (orgData == null)
        {
            Debug.LogError($"OrganizationData 없음 {DefaultOrganizationId}");
            return;
        }

        if (Text_OrganizationName != null)
        {
            Text_OrganizationName.text = orgData.Name;
        }

        List<string> fighterIds = orgData.GetFighterIdList();
        for(int i = 0; i < fighterIds.Count; i++)
        {
            FighterData fighterData = GameDataManager.Instance.GetFighterData(fighterIds[i]);
            if (fighterData == null)
            {
                Debug.LogError($"FighterData 없음 {fighterIds[i]}");
                continue;
            }

            _opponentList.Add(fighterData);
        }
    }

    private void RefreshListUI()
    {

    }

    public void SelectOpponentRow(int index)
    {
        if (index < 0 || index >= _opponentList.Count)
        {
            return;
        }

        _selectedIndex = index;
        ShowRequestPanel(_opponentList[index]);
    }

    private void ClearRowUI()
    {

    }

    private void ShowRequestPanel(FighterData opponent)
    {
        if (Panel_Request == null)
        {
            return;
        }

        if ()
    }

    private void HideRequestPanel()
    {
        _selectedIndex = -1;

        if(Panel_Request != null)
        {
            Panel_Request.SetActive(false);
        }
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
