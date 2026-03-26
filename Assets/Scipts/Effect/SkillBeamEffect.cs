using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class SkillBeamEffect : MonoBehaviour
{
    private RawImage rawImage;
    private RectTransform rectTransform;

    [Header("纹理流动设置")]
    public float scrollSpeed = 2f; // 纹理滚动的速度

    [Header("动画设置")]
    public float animDuration = 0.3f; // 划入并变粗的时间
    public float stayDuration = 1f; // 在屏幕中间停留的时间

    [SerializeField] private Image Bg;

    // 预设位置和缩放
    private Vector2 startPos = new Vector2(-1500f, 0f); // 屏幕左侧外
    private Vector2 centerPos = new Vector2(0f, 0f);    // 屏幕中间

    private void Awake()
    {
        Bg.gameObject.SetActive(false);
        rawImage = GetComponent<RawImage>();
        rectTransform = GetComponent<RectTransform>();

        // 初始状态隐藏
        gameObject.SetActive(false);
    }

    private void Update()
    {
        Rect currentUV = rawImage.uvRect;
        currentUV.x -= scrollSpeed * Time.unscaledDeltaTime;
        rawImage.uvRect = currentUV;
    }

    /// <summary>
    /// 触发技能特效
    /// </summary>
    public void PlayEffect(Action skillAction)
    {
        Debug.Log("<color=yellow>特效触发指令已收到！</color>");
        gameObject.SetActive(true);
        StopAllCoroutines(); // 打断之前的动画
        StartCoroutine(BeamAnimationRoutine(skillAction));
    }

    private IEnumerator BeamAnimationRoutine(Action skillAction)
    {
        float elapsed = 0f;

        // 开启时停大招模式
        Time.timeScale = 0f;

        rectTransform.anchoredPosition = startPos;
        rectTransform.localScale = new Vector3(1f, 0.1f, 1f);

        if (Bg != null) Bg.gameObject.SetActive(true);

        // ================= 阶段一：划入屏幕 + 变粗 =================
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animDuration;

            float easeT = 1f - Mathf.Pow(1f - t, 3f);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, centerPos, easeT);
            float scaleY = Mathf.Lerp(0.1f, 1f, easeT);
            rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            yield return null;
        }

        // ================= 阶段二：边框死死定在中间不动 =================
        rectTransform.anchoredPosition = centerPos;
        rectTransform.localScale = Vector3.one;

        // 使用 Realtime 等待。等待期间由于 Update 还在跑，所以里面花纹会一直流！
        yield return new WaitForSecondsRealtime(stayDuration);

        skillAction();

        // ================= 阶段三：迅速变细消失 =================
        elapsed = 0f;
        while (elapsed < 0.15f)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / 0.15f;
            rectTransform.localScale = new Vector3(1f, Mathf.Lerp(1f, 0f, t), 1f);
            yield return null;
        }

        if (Bg != null) Bg.gameObject.SetActive(false);

        // 解除时停
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}