using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class SimpleBlock : Block
{
    #region Abstarct Block Methods Implementation

    // POLYMORPHISM
    protected override void Move()
    {
        return;
    }

    // POLYMORPHISM
    protected override void StaticMove()
    {
        return;
    }

    #endregion
}
