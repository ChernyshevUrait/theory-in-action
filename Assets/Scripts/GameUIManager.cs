using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{

    #region Singleton Implementation

    // ENCAPSULATION
    /// <summary>
    /// <para>Instance of <see cref="GameUIManager"/></para>
    /// <para>Singleton pattern</para>
    /// </summary>
    public static GameUIManager Instance { get; private set; }

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
    /// Return to main menu button
    /// </summary>
    [Tooltip("Return to main menu button")]
    [SerializeField] private Button returnButton;

    #endregion

    #region Unity Messages

    private void Start()
    {
        if (MenuManager.Instance == null)
        {
            returnButton.gameObject.SetActive(false);
        }
        else
        {
            returnButton.onClick.AddListener(MenuManager.Instance.Return);
        }
    }

    #endregion
}
