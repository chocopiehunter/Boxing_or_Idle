using System;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField] private Button Button_Base;
    [SerializeField] private Text Text_Base;
    [SerializeField] private Image Image_Base;
    [SerializeField] private Image Image_Select;

    // 수동으로 끊어줌
    private bool _isSlotMenualUnbindEvent;

    private void Awake()
    {
        InitUIButton();
        SetDefaultUI();
    }

    private void OnDisable()
    {
        if (_isSlotMenualUnbindEvent == false)
        {
            Button_Base.onClick.RemoveAllListeners();
        }
    }

    private void SetDefaultUI()
    {
        if (Image_Select != null) 
        {
            Image_Select.gameObject.SetActive(false);
        }
    }

    private void InitUIButton()
    {
        if(Button_Base != null)
        {
            return;
        }

        // 외부에서 등록할 수 있고, 누락했다면 등록안해도 알아서 찾아주도록 로직을 넣어놨다
        var button = this.gameObject.GetComponentInChildren<Button>();
        if (button != null)
        {
            this.Button_Base = button;
        }
    }

    public void BindOnClickButtonEvent(Action onClickCallback, bool isMenualUnbineEvent = false)
    {
        if (Button_Base == null)
        {
            Button_Base.onClick.AddListener(onClickCallback.Invoke);
            _isSlotMenualUnbindEvent = isMenualUnbineEvent;
        }
    }

    public void UnBindAllOnClickButtonEvent()
    {
        if (Button_Base == null) return;

        Button_Base.onClick.RemoveAllListeners();
    }

    public void ChangeButtonText(string buttonStr)
    {
        //혹시 이 버튼을 동적으로 코드에서 텍스트 수정해야할 때 사용
        if (Text_Base == null) return;

        Text_Base.text = buttonStr;
    }
}
