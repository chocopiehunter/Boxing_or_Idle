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

    [SerializeField] private RectTransform Rect_Window;

    
    private List<FighterData> _opponentList = new List<FighterData>();
    private int _selectedIndex = -1;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClick_Close();
            return;
        }

        if (Input.GetMouseButtonDown(0) == false)
        {
            return;
        }

        bool inside = RectTransformUtility.RectangleContainsScreenPoint(Rect_Window, Input.mousePosition, null);

        if (inside == true)
        {
            return;
        }

        OnClick_Close();
    }

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
        _opponentList.Clear();

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
        ClearRowUI();

        for (int i = 0; i < _opponentList.Count; i++)
        {
            MatchOpponentRowUI rowUI = Instantiate(Prefab_OpponentRow, Transform_Content);
            int rank = i + 1;
            rowUI.Setup(this, i, rank, _opponentList[i]);
        }
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
        for (int i = Transform_Content.childCount - 1; i >= 0; i--)
        {
            Destroy(Transform_Content.GetChild(i).gameObject);
        }
    }

    private void ShowRequestPanel(FighterData opponent)
    {
        if (Panel_Request == null)
        {
            return;
        }

        if (Text_RequestMessage != null)
        {
            Text_RequestMessage.text = $"{opponent.Name} 에게 경기를 요청하시겠습니까?";
        }

        Panel_Request.SetActive(true);
    }

    private void HideRequestPanel()
    {
        _selectedIndex = -1;

        if(Panel_Request != null)
        {
            Panel_Request.SetActive(false);
        }
    }

    private void OnClick_RequestYes()
    {
        if (_selectedIndex  < 0 || _selectedIndex >= _opponentList.Count)
        {
            Debug.LogWarning("선택된 상대가 없습니다");
            return;
        }

        FighterModel player = FighterManager.Instance.GetFirstFighter();
        FighterData opponent = _opponentList[_selectedIndex];

        int opponentRank = _selectedIndex + 1;

        MatchRuleData ruleData = GameDataManager.Instance.GetMatchRuleDataByOpponent(opponentRank, false, false);
        if (ruleData == null)
        {
            Debug.LogError($"상대에게 적용할 경기 규칙 없음 Rank={opponentRank}");
            return;
        }

        bool success = MatchManager.Instance.TryScheduleMatch(player, opponent, ruleData);

        if (success == false)
        {
            return;
        }

        bool startSuccess = MatchManager.Instance.TryStartMatch();
        if (startSuccess == false)
        {
            return;
        }

        HideRequestPanel();
        UIManager.Instance.ClosePopupUI(UIType.MatchScheduleUI);
        UIManager.Instance.ClosePopupUI(UIType.MenuUI);

        GameManager.Instance.GameState.ChangeState(GameFlowState.Pause);
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.MatchUI);
    }

    private void OnClick_RequestNo()
    {
        HideRequestPanel();
    }

    private void OnClick_Close()
    {
        Debug.Log("MatchScheduleUI Close 버튼 눌림");
        HideRequestPanel();
        UIManager.Instance.ClosePopupUI(UIType.MatchScheduleUI);
    }
}
