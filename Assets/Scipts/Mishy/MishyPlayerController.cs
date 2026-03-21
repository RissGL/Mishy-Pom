using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyPlayerController : MonoBehaviour
{
    private Mishy mishy_One;
    private Mishy mishy_Two;
    private bool isActive;

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
    }

    private bool TryMove(GridPosition moveDir) 
    {
        //咪西一永远在下面
        if (GridManager.Instance.CanMoveWithGridPosition(new GridPosition[2] 
        { mishy_One.GetGridPosition(), mishy_Two.GetGridPosition() },moveDir))
        {
            GridManager.Instance.MoveMishyWithGridPosition(mishy_One.GetGridPosition(), moveDir, mishy_One);
            GridManager.Instance.MoveMishyWithGridPosition(mishy_Two.GetGridPosition(), moveDir, mishy_Two);
            //TODO :移动音效

            return true;
        }
        else
        {
            //TODO :移动失败音效

            return false;
        }

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
}
