using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mishy : MonoBehaviour
{
    [Header("视觉脚本")]
    [SerializeField] private MishyVisual mishyVisual;

    public event Action<Mishy> OnMishyLanded; // 落地事件

    [SerializeField]private MishyType type;
    private GridPosition gridPosition;

    public void SetUp(GridPosition gridPosition ,MishyType mishyType)
    {
        this.gridPosition = gridPosition;
        this.type = mishyType;
    }

    public void UpdateGridPosition(GridPosition gridPosition) 
    {
        this.gridPosition = gridPosition;
    }

    public MishyType GetMishyType() => type;

    public GridPosition GetGridPosition() => gridPosition;

    public void PlayDownAni(GridPosition targetGridPosition)
    {
        mishyVisual.DownMoveAni(this.transform, targetGridPosition);
        Debug.Log(targetGridPosition);
    }
}
