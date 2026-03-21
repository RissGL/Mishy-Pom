using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mishy : MonoBehaviour
{
    public event Action<Mishy> OnMishyLanded; // 落地事件

    [SerializeField]private MishyType type;
    private GridPosition gridPosition;

    private bool isLanded=false;


    private void Update()
    {
        SetGridPositionOnLand(GridManager.Instance.GetGridPosition(transform.position));
        if (isLanded)
        {
            //TODO: 下落逻辑
            OnMishyLanded?.Invoke(this);
        }
    }

public void SetUp(GridPosition gridPosition ,MishyType mishyType)
    {
        this.gridPosition = gridPosition;
        this.type = mishyType;
    }

    public void SetGridPositionOnLand(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public void UpdateGridPosition(GridPosition gridPosition) 
    {
        this.gridPosition = gridPosition;
    }

    public MishyType GetMishyType() => type;

    public GridPosition GetGridPosition() => gridPosition;
}
