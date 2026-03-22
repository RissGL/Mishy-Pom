using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mishy : MonoBehaviour
{
    [Header("动画系统")]
    [SerializeField] private Animator animator;

    public event Action<Mishy> OnMishyLanded; // 落地事件

    [SerializeField]private MishyType type;
    private GridPosition gridPosition;

    private bool isLanded=false;

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
}
