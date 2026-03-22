using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private GridPosition SpawnGridPositionUp;
    private GridPosition SpawnGridPositionDown = new GridPosition(2, 10);


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
        mishyNextPushUpColumn.OnMishyPushUp += NextPushUpColumnMishy_OnMishyPushUp;
        SpawnGridPositionUp=new GridPosition(SpawnGridPositionDown.x,SpawnGridPositionDown.y+1);

        mishyNextPushUpColumn.NextPushUpColumnMishyInit();
        StartCoroutine(AsyncSpawnFirstMishyPair());
        //TODO:改成等待玩家确定后再生成第一个
    }



    private void OnDestroy()
    {
        mishyPreviewQueue.OnNextMishyNeedSpawn -= MishyPreviewQueue_OnNextMishyNeedSpawn;
        mishyNextPushUpColumn.OnMishyPushUp -= NextPushUpColumnMishy_OnMishyPushUp;
    }

    private IEnumerator AsyncSpawnFirstMishyPair() 
    {
        yield return null;
        mishyPreviewQueue.DequeueNextMishy();
    }

    private void MishyPreviewQueue_OnNextMishyNeedSpawn(object sender, MishyPreviewQueue.MishyPairEventArgs e)
    {
        Mishy mishy_One= SpawnMishy(SpawnGridPositionDown, e.type_one);
        Mishy mishy_Two= SpawnMishy(SpawnGridPositionUp, e.type_two);

        mishyPlayerController.SetActivePair(mishy_One,mishy_Two);
    }

    /// <summary>
    /// 底部咪西往上推
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void NextPushUpColumnMishy_OnMishyPushUp(object sender, MishyType[] e)
    {
        PushAllMishyUp();

        for (int i = 0; i < e.Length; i++) 
        {
            SpawnMishy(new GridPosition(i, 0), e[i]);
        }
    }

    /*public void SetUp(GridSystem<GridObject> gridSystem) 
    {
        this.gridSystem = gridSystem;
    }*/

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

        Debug.Log($"生成了 {mishyDatabase.GetPrefab(mishyType).name} 在 {gridPosition}");
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

    public MishyType RandomMishyTypeWithBadMishy()
    {
        return (MishyType)UnityEngine.Random.Range(1, 6);//不会在这生成恶咪西
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

                        // 3. 表现层：执行平滑下落动画 (0.15秒掉落时间)
                        //StartCoroutine(currentMishy.SmoothMoveTo(GridManager.Instance.GetWorldPosition(targetPos), 0.15f));

                        movedAny = true;
                    }
                    emptySlotY++; // 无论是否下落，下一格空位指针都要上移
                }
            }
        }

        // 如果发生了掉落，等待掉落动画播完再继续（让玩家看清连消过程）
        if (movedAny)
        {
            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// 往上推所有的咪西
    /// </summary>
    public void PushAllMishyUp() 
    {
        for (int x = 0; x < GridManager.Instance.GetWidth(); x++)
        {
            for (int y = GridManager.Instance.GetHeight()-1; y >= 0; y--)
            {
                Mishy mishy = GridManager.Instance.GetGridMishy(new GridPosition(x, y));
                GridManager.Instance.MoveMishyWithGridPosition(mishy.GetGridPosition(),
                    new GridPosition(0, 1), mishy);
            }
        }
    }

    /// <summary>
    /// 消除结算完毕后，生成下一对新咪西
    /// </summary>
    public void SpawnNextPair()
    {
        mishyPreviewQueue.DequeueNextMishy();
    }
}
