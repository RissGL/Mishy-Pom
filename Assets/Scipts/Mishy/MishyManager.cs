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
        mishyPreviewQueue.DequeueNextMishy();
    }

    private void MishyPreviewQueue_OnNextMishyNeedSpawn(object sender, MishyPreviewQueue.twoMishy e)
    {
        SpawnMishy(SpawnGridPositionDown, e.type_one);
        SpawnMishy(SpawnGridPositionUp, e.type_two);
    }

    public void SetUp(GridSystem<GridObject> gridSystem) 
    {
        this.gridSystem = gridSystem;
    }

    public void SpawnMishy(GridPosition gridPosition,MishyType mishyType)
    {
        GameObject mishyPrefab= mishyDatabase.GetPrefab(mishyType);
        if(mishyPrefab==null)
            return;

        GameObject mishyTransform=Instantiate(mishyPrefab,mishyContainer);
        mishyTransform.transform.localPosition = gridSystem.GetWorldPosition(gridPosition);

        Mishy mishy = mishyTransform.GetComponent<Mishy>();
        mishy.SetUp(gridPosition, mishyType);
        mishy.OnMishyLanded += (landedMishy) => 
        {
            gridSystem.SetGridMishy(landedMishy.GetGridPosition(), landedMishy);
        };

        Debug.Log($"生成了 {mishyDatabase.GetPrefab(mishyType).name} 在 {gridPosition}");
    }
}
