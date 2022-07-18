using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public struct AreaBound
{
    [Tooltip("X")]
    [SerializeField] private float minX;
    public float MinX => minX;

    [Tooltip("Y")]
    [SerializeField] private float maxX;
    public float MaxX => maxX;

    [Tooltip("X")]
    [SerializeField] private float minY;
    public float MinY => minY;

    [Tooltip("Y")]
    [SerializeField] private float maxY;
    public float MaxY => maxY;

    public Vector3 Center => new((minX + maxX) / 2, (minY + maxY) / 2, 0);

    public AreaBound(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }

    public override bool Equals(object obj)
    {
        if (obj is not AreaBound other) { return false; }
        return this.minX == other.minX && this.maxX == other.maxX && this.minY == other.minY && this.maxY == other.maxY;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"X = [{minX} ; {maxX}] , Y = [{minY} ; {maxY}]";
    }
}

