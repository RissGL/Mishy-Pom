using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject
{
    private GridPosition gridPosition;
    private Mishy mishy;

    public GridObject(GridPosition gridPosition) 
    {
        this.gridPosition = gridPosition;
        mishy = null;
    }

    public Mishy GetMishy() 
    {
        return mishy;
    }

    public void SetMishy(Mishy mishy)
    {
        this.mishy = mishy;
    }

    public void ClearMishy() 
    {
        this.mishy = null;
    }

    public bool HasMishy() 
    {
        return mishy != null;
    }

    public override string ToString()
    {
        return $"{gridPosition} : {(mishy != null ? "Occupied" : "Empty")}";
    }
}
