using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GridSystem <TGridObject>
{
    private int width;
    private int height;
    private int cellSize = 2;

    private GridObject[,] gridObjects;
    private Vector3 originPosition;

    public GridSystem(int width,int height, int cellSize,Vector3 originPosition)
    {
        this.width = width;
        this.height = height;
        gridObjects = new GridObject[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
            {
                gridObjects[x, y]=new GridObject(new GridPosition(x,y));
            }
        }
        this.cellSize = cellSize;
        this.originPosition = originPosition;
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

    public bool TryGetGridMishy(GridPosition gridPosition,out Mishy mishy)
    {
        if (!IsValidGridPosition(gridPosition))
        {
            mishy = null;
            return false;
        }

        mishy = gridObjects[gridPosition.x, gridPosition.y].GetMishy();
        return HasMishy(gridPosition);
    }

    public bool HasMishy(GridPosition gridPosition)
    {
        if(!IsValidGridPosition(gridPosition))
            return false;

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

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt((worldPosition.x  - originPosition.x) /(float) cellSize),
            Mathf.RoundToInt((worldPosition.y -originPosition.y) / (float)cellSize)
            );
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(
            gridPosition.x*cellSize + originPosition.x,
            gridPosition.y*cellSize + originPosition.y,
            originPosition.z);
    }

    public bool IsValidGridPosition(GridPosition gridPosition) 
    {
        return (gridPosition.x >= 0 && gridPosition.y >= 0 &&gridPosition.x<width && gridPosition.y<height);
    }
}
