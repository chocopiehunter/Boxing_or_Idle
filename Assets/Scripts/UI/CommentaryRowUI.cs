using UnityEngine;
using UnityEngine.UI;

public class CommentaryRowUI : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform_Row;
    [SerializeField] private CanvasGroup CanvasGroup_Row;
    [SerializeField] private LayoutElement LayoutElement_Row;
    [SerializeField] private Text Text_Commentary;

    [SerializeField] private float MinHeight = 50f;
    [SerializeField] private float VerticalPadding = 10f;

    private float _targetY;
    public float Height
    {
        get
        {
            return RectTransform_Row.rect.height;
        }
    }

    public void Setup(string message)
    {

    }

    public void UpdatePosition()
    {

    }

    public void ResetRow()
    {

    }
}
