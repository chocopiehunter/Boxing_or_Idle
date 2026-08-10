using System;
using UnityEngine;
using UnityEngine.UI;

public class UIButton_Check : MonoBehaviour
{
    [SerializeField] private Button Button_Base;
    [SerializeField] private Text Text_Base;
    [SerializeField] private Image Image_Base;
    [SerializeField] private Image Image_Select;

    private bool _isSlotMenualUnbindEvent;
    private bool _isChecked;

    public bool IsChecked { get { return _isChecked; } }

    private void Awake()
    {
        InitUIButton();
    }

    private void OnDisable()
    {
        if (_isSlotMenualUnbindEvent == false)
        {
            if (Button_Base != null)
            {
                Button_Base.onClick.RemoveAllListeners();
            }
        }
    }

    private void InitUIButton()
    {
        if (Button_Base != null)
        {
            return;
        }

        Button button = gameObject.GetComponentInChildren<Button>();
        if (button != null)
        {
            this.Button_Base = button;
        }
    }

    public void BindOnClickButtonEvent(Action onClickCallback, bool isMenualUnbineEvent = false)
    {
        if (Button_Base == null)
        {
            return;
        }

        Button_Base.onClick.AddListener(onClickCallback.Invoke);
        _isSlotMenualUnbindEvent = isMenualUnbineEvent;
    }

    public void UnBindAllOnClickButtonEvent()
    {
        if (Button_Base == null)
        {
            return;
        }

        Button_Base.onClick.RemoveAllListeners();
    }

    public void ChangeButtonText(string buttonStr)
    {
        if (Text_Base == null)
        {
            return;
        }

        Text_Base.text = buttonStr;
    }

    public void SetChecked(bool isChecked)
    {
        _isChecked = isChecked;

        if (Image_Select == null)
        {
            return;
        }

        Image_Select.gameObject.SetActive(isChecked);
    }
}
