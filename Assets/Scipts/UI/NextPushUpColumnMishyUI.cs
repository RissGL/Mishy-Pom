using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool; // 引入内置对象池命名空间
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.UI;

public class NextPushUpColumnMishyUI : MonoBehaviour
{
    [SerializeField] private PlayerBoard board;

    [SerializeField] private MishyDatabase database; // 引入数据库来获取外观
    [SerializeField] private RectTransform container; // 存放队列 UI 的父节点

    [SerializeField] private float spriteSize = 108f;

    [SerializeField] private float animDuration = 0.3f;  // 动画时长
    [SerializeField] private GameObject uiMishyPrefab;

    // 声明对象池
    private IObjectPool<GameObject> nodePool;

    private void Awake()
    {
        // 初始化对象池
        nodePool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(uiMishyPrefab, container),
            actionOnGet: (obj) =>
            {
                obj.SetActive(true);
                // 【关键重置 1】飞出动画将其移出了 container，复用时必须移回来
                obj.transform.SetParent(container, false);

                // 【关键重置 2】恢复透明度，因为飞出动画把它变成了 0
                CanvasGroup cg = obj.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 1f;
            },
            actionOnRelease: (obj) =>
            {
                obj.SetActive(false);
            },
            actionOnDestroy: (obj) =>
            {
                Destroy(obj);
            },
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 30
        );

        board.pushUpColumn.OnUpdateSingleRow += NextPushUpColumnMishy_OnUpdateSingleRow;
    }

    private void NextPushUpColumnMishy_OnUpdateSingleRow(object sender, MishyType[] newRow)
    {
        List<Transform> oldChildren = new List<Transform>();
        foreach (Transform child in container)
        {
            oldChildren.Add(child);
        }

        foreach (Transform oldChild in oldChildren)
        {
            oldChild.SetParent(container.parent, true);
            StartCoroutine(AnimateOutRoutine(oldChild.gameObject));
        }

        for (int i = 0; i < newRow.Length; i++)
        {
            SpawnPureUINode(newRow[i]);
        }
    }

    private GameObject SpawnPureUINode(MishyType type)
    {
        // 【关键】从对象池获取，而不是 Instantiate
        GameObject uiNode = nodePool.Get();
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

    private IEnumerator AnimateOutRoutine(GameObject node)
    {
        RectTransform rect = node.GetComponent<RectTransform>();
        CanvasGroup cg = node.GetComponent<CanvasGroup>();

        float elapsed = 0;
        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPosition = startPos + new Vector2(0, spriteSize);

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animDuration;
            float easeT = 1f - Mathf.Pow(1f - t, 3f);

            rect.anchoredPosition = Vector2.Lerp(startPos, targetPosition, easeT);
            cg.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        // 【关键】动画结束后，归还给对象池，而不是 Destroy
        nodePool.Release(node);
    }

    private void OnDestroy()
    {
        board.pushUpColumn.OnUpdateSingleRow -= NextPushUpColumnMishy_OnUpdateSingleRow;
    }

}