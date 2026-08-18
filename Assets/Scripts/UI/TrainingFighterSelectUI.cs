using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingFighterSelectUI : UIBase
{
    [SerializeField] private Button Button_Close;
    [SerializeField] private Button Button_Fighter1;
    [SerializeField] private Button Button_Fighter2;
    [SerializeField] private Button Button_Fighter3;
    [SerializeField] private Button Button_Fighter4;

    [SerializeField] private Text Text_Name_Fighter1;
    [SerializeField] private Text Text_Name_Fighter2;
    [SerializeField] private Text Text_Name_Fighter3;
    [SerializeField] private Text Text_Name_Fighter4;

    private void OnEnable()
    {
        Button_Close.onClick.AddListener(OnClick_Close);
        Button_Fighter1.onClick.AddListener(OnClick_Fighter1);
        Button_Fighter2.onClick.AddListener(OnClick_Fighter2);
        Button_Fighter3.onClick.AddListener(OnClick_Fighter3);
        Button_Fighter4.onClick.AddListener(OnClick_Fighter4);

        RefreshUI();
    }

    private void OnDisable()
    {
        Button_Close.onClick.RemoveListener(OnClick_Close);
        Button_Fighter1.onClick.RemoveListener(OnClick_Fighter1);
        Button_Fighter2.onClick.RemoveListener(OnClick_Fighter2);
        Button_Fighter3.onClick.RemoveListener(OnClick_Fighter3);
        Button_Fighter4.onClick.RemoveListener(OnClick_Fighter4);
    }

    private void RefreshUI()
    {
        List<FighterModel> fighters = FighterManager.Instance.PlayerFighters;

        RefreshSlot(Button_Fighter1, Text_Name_Fighter1, 0, fighters);
        RefreshSlot(Button_Fighter2, Text_Name_Fighter2, 1, fighters);
        RefreshSlot(Button_Fighter3, Text_Name_Fighter3, 2, fighters);
        RefreshSlot(Button_Fighter4, Text_Name_Fighter4, 3, fighters);
    }

    private void RefreshSlot(Button button, Text nameText, int index, List<FighterModel> fighters)
    {

    }

    private void OpenTrainingManagement(int index)
    {

    }

    private void OnClick_Fighter1()
    {

    }

    private void OnClick_Fighter2()
    {

    }

    private void OnClick_Fighter3()
    {

    }

    private void OnClick_Fighter4()
    {

    }

    private void OnClick_Close()
    {

    }
}
