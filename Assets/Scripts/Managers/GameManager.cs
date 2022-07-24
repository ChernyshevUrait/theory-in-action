using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System;
using System.Reflection;

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

    #region Inspector Properties

    /// <summary>
    /// Ball game object to spawn
    /// </summary>
    [Tooltip("Ball game object to spawn")]
    [SerializeField] private Ball ballPrefab;

    /// <summary>
    /// Box game object to spawn
    /// </summary>
    [Tooltip("Box game object to spawn")]
    [SerializeField] private Box boxPrefab;

    /// <summary>
    /// Button to start simulation
    /// </summary>
    [Tooltip("Button to start simulation")]
    [SerializeField] private Button startButton;

    /// <summary>
    /// Text to display <see cref="currentLevel"/> number
    /// </summary>
    [Tooltip("Text to display level number")]
    [SerializeField] private TextMeshProUGUI levelText;

    /// <summary>
    /// Text to display <see cref="remainingPoints"/>
    /// </summary>
    [Tooltip("Text to display remaining points")]
    [SerializeField] private TextMeshProUGUI remainingPointsText;

    /// <summary>
    /// Start remaining points for level 1
    /// Decrease level by level
    /// </summary>
    [Tooltip("Start remaining points for level 1. Decrease level by level.")]
    [SerializeField] private int startPoints;


    #endregion

    #region Events

    /// <summary>
    /// Invoked when <see cref="RemainingPoints"/> is changed
    /// </summary>
    public UnityEvent<int> onPointsChenged = new();

    /// <summary>
    /// Invoked when game area objects is resetting
    /// </summary>
    public UnityEvent onResetting = new();

    #endregion

    private void Start()
    {
        InitializeLevel();
    }

    #region Spawn

    /// <summary>
    /// Additional offset of spawn area in <see cref="Area.AreaBounds"/>
    /// </summary>
    private readonly float spawnBound = 2;

    /// <summary>
    /// <see cref="ballPrefab"/> position for current level
    /// Calc in <see cref="InitializeLevel(int)"/>
    /// </summary>
    private Vector3 ballSpawnPosition;

    // POLYMORPHISM
    // ABSTRACTION
    /// <summary>
    /// Spawn <see cref="ballPrefab"/> in random position
    /// </summary>
    /// <returns>Spawned ball</returns>
    private void SpawnBall()
    {
        float spawnX = UnityEngine.Random.Range(AreaBounds.MinX + spawnBound, AreaBounds.MaxX - spawnBound);
        float spawnY = AreaBounds.MaxY - spawnBound;

        SpawnBall(new(spawnX, spawnY));
    }

    // POLYMORPHISM
    // ABSTRACTION
    /// <summary>
    /// Spawn <see cref="ballPrefab"/> in specified position
    /// </summary>
    /// <param name="spawnPosition">Position for spawn</param>
    /// <returns>Spawned ball</returns>
    private void SpawnBall(Vector3 spawnPosition)
    {
        if (ballPrefab == null) { return; }
        
        ballSpawnPosition = spawnPosition;

        ball = Instantiate(ballPrefab, spawnPosition, ballPrefab.transform.rotation);
        ball.onFallen.AddListener(ResetLevel);
        ball.onFinished.AddListener(NextLevel);
    }

    // POLYMORPHISM
    // ABSTRACTION
    /// <summary>
    /// Spawn <see cref="boxPrefab"/> in random position
    /// </summary>
    private void SpawnBox()
    {
        float spawnX = UnityEngine.Random.Range(AreaBounds.MinX + spawnBound, AreaBounds.MaxX - spawnBound);
        float spawnY = AreaBounds.MinY + spawnBound;
        
        SpawnBox(new(spawnX, spawnY));
    }

    // POLYMORPHISM
    // ABSTRACTION
    /// <summary>
    /// Spawn <see cref="boxPrefab"/> in specified position
    /// </summary>
    /// <param name="spawnPosition">Position for spawn</param>
    private void SpawnBox(Vector3 spawnPosition)
    {
        if (boxPrefab == null) { return; }

        boxPrefab.tag = "Finish";

        Instantiate(boxPrefab, spawnPosition, boxPrefab.transform.rotation);
    }

    #endregion

    #region Game Simulation

    /// <summary>
    /// Spawned ball in <see cref="InitializeLevel(int)"/>
    /// </summary>
    private Ball ball;

    /// <summary>
    /// Current game level
    /// </summary>
    private int currentLevel = 0;

    /// <summary>
    /// Remaining points
    /// </summary>
    private int remainingPoints = 0;

    // ENCAPSULATION
    /// <summary>
    /// Remaining points
    /// </summary>
    public int RemainingPoints
    {
        get { return remainingPoints; }
        set
        {
            remainingPoints = value;
            remainingPointsText.text = $"Remaining points: {remainingPoints}";

            onPointsChenged.Invoke(remainingPoints);
        }
    }


    // POLYMORPHISM
    // ABSTRACTION
    /// <summary>
    /// Initialize next level
    /// Calc based on <see cref="currentLevel"/>
    /// Set <see cref="remainingPoints"/>
    /// Spawn <see cref="ballPrefab"/>
    /// Spawn <see cref="boxPrefab"/>
    /// </summary>
    private void InitializeLevel()
    {
        currentLevel++;
        InitializeLevel(currentLevel);
    }

    // POLYMORPHISM
    // ABSTRACTION
    /// <summary>
    /// Initialize specified level
    /// Set start count of <see cref="remainingPoints"/>
    /// Spawn <see cref="ballPrefab"/>
    /// Spawn <see cref="boxPrefab"/>
    /// </summary>
    /// <param name="level">Level number</param>
    private void InitializeLevel(int level)
    {
        startButton.interactable = false;
        startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start";
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(StartLevel);        
        
        levelText.text = $"Level {level}";

        CalcRemainingPointsByLevel(level);

        SpawnBall();
        SpawnBox();

        startButton.interactable = true;
    }

    // ABSTRACTION
    [Label("Start")]
    private void StartLevel()
    {
        if (ball == null)
        { return; }

        SetButtonState(ResetLevel);

        ball.GetComponent<Rigidbody>().useGravity = true;
    }

    [Label("Reset")]
    private void ResetLevel()
    {
        if (ball == null)
        {
            return;
        }

        SetButtonState(StartLevel);

        ball.GetComponent<Rigidbody>().useGravity = false;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.position = ballSpawnPosition;
    }

    // ABSTRACTION
    /// <summary>
    /// Initialize next level
    /// </summary>
    private void NextLevel()
    {
        onResetting.Invoke();
        InitializeLevel();
    }

    // ABSTRACTION
    /// <summary>
    /// Set start count of <see cref="remainingPoints"/> for specified level
    /// </summary>
    /// <param name="level">Level number</param>
    private void CalcRemainingPointsByLevel(int level)
    {
        if (level <= 10)
        {
            RemainingPoints = startPoints - 10 * (int)Mathf.Pow(level-1, 2);
        }
        else
        {
            RemainingPoints = 30;
        }
    }

    // ABSTRACTION
    /// <summary>
    /// Set <see cref="startButton"/> properties (text & action)
    /// </summary>
    /// <param name="action">Action for button click event</param>
    private void SetButtonState(UnityAction action)
    {
        LabelAttribute info = action.GetMethodInfo().GetCustomAttribute<LabelAttribute>();
        if (info != null)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = info.Label;
        }
        else
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = action.GetMethodInfo().Name;
        }

        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(action);
    }

    // ABSTRACTION
    public void IncreaseRemainingPoint(int value)
    {
        RemainingPoints+=value;
    }

    // ABSTRACTION
    public void DecreaseRemainingPoints(int value)
    {
        IncreaseRemainingPoint(-value);
    }

    #endregion
}
