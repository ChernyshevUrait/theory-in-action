using System.Collections;
using UnityEngine;


// ABSTRACTION
public abstract class Resetable : MonoBehaviour
{
    /// <summary>
    /// Reset game object for starting new level
    /// </summary>
    protected virtual void Reset()
    {
        Destroy(gameObject);
    }

    protected virtual void Start()
    {
        if (GameManager.Instance == null)
        { return; }

        GameManager.Instance.onResetting.AddListener(Reset);
    }
}
