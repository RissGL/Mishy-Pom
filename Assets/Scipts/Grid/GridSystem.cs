using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem <TGridObject>
{
    private int width;
    private int height;
    private int cellSize = 2;

    private GridObject[,] gridObjects;

    public GridSystem(int width,int height, int cellSize)
    {
        this.width = width;
        this.height = height;
        gridObjects = new GridObject[width, height];
        this.cellSize = cellSize;
    }

    public void ClearGridMishy(GridPosition gridPosition)
    {
        gridObjects[gridPosition.x, gridPosition.y].ClearMishy();
    }

    public void SetGridMishy(GridPosition gridPosition,Mishy mishy) 
    {
        gridObjects[gridPosition.x,gridPosition.y].SetMishy(mishy);
    }

    public GridObject GetGridObject(GridPosition gridPosition) 
    {
        return gridObjects[gridPosition.x,gridPosition.y];
    }

    public Mishy GetGridMishy(GridPosition gridPosition) 
    {
        return gridObjects[gridPosition.x, gridPosition.y].GetMishy();
    }

    public bool HasMishy(GridPosition gridPosition)
    {
        return gridObjects[gridPosition.x, gridPosition.y].HasMishy();
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }
}
