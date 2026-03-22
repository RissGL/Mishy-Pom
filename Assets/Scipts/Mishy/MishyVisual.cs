using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private bool isVanishing = false;

    // 记录当前的移动协程
    private Coroutine moveCoroutine;

    private float startSpeed=2f;
    [SerializeField]private float gravity=40f;

    private const string VANISH_TRIGGER = "Vanish";
    private const string FALL_OVER_TRIGGER = "FallOver";


    public void DownMoveAni(Transform rootTransform, GridPosition targetGridPosition) 
    {
        Vector3 targetPosition=GridManager.Instance.GetWorldPosition(targetGridPosition);

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        StartCoroutine(MoveToTargetRoutine(rootTransform,targetPosition));
    }

    private IEnumerator MoveToTargetRoutine(Transform rootTransform,Vector3 targetPos)
    {
        float currentSpeed = startSpeed;

        while (Vector3.Distance(targetPos, rootTransform.localPosition) > 0.01f)
        {
            currentSpeed += Time.deltaTime * gravity;

            rootTransform.localPosition = Vector3.MoveTowards(
                rootTransform.localPosition,
                targetPos,
                currentSpeed * Time.deltaTime
            );

            yield return null;
        }

        rootTransform.localPosition = targetPos;
        moveCoroutine = null;

        //TODO :弹跳动画
        PlayLandEffects();
    }

    /// <summary>
    /// 落地动画
    /// </summary>
    public void PlayLandEffects() 
    {
        animator.SetTrigger(FALL_OVER_TRIGGER);
    }

    /// <summary>
    /// 消除动画
    /// </summary>
    public void PlayVanishEffects() 
    {
        isVanishing = true;
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        //中断掉落动画
        animator.ResetTrigger(FALL_OVER_TRIGGER);

        animator.SetTrigger(VANISH_TRIGGER);

        GameObject anchor = new GameObject("Vanish_Anchor");

        anchor.transform.position = transform.position;

        //取消父子关系
        transform.SetParent(null);

        transform.SetParent(anchor.transform);
        Destroy(gameObject,0.6f);
    }
}
