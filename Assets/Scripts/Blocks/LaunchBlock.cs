using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class LaunchBlock : Block
{
    /// <summary>
    /// Direction of block rotation. The side where the ball will be launched
    /// </summary>
    [Tooltip("Direction of block rotation. The side where the ball will be launched")]
    [SerializeField] private LaunchSide side = LaunchSide.Left;

    /// <summary>
    /// Is moving coroutine started
    /// <see cref="Rotate"/>
    /// </summary>
    private bool isMoving = false;

    #region Abstarct Block Methods Implementation

    // INHERITANCE
    protected override void Move()
    {
        if (!isMoving)
        {
            isMoving = true;
            StartCoroutine(Rotate());
        }
    }

    // INHERITANCE
    protected override void StaticMove()
    {
        Move();
    }

    #endregion

    // ABSTRACTION
    /// <summary>
    /// Rotation (launch)
    /// </summary>
    private IEnumerator Rotate()
    {
        float angle = 60 * (int)side;
        Vector3 offset = (int)side * (transform.lossyScale.x - transform.lossyScale.y) * Vector3.right / 2;
        Vector3 point = transform.position - offset;

        while (isMoving && !isDragging)
        {
            yield return new WaitForSeconds(3);
            transform.RotateAround(point,Vector3.forward, angle);
            yield return new WaitForSeconds(1);
            transform.RotateAround(point, Vector3.forward, -angle);
        }
        transform.rotation = Quaternion.identity;
        isMoving = false;
    }
}
