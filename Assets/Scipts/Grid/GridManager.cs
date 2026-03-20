using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]private int width;
    [SerializeField]private int height;
    [SerializeField]private int cellSize = 2;

    private GridSystem<GridObject> gridSystem;

    public static GridManager Instance { get; private set; }

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

    /*public void SetUp() 
    {
        gridSystem=new GridSystem(width,height,cellSize);
    }*/

    private void Start()
    {
        gridSystem = new GridSystem<GridObject>(width, height, cellSize);
    }
    public void MoveMishyWithGridPosition(GridPosition fromGridPosition, GridPosition moveDir,Mishy mishy)
    {
        gridSystem.ClearGridMishy(fromGridPosition);
        gridSystem.SetGridMishy(fromGridPosition + moveDir, mishy);
    }

    public void ClearMishyWithGridPosition(GridPosition gridPosition)
    {
        gridSystem.ClearGridMishy(gridPosition);
    }

    public Mishy GetMishy(GridPosition gridPosition)=>gridSystem.GetGridMishy(gridPosition);

    public int GetCellSize() => cellSize;
    public int GetWidth() => gridSystem.GetWidth();//得到网格宽
    public int GetHeight() => gridSystem.GetHeight();//得到网格高
}
