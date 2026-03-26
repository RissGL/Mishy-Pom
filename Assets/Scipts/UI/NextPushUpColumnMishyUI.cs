using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.UI;

public class NextPushUpColumnMishyUI : MonoBehaviour
{
    [SerializeField] private PlayerBoard board;

    [SerializeField] private MishyDatabase database; // 引入数据库来获取外观
    [SerializeField] private RectTransform container; // 存放队列 UI 的父节点

    [SerializeField] private float spriteSize=108f;

    [SerializeField] private float animDuration = 0.3f;  // 动画时长
    [SerializeField] private GameObject uiMishyPrefab;

    private void Awake()
    {
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

    private IEnumerator AnimateOutRoutine(GameObject node) 
    {
        RectTransform rect=node.GetComponent<RectTransform>();
        CanvasGroup cg=node.GetComponent<CanvasGroup>();

        float elapsed = 0;
        Vector2 startPos=rect.anchoredPosition;
        Vector2 targetPosition=startPos+new Vector2(0,spriteSize);

        while (elapsed < animDuration) 
        {
            elapsed +=Time.deltaTime;
            float t=elapsed / animDuration;
            float easeT = 1f - Mathf.Pow(1f - t, 3f); 

            rect.anchoredPosition=Vector2.Lerp(startPos, targetPosition, easeT);
            cg.alpha = Mathf.Lerp(1f, 0f, t);

            yield return null;
        }

        Destroy(node);
    }


    private void OnDestroy()
    {
        board.pushUpColumn.OnUpdateSingleRow -= NextPushUpColumnMishy_OnUpdateSingleRow;
    }

}
