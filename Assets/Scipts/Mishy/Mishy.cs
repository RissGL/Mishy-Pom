using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mishy : MonoBehaviour
{
    public event Action<Mishy> OnMishyLanded; // ÂäµØÊÂ¼þ

    [SerializeField]private MishyType type;
    private GridPosition gridPosition;


    private void Update()
    {
        SetGridPositionAfterDown(GridManager.Instance.GetGridPosition(transform.position));
        OnMishyLanded?.Invoke(this);
    }

public void SetUp(GridPosition gridPosition ,MishyType mishyType)
    {
        this.gridPosition = gridPosition;
        this.type = mishyType;
    }

    public void SetGridPositionAfterDown(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
    }

    public MishyType GetMishyType() => type;

    public GridPosition GetGridPosition() => gridPosition;
}
