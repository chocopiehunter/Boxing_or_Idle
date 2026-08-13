using Cysharp.Threading.Tasks;
using UnityEngine;

public enum UIRootType
{
    None = 0,
    BackgroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    VeryFrontUI
}

public enum UIType
{
    MainUI,
    LoadingUI,
    TitleUI,
    Inventory,
    DialogueUI,
    SimplePopupUI,
    SettingUI,
    MenuUI,
    TrainingManagementUI,
    MatchScheduleUI
}

public static class UIManagerExtension
{
    public static string GetUIPath(this UIManager uiManager, UIRootType uiRootType, UIType uiType)
    {
        string path = string.Empty;

        path = $"Prefabs/UI/{uiRootType}/{uiType}";
        return path;
    }

    public static async UniTaskVoid ShowStartupUIOnGameStart(this UIManager uiManager)
    {
        LoadingUI loadingUI = UIManager.Instance.OpenUI(UIRootType.VeryFrontUI, UIType.LoadingUI) as LoadingUI;

        if (loadingUI != null)
        {
            await loadingUI.PlayLoadingBarAsync();
        }

        UIManager.Instance.CloseUI(UIRootType.VeryFrontUI, UIType.LoadingUI);
        UIManager.Instance.OpenUI(UIRootType.MainUI, UIType.TitleUI);
    }
}
