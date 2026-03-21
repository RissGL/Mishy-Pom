using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyPlayerController : MonoBehaviour
{
    private Mishy mishy_One;
    private Mishy mishy_Two;
    private bool isActive;

    [Header("下落节奏设置")]
    [SerializeField] private float fallInterval = 1.0f;     // 正常下落速度
    [SerializeField] private float fastFallInterval = 0.05f;// 加速下落速度
    private float fallTimer;
    private float currentFallInterval;

    public void SetActivePair(Mishy mishy_One, Mishy mishy_Two)
    {
        this.mishy_One = mishy_One;
        this.mishy_Two = mishy_Two;
        isActive = true;
    }

    private void Update()
    {
        if (!isActive) 
            return;

        if (Input.GetKeyDown(KeyCode.A))
        {
            TryMove(new GridPosition(-1,0));

            //TODO: 检测是否能动，不能未来要触发错误音效
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            TryMove(new GridPosition(1, 0));

            //TODO: 检测是否能动，不能未来要触发错误音效
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SwapMishies();
        }

        if (Input.GetKeyDown(KeyCode.S)) currentFallInterval = fastFallInterval;
        if (Input.GetKeyUp(KeyCode.S)) currentFallInterval = fallInterval;

        fallTimer += Time.deltaTime;
        if (fallTimer >= currentFallInterval)
        {
            fallTimer = 0f;
            TryMoveDown(); // 逻辑上一格一格掉
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Vector3 targetPosOne = GridManager.Instance.GetWorldPosition(mishy_One.GetGridPosition());
        Vector3 targetPosTwo = GridManager.Instance.GetWorldPosition(mishy_Two.GetGridPosition());

        // 使用 Lerp 平滑插值 (15f 是速度，可以根据手感微调)
        mishy_One.transform.localPosition = Vector3.Lerp(mishy_One.transform.localPosition, targetPosOne, Time.deltaTime * 15f);
        mishy_Two.transform.localPosition = Vector3.Lerp(mishy_Two.transform.localPosition, targetPosTwo, Time.deltaTime * 15f);
    }

    private void TryMove(GridPosition moveDir) 
    {
        //咪西一永远在下面
        GridPosition nextPosOne = mishy_One.GetGridPosition() + moveDir;
        GridPosition nextPosTwo = mishy_Two.GetGridPosition() + moveDir;

        if (CanOccupy(nextPosOne) && CanOccupy(nextPosTwo))
        {
            ExecuteMove(nextPosOne, nextPosTwo);
            // TODO: 播放平移音效 (Swoosh)
        }
        else
        {
            // TODO: 播放撞墙报错音效 (Error Buzzer)
        }
    }

    public void TryMoveDown() 
    {
        GridPosition downDir = new GridPosition(0, -1);
        GridPosition nextPosOne = mishy_One.GetGridPosition() + downDir;
        GridPosition nextPosTwo = mishy_Two.GetGridPosition() + downDir;

        // 向下移动合法，继续下落
        if (CanOccupy(nextPosOne) && CanOccupy(nextPosTwo))
        {
            ExecuteMove(nextPosOne, nextPosTwo);
        }
        else
        {
            // 被挡住了，或者碰到底了 -> 锁定并结算
            LockAndSettle();
        }
    }

    /// <summary>
    /// 能否占据格子
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool CanOccupy(GridPosition pos) 
    {
        if (!GridManager.Instance.IsValidGridPosition(pos))
            return false;

        if (GridManager.Instance.HasMishy(pos))
            return false;

        return true;
    }

    /// <summary>
    /// 更新咪西的网格位置
    /// </summary>
    /// <param name="posOne"></param>
    /// <param name="posTwo"></param>
    private void ExecuteMove(GridPosition posOne, GridPosition posTwo)
    {
        mishy_One.UpdateGridPosition(posOne);
        mishy_Two.UpdateGridPosition(posTwo);
    }

    private void SwapMishies() 
    {
        Vector3 temp = mishy_One.transform.localPosition;
        mishy_One.transform.localPosition = mishy_Two.transform.localPosition;
        mishy_Two.transform.localPosition=temp;

        GridPosition tempGrid = mishy_One.GetGridPosition();
        mishy_One.UpdateGridPosition(mishy_Two.GetGridPosition());
        mishy_Two.UpdateGridPosition(tempGrid);

        //交换引用，确保咪西一是下面的咪西
        Mishy tempMishy=mishy_One;
        mishy_One = mishy_Two;
        mishy_Two = tempMishy;
    }

    private void LockAndSettle()
    {
        isActive = false;

        GridManager.Instance.SetGridMishy(mishy_One.GetGridPosition(), mishy_One);
        GridManager.Instance.SetGridMishy(mishy_Two.GetGridPosition(), mishy_Two);
    }
}
