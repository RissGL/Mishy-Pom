using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x;
    public int y;

    public GridPosition(int x,int y)
    {
        this.x = x;
        this.y = y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x,y);
    }

    public override string ToString()
    {
        return $"X:{x}, Y:{y}";
    }

    public static GridPosition operator +(GridPosition a,GridPosition b)
    {
        return new GridPosition ( a.x + b.x, a.y + b.y );
    }

    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition (a.x - b.x, a.y - b.y);
    }

    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x==b.x && a.y==b.y;
    }

    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a==b);
    }

    public bool Equals(GridPosition other)
    {
        return other.x==this.x&&other.y==this.y;
    }

    public override bool Equals(object pbj)
    {
        return pbj is GridPosition position&& position.x == this.x && position.y == this.y;
    }
}
