using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mishy : MonoBehaviour
{
    [SerializeField]private MishyType type;
    private GridPosition gridPosition;

    public MishyType GetMishyType() => type;

    public GridPosition GetGridPosition() => gridPosition;

    public void SetUp(GridPosition gridPosition ,MishyType mishyType)
    {
        this.gridPosition = gridPosition;
        this.type = mishyType;
    }
}
