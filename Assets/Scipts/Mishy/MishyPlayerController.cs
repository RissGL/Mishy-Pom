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
    [SerializeField] private float autoFastFallDelay = 0.18f; // 缓冲时间

    [SerializeField] private GameObject promptBar;

    private float fallTimer;
    private float currentFallInterval;
    private float autoFastFallTimer;//缓冲时间计时器

    private Vector3 ghostVector3Top;
    private Vector3 ghostVector3Bottom;

    private float swapTimer=0f;
    [SerializeField]private float swapDuration=0.15f;

    private float moveSpeed=30f;//左右移动的速度

    public void SetActivePair(Mishy mishy_One, Mishy mishy_Two)
    {
        this.mishy_One = mishy_One;
        this.mishy_Two = mishy_Two;
        isActive = true;

        currentFallInterval = fallInterval;
        fallTimer = currentFallInterval;
        autoFastFallTimer = autoFastFallDelay;

        promptBar.SetActive(true);
        promptBar.transform.position = GridManager.Instance.GetWorldPosition
            (new GridPosition(MishyManager.Instance.GetSpawnX(),0));

        ghostVector3Bottom = GridManager.Instance.GetWorldPosition(mishy_One.GetGridPosition());
        ghostVector3Top = GridManager.Instance.GetWorldPosition(mishy_Two.GetGridPosition());

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

        if (Input.GetKeyDown(KeyCode.S))
        {
            autoFastFallTimer = 0;
        }


        if (autoFastFallTimer>0f)
        {
            autoFastFallTimer-=Time.deltaTime;
        }
        bool isHoldingS = Input.GetKey(KeyCode.S)&&autoFastFallTimer<=0;
        float targetFallInterval =isHoldingS ? fastFallInterval : fallInterval;

        //利用百分比同步下落进度
        if (currentFallInterval != targetFallInterval)
        {
            float process=fallTimer/currentFallInterval;
            currentFallInterval=targetFallInterval;
            fallTimer=currentFallInterval*process;
        }

        /*if (Input.GetKeyDown(KeyCode.S))
        {
            currentFallInterval = fastFallInterval;
            if (fallTimer > currentFallInterval) 
            {
                fallTimer = currentFallInterval; 
            }
        }
        if (Input.GetKeyUp(KeyCode.S)) currentFallInterval = fallInterval;*/


        fallTimer += Time.deltaTime;
        while (fallTimer >= currentFallInterval)
        {
            fallTimer -= currentFallInterval;
            TryMoveDown(); // 逻辑上一格一格掉

            if (!isActive)
            {
                break;
            }
        }

        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        Vector3 targetPosOne = GridManager.Instance.GetWorldPosition(mishy_One.GetGridPosition());
        Vector3 targetPosTwo = GridManager.Instance.GetWorldPosition(mishy_Two.GetGridPosition());

        float fallSpeed = (float)GridManager.Instance.GetCellSize() / currentFallInterval;

        ghostVector3Top.x = Mathf.Lerp(ghostVector3Top.x, targetPosTwo.x, Time.deltaTime * moveSpeed);
        ghostVector3Top.y= Mathf.MoveTowards(ghostVector3Top.y, targetPosTwo.y, Time.deltaTime * fallSpeed);

        ghostVector3Bottom.x = Mathf.Lerp(ghostVector3Bottom.x, targetPosOne.x, Time.deltaTime * moveSpeed);
        ghostVector3Bottom.y = Mathf.MoveTowards(ghostVector3Bottom.y, targetPosOne.y, Time.deltaTime * fallSpeed);

        if (swapTimer > 0f)
        {
            swapTimer -= Time.deltaTime;
            float t = 1.0f - swapTimer / swapDuration;

            float arc = 0.7f;//转的弧度
            float arcOffset = (float)GridManager.Instance.GetCellSize()*arc*Mathf.Sin(Mathf.PI * t);
            Vector3 posOne = Vector3.Lerp(ghostVector3Top, ghostVector3Bottom, t);
            posOne.x += arcOffset; 
            mishy_One.transform.localPosition = posOne;

            Vector3 posTwo = Vector3.Lerp(ghostVector3Bottom, ghostVector3Top, t);
            posTwo.x -= arcOffset;
            mishy_Two.transform.localPosition = posTwo;
        }
        else 
        {
            mishy_One.transform.localPosition = ghostVector3Bottom;
            mishy_Two.transform.localPosition = ghostVector3Top;
        }

        /*
        // 咪西一
        Vector3 posOne = mishy_One.transform.localPosition;
        posOne.x = Mathf.Lerp(posOne.x, targetPosOne.x, Time.deltaTime * moveSpeed);
        posOne.y = Mathf.MoveTowards(posOne.y, targetPosOne.y, fallSpeed * Time.deltaTime);
        mishy_One.transform.localPosition = posOne;

        // 咪西二
        Vector3 posTwo = mishy_Two.transform.localPosition;
        posTwo.x = Mathf.Lerp(posTwo.x, targetPosTwo.x, Time.deltaTime * moveSpeed);
        posTwo.y = Mathf.MoveTowards(posTwo.y, targetPosTwo.y, fallSpeed * Time.deltaTime);
        mishy_Two.transform.localPosition = posTwo;
        */
        
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

            promptBar.transform.position =GridManager.Instance.GetWorldPosition( new GridPosition(nextPosOne.x, 0));

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
            // 被挡住或者碰到底 -> 锁定并结算
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
        /*
        Vector3 temp = mishy_One.transform.localPosition;
        mishy_One.transform.localPosition = mishy_Two.transform.localPosition;
        mishy_Two.transform.localPosition=temp;
        */

        GridPosition tempGrid = mishy_One.GetGridPosition();
        mishy_One.UpdateGridPosition(mishy_Two.GetGridPosition());
        mishy_Two.UpdateGridPosition(tempGrid);

        //交换引用，确保咪西一是下面的咪西
        Mishy tempMishy=mishy_One;
        mishy_One = mishy_Two;
        mishy_Two = tempMishy;

        swapTimer = swapDuration;
    }

    private void LockAndSettle()
    {
        isActive = false;

        mishy_One.transform.localPosition = GridManager.Instance.GetWorldPosition(mishy_One.GetGridPosition());
        mishy_Two.transform.localPosition = GridManager.Instance.GetWorldPosition(mishy_Two.GetGridPosition());

        mishy_One.PlayLandAni();
        mishy_Two.PlayLandAni();

        promptBar.gameObject.SetActive(false);

        GridManager.Instance.SetGridMishy(mishy_One.GetGridPosition(), mishy_One);
        GridManager.Instance.SetGridMishy(mishy_Two.GetGridPosition(), mishy_Two);

        FindObjectOfType<MatchSystem>().StartMatchSequence();
    }


    /// <summary>
    /// 中断当前的下落，销毁物体，并返回它们的类型
    /// </summary>
    public MishyType[] InterruptAndClearActivePair() 
    {
        if (isActive==false||mishy_One==null||mishy_Two==null) 
        {
            return null;
        }

        isActive = false;

        promptBar.SetActive(false);

        MishyType[] mishyTypes=new MishyType[2];

        mishyTypes[0]=mishy_One.GetMishyType();
        mishyTypes[1] = mishy_Two.GetMishyType();

        Destroy(mishy_One.gameObject);
        Destroy(mishy_Two.gameObject);

        // 清空引用
        mishy_One = null;
        mishy_Two = null;

        return mishyTypes;
    }
}
