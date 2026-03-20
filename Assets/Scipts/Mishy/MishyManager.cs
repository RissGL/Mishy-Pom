using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyManager : MonoBehaviour
{
    public static MishyManager Instance { get; private set; }

    private GridSystem<GridObject> gridSystem;

    [SerializeField] private Transform mishyContainer;   // 场景中存放咪西的父节点
    [SerializeField] private MishyDatabase mishyDatabase; //SO 文件
    [SerializeField] private MishyPreviewQueue mishyPreviewQueue;

    [SerializeField] private GridPosition SpawnGridPositionDown;
    [SerializeField] private MishyPlayerController mishyPlayerController;
    private GridPosition SpawnGridPositionUp;

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
        SpawnGridPositionUp=new GridPosition(SpawnGridPositionDown.x,SpawnGridPositionDown.y+1);
        StartCoroutine(AsyncSpawnFirstMishyPair());
        //TODO:改成等待玩家确定后再生成第一个
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

    public void SetUp(GridSystem<GridObject> gridSystem) 
    {
        this.gridSystem = gridSystem;
    }

    public Mishy SpawnMishy(GridPosition gridPosition,MishyType mishyType)
    {
        GameObject mishyPrefab= mishyDatabase.GetPrefab(mishyType);
        if(mishyPrefab==null)
            return null;

        GameObject mishyTransform=Instantiate(mishyPrefab,mishyContainer);
        mishyTransform.transform.localPosition = gridSystem.GetWorldPosition(gridPosition);

        Mishy mishy = mishyTransform.GetComponent<Mishy>();
        mishy.SetUp(gridPosition, mishyType);
        mishy.OnMishyLanded += (landedMishy) => 
        {
            GridPosition finalPosition = landedMishy.GetGridPosition();

            if (!gridSystem.HasMishy(finalPosition))
            {
                gridSystem.SetGridMishy(landedMishy.GetGridPosition(), landedMishy);
                // TODO: 消除检查逻辑
            }
        };

        Debug.Log($"生成了 {mishyDatabase.GetPrefab(mishyType).name} 在 {gridPosition}");
        return mishy;
    }

    /// <summary>
    /// 暂时得到所有咪西，因为往上推的时候每一个咪西都要动
    /// </summary>
    /// <returns></returns>
    public List<Mishy> GetAllMishies() 
    {
        List<Mishy> mishies = new List<Mishy>();

        for (int x = 0; x < gridSystem.GetWidth(); x++) 
        {
            for (int y = 0; y < gridSystem.GetHeight(); y++)
            {
                mishies.Add(gridSystem.GetGridMishy(new GridPosition(x,y)));
            }
        }

        return mishies;
    }
}
