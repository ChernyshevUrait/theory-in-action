using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

// INHERITANCE
public class GameManager : Area
{
    #region Singleton Implementation

    // ENCAPSULATION
    /// <summary>
    /// <para>Instance of <see cref="GameManager"/></para>
    /// <para>Singleton pattern</para>
    /// </summary>
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    #endregion

    #region Events

    public UnityEvent<int> onPiontsChenged;

    #endregion

    private void LoadLevel()
    {

    }
}
