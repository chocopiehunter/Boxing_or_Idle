using System;
using UnityEngine;
using UnityEngine.UI;

public class GymFacilityRowUI : MonoBehaviour
{
    [SerializeField] private UIButton Button_Action;
    [SerializeField] private Text Text_Level;
    [SerializeField] private Text Text_Name;
    [SerializeField] private Text Text_State;

    private string _facilityType;
    private Action<string> _onClick; // 버튼 누를때 시설 타입을 GymManagementUI로 전달하는 함수 보관용

    public void Setup(string facilityType, TrainingFacilityData currentData, TrainingFacilityData nextData, Action<string> onClick)
    {
        _facilityType = facilityType;
        _onClick = onClick;

        Button_Action.UnBindAllOnClickButtonEvent();
        Button_Action.BindOnClickButtonEvent(OnClick_Action);

        if (currentData == null)
        {
            Text_Level.text = "미건설";
            Text_Name.text = nextData != null ? nextData.Name : facilityType;
            Text_State.text = nextData != null ? "건설 가능" : "건설 불가";
        }
        else
        {
            Text_Level.text = $"Lv.{currentData.Level}";
            Text_Name.text = currentData.Name;
            Text_State.text = nextData != null ? $"다음 Lv.{nextData.Level}" : "최대 레벨";
        }

        Button_Action.SetInteractable(nextData != null);
    }

    private void OnClick_Action()
    {
        if (_onClick == null)
        {
            return;
        }

        _onClick.Invoke(_facilityType);
    }
}
