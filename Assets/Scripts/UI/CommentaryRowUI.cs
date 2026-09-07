using System;
using DG.Tweening;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class CommentaryRowUI : MonoBehaviour
{
    [SerializeField] private RectTransform RectTransform_Row;
    [SerializeField] private CanvasGroup CanvasGroup_Row;
    [SerializeField] private LayoutElement LayoutElement_Row;
    [SerializeField] private Text Text_Commentary;

    [SerializeField] private MMF_Player Feedbacks_Show;

    [SerializeField] private float MinHeight = 50f;
    [SerializeField] private float VerticalPadding = 10f;
    [SerializeField] private float MoveDuration = 0.25f;
    [SerializeField] private float FadeDuration = 1.25f;
    [SerializeField] private Ease MoveEase = Ease.OutCubic;
    [SerializeField] private Ease FadeEase = Ease.Linear;

    private Tween _moveTween;
    private Tween _fadeTween;
    private Action<CommentaryRowUI> _onFadeCompleted;

    public float Height
    {
        get
        {
            return RectTransform_Row.rect.height;
        }
    }

    public void Setup(string message)
    {
        StopTweens();

        if (Feedbacks_Show != null)
        {
            Feedbacks_Show.StopFeedbacks();
        }

        CanvasGroup_Row.alpha = 1f;

        Text_Commentary.text = message;

        float preferredHeight = Text_Commentary.preferredHeight + VerticalPadding;

        float rowHeight = Mathf.Max(MinHeight, preferredHeight);

        LayoutElement_Row.preferredHeight = rowHeight;

        RectTransform_Row.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rowHeight);

        if (Feedbacks_Show != null)
        {
            Feedbacks_Show.PlayFeedbacks();
        }
    }

    public void SetPositionImmediately(float targetY)
    {
        StopMoveTween();

        Vector2 position = RectTransform_Row.anchoredPosition;

        position.y = targetY;

        RectTransform_Row.anchoredPosition = position;
    }

    public void MoveTo(float targetY)
    {
        StopMoveTween();

        _moveTween = RectTransform_Row.DOAnchorPosY(targetY, MoveDuration).SetEase(MoveEase).SetUpdate(true).SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }

    public void FadeOut(Action<CommentaryRowUI> onFadeCompleted)
    {
        StopFadeTween();

        _onFadeCompleted = onFadeCompleted;

        _fadeTween = CanvasGroup_Row.DOFade(0f, FadeDuration).SetEase(FadeEase).SetUpdate(true).SetLink(gameObject, LinkBehaviour.KillOnDisable).OnComplete(OnFadeCompleted);
    }

    public void ResetRow()
    {
        StopTweens();

        if (Feedbacks_Show != null)
        {
            Feedbacks_Show.StopFeedbacks();
        }

        CanvasGroup_Row.alpha = 1f;

        Text_Commentary.text = "";

        Vector2 position = RectTransform_Row.anchoredPosition;

        position.y = 0f;

        RectTransform_Row.anchoredPosition = position;
    }

    private void OnFadeCompleted()
    {
        Action<CommentaryRowUI> onFadeCompleted = _onFadeCompleted;

        _fadeTween = null;

        _onFadeCompleted = null;

        if (onFadeCompleted != null)
        {
            onFadeCompleted(this);
        }
    }

    private void StopTweens()
    {
        StopMoveTween();
        StopFadeTween();
    }

    private void StopMoveTween()
    {
        if (_moveTween == null)
        {
            return;
        }

        _moveTween.Kill();

        _moveTween = null;
    }

    private void StopFadeTween()
    {
        if (_fadeTween != null)
        {
            _fadeTween.Kill();

            _fadeTween = null;
        }

        _onFadeCompleted = null;
    }

    private void OnDisable()
    {
        StopTweens();

        if (Feedbacks_Show != null)
        {
            Feedbacks_Show.StopFeedbacks();
        }
    }
}
