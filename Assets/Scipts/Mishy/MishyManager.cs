using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MishyManager : MonoBehaviour
{
    private GridSystem<GridObject> gridSystem;
    public MishyManager Instance { get; private set; }
    [SerializeField] private Transform mishyContainer;   // 场景中存放咪西的父节点
    [SerializeField] private MishyDatabase mishyDatabase; //SO 文件

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

    public void SetUp(GridSystem<GridObject> gridSystem) 
    {
        this.gridSystem = gridSystem;
    }

    public void SpawnMishy(GridPosition gridPosition,MishyType mishyType)
    {
        GameObject mishyPrefab= mishyDatabase.GetPrefab(mishyType);
        if(mishyPrefab==null)
            return;

        Mishy mishy =mishyPrefab.GetComponent<Mishy>();
        gridSystem.SetGridMishy(gridPosition, mishy);
    }
}
