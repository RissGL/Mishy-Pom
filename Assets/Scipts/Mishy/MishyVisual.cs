using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyVisual : MonoBehaviour
{
    [SerializeField] private Animator animator;

    // 记录当前的移动协程
    private Coroutine moveCoroutine;

    private float startSpeed=2f;
    [SerializeField]private float gravity=40f;

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

        transform.parent.localPosition = targetPos;
        moveCoroutine = null;

        //TODO :弹跳动画
    }

    /// <summary>
    /// 落地动画
    /// </summary>
    public void PlayLandEffects() 
    {

    }

    /// <summary>
    /// 消除动画
    /// </summary>
    public void PlayVanishEffects() 
    {
        
    }
}
