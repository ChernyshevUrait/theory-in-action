using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public struct GameAreaBound
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

}

