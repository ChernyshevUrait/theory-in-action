using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
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

    #region Inspector Properties

    /// <summary>
    /// Bounds of game area <see cref="GameAreaBound"/>
    /// </summary>
    [Tooltip("Bounds of game area")]
    [SerializeField] private GameAreaBound m_gameAreaBounds;

    /// <summary>
    /// Bounds of game area <see cref="GameAreaBound"/>
    /// </summary>
    [HideInInspector] public GameAreaBound GameAreaBounds => m_gameAreaBounds;

    #endregion

    #region Events

    public UnityEvent<int> onPiontsChenged;

    #endregion
}
