using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RawImage))]
public class SkillBeamEffect : MonoBehaviour
{
    private RawImage rawImage;
    private RectTransform rectTransform;

    [Header("流动速度")]
    [SerializeField] private float scrollSpeed = -3f;

    [Header("动画设置")]
    [SerializeField] private float animDuration = 0.15f;
    [SerializeField] private float stayDuration = 0.8f;

    [SerializeField] private Image Bg;

    private Vector2 startPos = new Vector2(-2000f, 0f);
    private Vector2 centerPos = new Vector2(0f, 0f);

    private void Awake()
    {
        if (Bg != null) Bg.gameObject.SetActive(false);
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();

        gameObject.SetActive(false);
    }

    private void Update()
    {
        // 使用原生 uvRect 滚动，UI 系统会自动刷新顶点 UV 给 Shader
        if (rawImage != null)
        {
            Rect currentUV = rawImage.uvRect;
            currentUV.x += scrollSpeed * Time.unscaledDeltaTime;
            rawImage.uvRect = currentUV;
        }
    }

    public void PlayEffect(Action skillAction)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();

        rectTransform.DOKill();
        if (Bg != null) Bg.DOKill();

        StartCoroutine(BeamAnimationRoutine(skillAction));
    }

    private IEnumerator BeamAnimationRoutine(Action skillAction)
    {
        // 1. 时间停止
        Time.timeScale = 0f;

        // 2. 初始状态：极细的一条线
        rectTransform.anchoredPosition = startPos;
        rectTransform.localScale = new Vector3(1f, 0.02f, 1f);

        if (Bg != null)
        {
            Bg.gameObject.SetActive(true);
            Bg.color = new Color(0, 0, 0, 0);
            Bg.DOFade(0.7f, 0.1f).SetUpdate(true);
        }

        // 3. 极速划入并爆开变粗
        rectTransform.DOAnchorPos(centerPos, animDuration).SetEase(Ease.OutExpo).SetUpdate(true);
        rectTransform.DOScaleY(1.0f, 0.2f).SetEase(Ease.OutBack, 2.5f).SetUpdate(true);

        if (CameraShakeManager.instance != null) CameraShakeManager.instance.ShakeHeavy();

        // 4. 等待能量倾泻，此时 Update 里的 uvRect 正在疯狂滚动
        yield return new WaitForSecondsRealtime(stayDuration);

        // 5. 执行技能消除
        skillAction();

        if (CameraShakeManager.instance != null) CameraShakeManager.instance.ShakeMedium();

        // 6. 干净利落地压扁变细，不带溶解
        rectTransform.DOScaleY(0f, 0.15f).SetEase(Ease.InCubic).SetUpdate(true);

        if (Bg != null) Bg.DOFade(0f, 0.15f).SetUpdate(true);

        yield return new WaitForSecondsRealtime(0.15f);

        // 7. 恢复时间与清理
        if (Bg != null) Bg.gameObject.SetActive(false);
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }
}