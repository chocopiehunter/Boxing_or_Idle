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
    private Action<string> _onClick;

    public void Setup()
    {

    }

    private void OnClick_Action()
    {

    }
}
