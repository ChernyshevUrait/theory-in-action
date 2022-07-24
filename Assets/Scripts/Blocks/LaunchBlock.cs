using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
[RequireComponent(typeof(Rigidbody))]
public class LaunchBlock : Block
{
    /// <summary>
    /// Direction of block rotation. The side where the ball will be launched
    /// </summary>
    [Tooltip("Direction of block rotation. The side where the ball will be launched")]
    [SerializeField] private LaunchSide side = LaunchSide.Left;

    /// <summary>
    /// Is moving coroutine started
    /// <see cref="StaticRotate"/>
    /// </summary>
    private bool isMoving = false;

    #region Abstarct Block Methods Implementation

    // INHERITANCE
    protected override void Move()
    {
        if (isMoving)
        {
            isMoving = false;
            StartCoroutine(Rotate());
        }
    }

    // INHERITANCE
    protected override void StaticMove()
    {
        if (!isMoving)
        {
            isMoving = true;
            StartCoroutine(StaticRotate());
        }
    }

    #endregion

    // ABSTRACTION
    /// <summary>
    /// Rotation (launch) for <see cref="StaticMove"/>
    /// </summary>
    private IEnumerator StaticRotate()
    {

        while (isMoving)
        {
            yield return new WaitForSeconds(3);

            if (isDragging)
            { break; }

            Vector3 startPosition = transform.position;
            GetComponent<Rigidbody>().AddTorque(Vector3.back * 200 * (int)side, ForceMode.Impulse);

            yield return new WaitForSeconds(1);

            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            transform.position = startPosition;
            transform.rotation = Quaternion.identity;

            
        }

        isMoving = false;
    }

    // ABSTRACTION
    /// <summary>
    /// Rotation (launch) for <see cref="Move"/>
    /// </summary>
    private IEnumerator Rotate()
    {

        Vector3 startPosition = transform.position;
        GetComponent<Rigidbody>().AddTorque(Vector3.back * 400 * (int)side, ForceMode.Impulse);
            
        yield return new WaitForSeconds(1);

        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.position = startPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Ball")
        {
            isMoving = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.tag == "Ball")
        {
            isMoving = false;
        }
    }

    protected override void Start()
    {
        base.Start();

        Vector3 offset = (int)side * (transform.lossyScale.x - transform.lossyScale.y) * Vector3.right / 2;
        
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        rigidbody.centerOfMass = offset;

    }
}
