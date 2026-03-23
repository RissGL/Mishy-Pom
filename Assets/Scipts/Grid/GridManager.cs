using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [SerializeField]private int width;
    [SerializeField]private int height;
    [SerializeField]private int cellSize = 2;

    private GridSystem<GridObject> gridSystem;
    [SerializeField] private Vector3 originPosition;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("已经存在GridManager实例");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        gridSystem = new GridSystem<GridObject>(width, height, cellSize, originPosition);
    }

    /*public void SetUp() 
    {
        gridSystem=new GridSystem(width,height,cellSize);
    }*/

    private void Start()
    {
        //MishyManager.Instance.SetUp(gridSystem);
    }

    /// <summary>
    /// 移动咪西
    /// </summary>
    /// <param name="fromGridPosition">起始格子</param>
    /// <param name="moveDir">移动向量</param>
    /// <param name="mishy">移动的咪西</param>
    public void MoveMishyWithGridPosition(GridPosition fromGridPosition, GridPosition moveDir,Mishy mishy)
    {
        gridSystem.ClearGridMishy(fromGridPosition);
        gridSystem.SetGridMishy(fromGridPosition + moveDir, mishy);
    }

    /// <summary>
    /// 尝试移动咪西
    /// </summary>
    /// <param name="fromGridPosition">起始格子</param>
    /// <param name="moveDir">移动向量</param>
    /// <param name="mishy">移动的咪西</param>
    /// <returns></returns>
    public bool TryMoveMishyWithGridPosition(GridPosition fromGridPosition, GridPosition moveDir, Mishy mishy)
    {
        GridPosition testPosition= fromGridPosition+moveDir;
        if (!gridSystem.IsValidGridPosition(testPosition))
            return false;

        if(gridSystem.HasMishy(testPosition))
            return false;

        MoveMishyWithGridPosition(fromGridPosition,moveDir, mishy);
        return true;
    }

    /// <summary>
    /// 仅用于检测是否可以东
    /// </summary>
    /// <param name="fromGridPosition">起始格子</param>
    /// <param name="moveDir">移动向量</param>
    /// <returns></returns>
    public bool CanMoveWithGridPosition(GridPosition fromGridPosition, GridPosition moveDir) 
    {
        GridPosition testPosition = fromGridPosition + moveDir;
        if (!gridSystem.IsValidGridPosition(testPosition))
            return false;

        if (gridSystem.HasMishy(testPosition))
            return false;

        return true;
    }

    public bool CanMoveWithGridPosition(GridPosition[] fromGridPosition, GridPosition moveDir)
    {
        foreach (GridPosition gridPosition in fromGridPosition)
        {
            CanMoveWithGridPosition (gridPosition, moveDir);
        }
        return true;
    }
    public void ClearMishyWithGridPosition(GridPosition gridPosition)
    {
        gridSystem.ClearGridMishy(gridPosition);
    }

    public Mishy GetMishy(GridPosition gridPosition)=>gridSystem.GetGridMishy(gridPosition);

    public int GetCellSize() => cellSize;
    public int GetWidth() => gridSystem.GetWidth();//得到网格宽
    public int GetHeight() => gridSystem.GetHeight();//得到网格高
    public bool HasMishy(GridPosition gridPosition) => gridSystem.HasMishy(gridPosition);
    public void SetGridMishy(GridPosition gridPosition, Mishy mishy) => gridSystem.SetGridMishy(gridPosition, mishy);
    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    //由世界到网格
    public GridPosition GetGridPosition(Vector3 worldPosition)
        => gridSystem.GetGridPosition(worldPosition);

    //由网格到世界
    public Vector3 GetWorldPosition(GridPosition gridPosition)
        => gridSystem.GetWorldPosition(gridPosition);

    public Mishy GetGridMishy(GridPosition gridPosition)
    {
        return gridSystem.GetGridMishy(gridPosition);
    }

    public bool TryGetGridMishy(GridPosition gridPosition, out Mishy mishy)
    {
        return gridSystem.TryGetGridMishy(gridPosition,out mishy);
    }

    public void ClearGridMishy(GridPosition gridPosition)
    {
        gridSystem.ClearGridMishy(gridPosition);
    }

    public MishyType[,] GetRowMishyType(int rowCount =1) 
    {
        MishyType[,] mishyTypes = new MishyType[width,rowCount];
        for (int y = 0; y < rowCount; y++) 
        {
            for (int x = 0; x < width; x++)
            {
                if (TryGetGridMishy(new GridPosition(x,y),out Mishy mishy))
                {
                    mishyTypes[x,y]=mishy.GetMishyType();
                }
            }
        }

        return mishyTypes;
    }
}
