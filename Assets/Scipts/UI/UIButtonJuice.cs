using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.1f;    // 悬停时放大的倍数
    [SerializeField] private float pressScale = 0.9f;    // 按下时挤压的倍数
    [SerializeField] private float animDuration = 0.2f;  // 动画弹性时间

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        transform.DOKill();

        transform.DOScale(originalScale * pressScale, 0.1f).SetEase(Ease.OutQuad);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOKill();

        transform.DOScale(originalScale*hoverScale,animDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale, animDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(originalScale*hoverScale, animDuration).SetEase(Ease.OutBack);
    }

    private void OnDisable()
    {
        transform.DOKill();

        transform.localScale = originalScale;
    }
}
