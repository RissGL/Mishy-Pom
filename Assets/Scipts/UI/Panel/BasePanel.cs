using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BasePanel : MonoBehaviour
{
    protected CanvasGroup canvasGroup;
    private Vector3 originalScale;

    [SerializeField] private GameObject backGround; // 背景遮罩

    [Header("动画持续时间")]
    [SerializeField] protected float animDuration = 0.2f;

    public virtual void Show()
    {
        gameObject.SetActive(true);
        backGround.SetActive(true);

        transform.localScale =new Vector3(0.0f, 0.02f, 1f);

        Sequence sequence = DOTween.Sequence();

        sequence.Append( transform.DOScaleX(1f,animDuration*0.5f).SetEase(Ease.OutQuad).SetUpdate(true)).SetUpdate(true);
        sequence.Append( transform.DOScaleY(1f, animDuration).SetEase(Ease.OutBack).SetUpdate(true)).SetUpdate(true);
    }

    public virtual void Hide()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScaleY(0.007f, animDuration * 0.5f).SetEase(Ease.InCubic).SetUpdate(true)).SetUpdate(true);
        sequence.Append(transform.DOScaleX(0f, animDuration*0.4f).SetEase(Ease.InExpo).SetUpdate(true).SetUpdate(true));
        sequence.OnComplete(() =>
        {
            gameObject.SetActive(false);
            backGround.SetActive(false);
        });
    }

    private void OnDisable()
    {
        transform.DOKill();
         transform.localScale = originalScale;
    }
}
