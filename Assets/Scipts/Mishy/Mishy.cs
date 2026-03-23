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

    public bool IsMoving=>mishyVisual.IsMoving;
    public void SetGridPosition(GridPosition pos)
    {
        gridPosition = pos;
    }

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

    /// <summary>
    /// 下落动画
    /// </summary>
    /// <param name="targetGridPosition"></param>
    public void PlayDownAni(GridPosition targetGridPosition)
    {
        mishyVisual.DownMoveAni(this.transform, targetGridPosition);
    }

    public void PlayVanishAni() 
    {
        mishyVisual.PlayVanishEffects();
    }

    /// <summary>
    /// 落地效果
    /// </summary>
    public void PlayLandAni() 
    {
        mishyVisual.PlayLandEffects();
    }

    /// <summary>
    /// 推上去的动画
    /// </summary>
    /// <param name="targetGridPosition"></param>
    public void PlayPushUpAni(GridPosition targetGridPosition,float delay=0)
    {
        mishyVisual.PushUpMoveAni(this.transform, targetGridPosition,delay);
    }
}
