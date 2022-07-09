using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour
{
    #region Singleton Implementation

    // ENCAPSULATION
    /// <summary>
    /// <para>Instance of <see cref="MenuUIManager"/></para>
    /// <para>Singleton pattern</para>
    /// </summary>
    public static MenuUIManager Instance { get; private set; }

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
    /// Menu start button
    /// </summary>
    [Tooltip("Menu start button")]
    [SerializeField] private Button startButton;

    /// <summary>
    /// Menu exit button
    /// </summary>
    [Tooltip("Menu exit button")]
    [SerializeField] private Button exitButton;

    #endregion

    #region Unity Messages

    private void Start()
    {
        if (MenuManager.Instance == null)
        {
            startButton.gameObject.SetActive(false);
            exitButton.gameObject.SetActive(false);
        }
        else
        {
            startButton.onClick.AddListener(MenuManager.Instance.StartGame);
            exitButton.onClick.AddListener(MenuManager.Instance.Exit);
        }
    }

    #endregion
}