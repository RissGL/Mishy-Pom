using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MishyManager : MonoBehaviour
{
    public static MishyManager Instance { get; private set; }

    //private GridSystem<GridObject> gridSystem;

    [Header("存放咪西的父节点")]
    [SerializeField] private Transform mishyContainer;   // 场景中存放咪西的父节点

    [Header("咪西SO配置文件")]
    [SerializeField] private MishyDatabase mishyDatabase; //SO 文件

    [Header("咪西预览队列 竖")]
    [SerializeField] private MishyPreviewQueue mishyPreviewQueue;


    [Header("下落的咪西")]
    [SerializeField] private MishyPlayerController mishyPlayerController;

    [Header("底部的咪西")]
    [SerializeField] private NextPushUpColumnMishy mishyNextPushUpColumn;

    [Header("消除系统")]
    [SerializeField] private MatchSystem mishyMatchSystem;

    [Header("生存压力")]
    [SerializeField] private float basePushUpChance = 10f;    //基础概率
    [SerializeField] private float maxPushUpChance = 20f;     //最大推上来的概率
    [SerializeField] private float chanceUpPerHalfMinute = 2.5f;//每半分钟增加的概率
    [SerializeField] private float gameTimer = 0f;            //游戏时长记时

    private GridPosition SpawnGridPositionUp;
    private GridPosition SpawnGridPositionDown = new GridPosition(3, 12);

    private MishyType[] currentMishyPairType = null;

    private int thisTurnUpCount=0;

    public event EventHandler<int> OnTurnPushNumUpdate;//更新推上来行数UI用的事件


    private void Update()
    {
        gameTimer += Time.deltaTime;
    }

    public int GetSpawnX()
    {
        return SpawnGridPositionDown.x;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("已经存在GridManager实例");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        mishyPreviewQueue.OnNextMishyNeedSpawn += MishyPreviewQueue_OnNextMishyNeedSpawn;
        mishyNextPushUpColumn.OnMultiMishyPushUp += NextPushUpColumnMishy_OnMishyPushUp;

        SkillSystem.OnSkillUse += SkillSystem_OnSkillUse;
        SpawnGridPositionUp = new GridPosition(SpawnGridPositionDown.x, SpawnGridPositionDown.y + 1);
        StartCoroutine(AsyncSpawnFirstMishyPair());
        //TODO:改成等待玩家确定后再生成第一个
        MishyColumnUp(4);
    }

    private void SkillSystem_OnSkillUse(object sender, SkillSystem.SkillUseInfo e)
    {
        if (e.isUpToEnemy)
        {
            mishyNextPushUpColumn.PushMultiColumnMishyUp(e.matchColumnCount);
        }
        else
        {
            currentMishyPairType = mishyPlayerController.InterruptAndClearActivePair();
            mishyMatchSystem.StartMatchBottomColumnMishy(e.matchColumnCount);
        }
    }

    private void OnDestroy()
    {
        mishyPreviewQueue.OnNextMishyNeedSpawn -= MishyPreviewQueue_OnNextMishyNeedSpawn;
        mishyNextPushUpColumn.OnMultiMishyPushUp -= NextPushUpColumnMishy_OnMishyPushUp;
        SkillSystem.OnSkillUse -= SkillSystem_OnSkillUse;
    }

    private IEnumerator AsyncSpawnFirstMishyPair()
    {
        mishyPreviewQueue.DequeueNextMishy();
        yield return null;
    }

    private void MishyPreviewQueue_OnNextMishyNeedSpawn(object sender, MishyPreviewQueue.MishyPairEventArgs e)
    {
        Mishy mishy_One = SpawnMishy(SpawnGridPositionDown, e.type_one);
        Mishy mishy_Two = SpawnMishy(SpawnGridPositionUp, e.type_two);

        mishyPlayerController.SetActivePair(mishy_One, mishy_Two);
    }

    /// <summary>
    /// 底部咪西往上推
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void NextPushUpColumnMishy_OnMishyPushUp(object sender, MishyType[][] newRows)
    {
        currentMishyPairType = mishyPlayerController.InterruptAndClearActivePair();

        SpawnMishyOnButtom(newRows);

    }

    /// <summary>
    /// 延迟结算
    /// </summary>
    /// <returns></returns>
    public IEnumerator DelayedSettlement()
    {
        //该处逻辑可能要修改，不太行
        yield return new WaitUntil(() => !CheckAllMishyMoveState());

        //短暂停留一下
        yield return new WaitForSeconds(0.1f);

        mishyMatchSystem.StartMatchSequence();
    }


    /// <summary>
    /// 底部生成咪西
    /// </summary>
    /// <param name="rowCount"></param>
    public void SpawnMishyOnButtom(MishyType[][] newRows)
    {
        int rowCount = newRows.Length;
        PushAllMishyUp(rowCount);
        for (int y = 0; y < rowCount; y++)
        {
            for (int x = 0; x < GridManager.Instance.GetWidth(); x++)
            {
                GridPosition targetPos = new GridPosition(x, y);

                Mishy mishy = SpawnAndSetMishy(new GridPosition(x, y), newRows[y][x]);

                GridPosition startVisualPos = new GridPosition(x, y - rowCount);
                mishy.transform.localPosition = GridManager.Instance.GetWorldPosition(startVisualPos);

                float delay =GetRandomDelay(x,y);

                //float delay = UnityEngine.Random.Range(0,0.15f);
                mishy.PlayPushUpAni(targetPos, delay);
            }
        }
        StartCoroutine(DelayedSettlement());
    }

    public Mishy SpawnMishy(GridPosition gridPosition,MishyType mishyType)
    {
        GameObject mishyPrefab= mishyDatabase.GetPrefab(mishyType);
        if(mishyPrefab==null)
            return null;

        GameObject mishyTransform=Instantiate(mishyPrefab,mishyContainer);
        mishyTransform.transform.localPosition = GridManager.Instance.GetWorldPosition(gridPosition);

        Mishy mishy = mishyTransform.GetComponent<Mishy>();
        mishy.SetUp(gridPosition, mishyType);
        mishy.OnMishyLanded += (landedMishy) => 
        {
            GridPosition finalPosition = landedMishy.GetGridPosition();

            if (!GridManager.Instance.HasMishy(finalPosition))
            {
                GridManager.Instance.SetGridMishy(landedMishy.GetGridPosition(), landedMishy);
                // TODO: 消除检查逻辑
            }
        };

        return mishy;
    }

    public Mishy SpawnAndSetMishy(GridPosition gridPosition, MishyType mishyType)
    {
        GameObject mishyPrefab = mishyDatabase.GetPrefab(mishyType);
        if (mishyPrefab == null)
            return null;

        GameObject mishyTransform = Instantiate(mishyPrefab, mishyContainer);
        mishyTransform.transform.localPosition = GridManager.Instance.GetWorldPosition(gridPosition);

        Mishy mishy = mishyTransform.GetComponent<Mishy>();
        mishy.SetUp(gridPosition, mishyType);
        mishy.OnMishyLanded += (landedMishy) =>
        {
            GridPosition finalPosition = landedMishy.GetGridPosition();

            if (!GridManager.Instance.HasMishy(finalPosition))
            {
                GridManager.Instance.SetGridMishy(landedMishy.GetGridPosition(), landedMishy);
                // TODO: 消除检查逻辑
            }
        };

        GridManager.Instance.SetGridMishy(gridPosition, mishy);
        return mishy;
    }

    /// <summary>
    /// 临时得到所有咪西
    /// </summary>
    /// <returns></returns>
    public List<Mishy> GetAllMishies() 
    {
        List<Mishy> mishies = new List<Mishy>();

        for (int x = 0; x < GridManager.Instance.GetWidth(); x++) 
        {
            for (int y = 0; y < GridManager.Instance.GetHeight(); y++)
            {
                if (GridManager.Instance.TryGetGridMishy(new GridPosition(x, y), out Mishy mishy))
                {
                    mishies.Add(mishy);
                }
            }
        }

        return mishies;
    }


    /// <summary>
    /// 检测是否在动，如果有在动的就返回true
    /// </summary>
    /// <returns></returns>
    public bool CheckAllMishyMoveState() 
    {
        for (int x = 0; x < GridManager.Instance.GetWidth(); x++)
        {
            for (int y = 0; y < GridManager.Instance.GetHeight(); y++)
            {
                if (GridManager.Instance.TryGetGridMishy(new GridPosition(x, y), out Mishy mishy))
                {
                    if (mishy.IsMoving)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    /*public GridSystem<GridObject> GetGridSystem() 
    {
        return gridSystem;
    }*/

    /*public int GetGridSystemWidth()
    {
        return gridSystem.GetWidth();
    }

    public int GetGridSystemHeight()
    {
        return gridSystem.GetHeight();
    }*/

    /// <summary>
    /// 随机咪西类型
    /// </summary>
    /// <returns></returns>
    public MishyType RandomMishyType()
    {
        return (MishyType)UnityEngine.Random.Range(1, 5);//不会在这生成恶咪西
    }

    /// <summary>
    /// 随机咪西类型，带生成恶咪西的
    /// </summary>
    /// <returns></returns>
    public MishyType RandomMishyTypeWithBadMishy()
    {
        return (MishyType)(int)(UnityEngine.Random.Range(10, 51)/10);//不会在这生成恶咪西
    }

    /// <summary>
    /// 咪西下落
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    /// <summary>
    /// 消除后的网格重力下落算法（双指针法）
    /// </summary>
    public IEnumerator ApplyGravityRoutine()
    {
        bool movedAny = false;
        int width = GridManager.Instance.GetWidth();
        int height = GridManager.Instance.GetHeight();

        // 逐列扫描
        for (int x = 0; x < width; x++)
        {
            int emptySlotY = 0; // 当前列最下方的空位

            // 从下往上扫描
            for (int y = 0; y < height; y++)
            {
                GridPosition currentPos = new GridPosition(x, y);

                // 如果当前格子里有咪西
                if (GridManager.Instance.TryGetGridMishy(currentPos, out Mishy currentMishy))
                {
                    // 如果它下方有空位，就让它掉下去
                    if (y > emptySlotY)
                    {
                        GridPosition targetPos = new GridPosition(x, emptySlotY);

                        // 1. 逻辑层：更新网格地图
                        GridManager.Instance.ClearGridMishy(currentPos);
                        GridManager.Instance.SetGridMishy(targetPos, currentMishy);

                        // 2. 数据层：更新咪西自己的坐标记录
                        currentMishy.UpdateGridPosition(targetPos);

                        currentMishy.PlayDownAni(targetPos);

                        movedAny = true;
                    }
                    emptySlotY++; // 无论是否下落，下一格空位指针都要上移
                }
            }
        }

        // 如果发生了掉落，等待掉落动画播完再继续（让玩家看清连消过程）
        if (movedAny)
        {
            yield return new WaitUntil(() => !CheckAllMishyMoveState());

            //稍微停顿一下
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 往上推所有的咪西
    /// </summary>
    public void PushAllMishyUp(int rowCount = 1) 
    {
        // 必须从上往下遍历，不然会覆盖掉上面的方块！(你的倒序遍历是对的)
        for (int x = 0; x < GridManager.Instance.GetWidth(); x++)
        {
            for (int y = GridManager.Instance.GetHeight() - 1; y >= 0; y--)
            {
                if (GridManager.Instance.TryGetGridMishy(new GridPosition(x, y), out Mishy m))
                {
                    GridPosition currentPos = m.GetGridPosition();
                    GridPosition moveDir = new GridPosition(0, rowCount);
                    GridPosition targetPos = currentPos + moveDir;

                    if (!GridManager.Instance.IsValidGridPosition(targetPos))
                    {
                        // 被顶出最高点了
                        GridManager.Instance.ClearGridMishy(currentPos);

                        m.PlayVanishAni(); // 或者直接 Destroy(m.gameObject);

                        // TODO: 在这里触发“游戏结束 (GameOver)
                        Debug.LogWarning("游戏结束：咪西被顶出天花板了！");

                        continue; 
                    }


                    GridManager.Instance.MoveMishyWithGridPosition(currentPos, moveDir, m);

                    m.UpdateGridPosition(targetPos);

                    float delay = GetRandomDelay(x,y);
                    //float delay = UnityEngine.Random.Range(0, 0.15f);
                    m.PlayPushUpAni(targetPos, delay);

                }
            }
        }
    }

    /// <summary>
    /// 用于得到不同的push延迟动画模式
    /// </summary>
    /// <returns></returns>
    public float GetRandomDelay(int x,int y)
    {
        float delay = (x * 0.02f) + (y * 0.015f);
        //float delay = UnityEngine.Random.Range(0, 0.15f);
        return delay;
    }

    /// <summary>
    /// 消除结算完毕后，生成下一对新咪西
    /// </summary>
    public void SpawnNextPair()
    {
        if (currentMishyPairType!=null)
        {
            Mishy mishy_One = SpawnMishy(SpawnGridPositionDown, currentMishyPairType[0]);
            Mishy mishy_Two = SpawnMishy(SpawnGridPositionUp, currentMishyPairType[1]);

            mishyPlayerController.SetActivePair(mishy_One, mishy_Two);

            currentMishyPairType = null;
        }
        else 
        {
            CalculateNextPressure();
            mishyPreviewQueue.DequeueNextMishy(); 
        }
    }

    /// <summary>
    /// 用作测试
    /// </summary>
    public void MishyColumnUp(int count) 
    {
        mishyNextPushUpColumn.PushMultiColumnMishyUp(count);
    }

    /// <summary>
    ///单次下落后回合判定 
    /// </summary>
    public void OnTurnSettlementFinished() 
    {
        if (thisTurnUpCount > 0) 
        {
            int rowCoumt=thisTurnUpCount;

            thisTurnUpCount=0;
            mishyNextPushUpColumn.PushMultiColumnMishyUp(rowCoumt);
            OnTurnPushNumUpdate?.Invoke(this, 0);
            return;
        }
        SpawnNextPair();
    }

    /// <summary>
    /// 计算推出去概率
    /// </summary>
    private void CalculateNextPressure() 
    {
        float chance = (gameTimer / 30f) * chanceUpPerHalfMinute + basePushUpChance;
        chance = Mathf.Min(chance, maxPushUpChance);

        if (UnityEngine.Random.Range(1f, 100f) < chance)
        {
            int maxTurnUpCopunt = 2;
            thisTurnUpCount = UnityEngine.Random.Range(10, maxTurnUpCopunt*10+2)/10;//调整一次上来两排的概率
        }
        else 
        {
            thisTurnUpCount = 0;
        }
        OnTurnPushNumUpdate?.Invoke(this,thisTurnUpCount);
    }
}

