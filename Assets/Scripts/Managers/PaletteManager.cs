using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE
public class PaletteManager : Area
{
    #region Singleton Implementation

    /// <summary>
    /// <para>Instance of <see cref="PaletteManager"/></para>
    /// <para>Singleton pattern</para>
    /// </summary>
    public static PaletteManager Instance { get; private set; }

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
}
