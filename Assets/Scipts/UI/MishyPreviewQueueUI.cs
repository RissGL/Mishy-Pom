using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 必须引入 UI

public class MishyPreviewQueueUI : MonoBehaviour
{
    [SerializeField] private PlayerBoard board;

    [SerializeField] private MishyDatabase database; // 引入数据库来获取外观
    [SerializeField] private RectTransform container; // 存放队列 UI 的父节点

    [Header("UI 动画设置")]
    [SerializeField] private float spacing = 100f;       // 每个咪西的上下间距
    [SerializeField] private float slideOffset = 200f;   // 侧边划入的 X 轴起始偏移量
    [SerializeField] private float animDuration = 0.3f;  // 动画时长
    [SerializeField] private float spriteSize = 100f;


    // 记录当前屏幕上活着的 UI 节点
    private List<GameObject> activeUINodes = new List<GameObject>();

    [SerializeField] private GameObject uiMishyPrefab;

    private void Awake()
    {
        board.previewQueue.OnNextMishyNeedSpawn += PreviewQueue_OnNextMishyNeedSpawn;
        board.previewQueue.OnNextMishyEequeue += PreviewQueue_OnNextMishyDequeue;
        board.previewQueue.OnPreviewQueueInit += PreviewQueue_OnPreviewQueueInit;
    }

    private void OnDestroy()
    {
        board.previewQueue.OnNextMishyNeedSpawn -= PreviewQueue_OnNextMishyNeedSpawn;
        board.previewQueue.OnNextMishyEequeue -= PreviewQueue_OnNextMishyDequeue;
        board.previewQueue.OnPreviewQueueInit -= PreviewQueue_OnPreviewQueueInit;
    }

    private void PreviewQueue_OnPreviewQueueInit(object sender, System.EventArgs e)
    {
        foreach (var node in activeUINodes)
        {
            if (node != null) Destroy(node);
        }
        activeUINodes.Clear();

        Queue<MishyType> initialQueue = board.previewQueue.GetAllNextMishy();
        int index = 0;
        foreach (var type in initialQueue)
        {
            GameObject uiNode = SpawnPureUINode(type);
            RectTransform rect = uiNode.GetComponent<RectTransform>();

            rect.anchoredPosition = new Vector2(0, -index * spacing);
            activeUINodes.Add(uiNode);
            index++;
        }
    }

    private void PreviewQueue_OnNextMishyDequeue(object sender, Queue<MishyType> newTwoMishies)
    {
        if (activeUINodes.Count >= 2)
        {
            GameObject top1 = activeUINodes[0];
            GameObject top2 = activeUINodes[1];

            activeUINodes.RemoveAt(0);
            activeUINodes.RemoveAt(0);

            StartCoroutine(AnimateOutRoutine(top1));
            StartCoroutine(AnimateOutRoutine(top2));
        }

        for (int i = 0; i < activeUINodes.Count; i++)
        {
            Vector2 targetPos = new Vector2(0, -i * spacing);
            StartCoroutine(AnimateShiftRoutine(activeUINodes[i], targetPos));
        }

        int startIndex = activeUINodes.Count;
        foreach (var type in newTwoMishies)
        {
            GameObject newNode = SpawnPureUINode(type);
            activeUINodes.Add(newNode);

            Vector2 targetPos = new Vector2(0, -startIndex * spacing);
            StartCoroutine(AnimateInRoutine(newNode, targetPos));
            startIndex++;
        }
    }

    private void PreviewQueue_OnNextMishyNeedSpawn(object sender, MishyPreviewQueue.MishyPairEventArgs e)
    {
        // 这里的逻辑已经合并在 OnNextMishyDequeue 里一并做完了，可以留空。
        // 因为队列推出和压入是同时发生的，统一在压入时更新视觉最连贯。
    }

    private GameObject SpawnPureUINode(MishyType type)
    {
        GameObject uiNode = Instantiate(uiMishyPrefab, container);
        uiNode.name = "UIPreview_" + type.ToString();

        Image img = uiNode.GetComponent<Image>();

        Sprite sprite = database.GetSprite(type);
        if (sprite != null)
        {
            img.sprite = sprite;
            img.color = database.GetColor(type);
        }

        return uiNode;
    }

    #region 动画协程库 (使用 Cubic Ease-Out 平滑曲线)
    private IEnumerator AnimateOutRoutine(GameObject node)
    {
        RectTransform rect = node.GetComponent<RectTransform>();
        CanvasGroup cg = node.GetComponent<CanvasGroup>();

        Vector2 startPos = rect.anchoredPosition;
        // 目标位置：往上飞出两个身位
        Vector2 targetPos = startPos + new Vector2(0, spacing * 2);

        float elapsed = 0;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animDuration;
            float easeT = 1f - Mathf.Pow(1f - t, 3f); // 让动画起步快，结尾柔和

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, easeT);
            cg.alpha = Mathf.Lerp(1f, 0f, t*2); // 透明度 255 -> 0 (在 CanvasGroup 里是 1 -> 0)

            yield return null;
        }

        // 动画播完后彻底销毁
        Destroy(node);
    }

    private IEnumerator AnimateShiftRoutine(GameObject node, Vector2 targetPos)
    {
        RectTransform rect = node.GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;

        float elapsed = 0;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animDuration;
            float easeT = 1f - Mathf.Pow(1f - t, 3f);

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, easeT);
            yield return null;
        }

        rect.anchoredPosition = targetPos;
    }

    private IEnumerator AnimateInRoutine(GameObject node, Vector2 targetPos)
    {
        RectTransform rect = node.GetComponent<RectTransform>();
        CanvasGroup cg = node.GetComponent<CanvasGroup>();

        // 起点设在目标位置的正右方 (侧边划入)
        Vector2 startPos = targetPos + new Vector2(slideOffset, 0);
        rect.anchoredPosition = startPos;
        cg.alpha = 0f; // 初始全透明

        float elapsed = 0;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animDuration;
            float easeT = 1f - Mathf.Pow(1f - t, 3f);

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, easeT);
            cg.alpha = Mathf.Lerp(0f, 1f, t); // 逐渐显示出来

            yield return null;
        }

        rect.anchoredPosition = targetPos;
        cg.alpha = 1f;
    }

    #endregion
}