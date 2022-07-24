using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System;

// ABSTRACTION
// INHERITANCE
[RequireComponent(typeof(Collider))]
abstract public class Block : Resetable
{
    #region Inspector properties

    /// <summary>
    /// Block is on palette
    /// </summary>
    [Tooltip("Block is on palette")]
    [SerializeField] private bool onPalette;

    /// <summary>
    /// Block cost
    /// </summary>
    [Tooltip("Block cost")]
    [SerializeField] private int cost;

    #endregion

    #region Materials

    /// <summary>
    /// Material of block on game area or palette
    /// </summary>
    private Material commonMaterial;

    /// <summary>
    /// Material of block while dragging
    /// </summary>
    private Material dragMaterial;

    #endregion

    #region Events

    /// <summary>
    /// Invoked when block is deleted from game area
    /// </summary>
    public UnityEvent<int> onDelete = new();

    /// <summary>
    /// Invoked when block is instantiated on game area
    /// </summary>
    public UnityEvent<int> onInstantiate = new();

    #endregion

    #region Resetable Overrides

    protected override void Reset()
    {
        if (onPalette)
        { return; }

        base.Reset();
    }

    #endregion

    #region Unity Messages

    protected override void Start()
    {
        base.Start();
    
        commonMaterial = GetComponent<Renderer>().material;
        dragMaterial = CreateTransparentMaterial(commonMaterial);

        if (onPalette && GameManager.Instance != null)
        {
            GameManager.Instance.onPointsChenged.AddListener(CheckRemainingPoint);

        }

        if (GameManager.Instance != null)
        {
            onDelete.AddListener(GameManager.Instance.IncreaseRemainingPoint);
            onInstantiate.AddListener(GameManager.Instance.DecreaseRemainingPoints);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!isDragging)
        {
            Action moveAction = (onPalette) ? StaticMove : Move;
            moveAction();
        }
    }

    private void OnMouseDrag()
    {
        if (!isEnabled) { return; }

        isDragging = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 rayPoint = ray.GetPoint(-Camera.main.gameObject.transform.position.z);
        if (onPalette)
        {
            GetComponent<Renderer>().material = dragMaterial;
            
            if (startDragPosition.Equals(Vector3.positiveInfinity))
            {
                startDragPosition = transform.position;
            }
            transform.position = rayPoint;
        }
        else
        {
            transform.position = FitPositionToBounds(rayPoint);
        }
    }

    private void OnMouseUp()
    {
        if (!isEnabled) { return; }

        if (isDragging) { isDragging = false; }

        GetComponent<Renderer>().material = commonMaterial;

        if (clickCount == 2)
        {
            if (onPalette) { Instantiate(); }
            else { Delete(); }
        }

        if (onPalette)
        {
            if (IsPositionInBounds(transform.position))
            { 
                Instantiate(transform.position);
            }
            transform.position = startDragPosition;
        }

        startDragPosition = Vector3.positiveInfinity;
    }

    #endregion

    #region Moving at game area

    /// <summary>
    /// Is block dragging now?
    /// </summary>
    protected bool isDragging = false;

    // ABSTRACTION
    // POLYMORPHISM
    /// <summary>
    /// Calc block position using <see cref="Transform.lossyScale"/> to fit it in <see cref="GameManager.GameAreaBounds"/>
    /// </summary>
    /// <param name="point">Target position</param>
    /// <returns>Block position in <see cref="GameManager.GameAreaBounds"/></returns>
    private Vector3 FitPositionToBounds(Vector3 point)
    {
        point.z = 0;
        if (GameManager.Instance == null)
        {
            return point;
        }

        point.x = FitPositionToBounds(GameManager.Instance.AreaBounds.MinX, GameManager.Instance.AreaBounds.MaxX, point.x, transform.localScale.x / 2);
        point.y = FitPositionToBounds(GameManager.Instance.AreaBounds.MinY, GameManager.Instance.AreaBounds.MaxY, point.y, transform.localScale.y / 2);

        return point;
    }

    // ABSTRACTION
    // POLYMORPHISM
    /// <summary>
    /// Calc Block position in one dimension to fit it in specified bound
    /// </summary>
    /// <param name="bound">Specified bound. One of <see cref="GameManager.GameAreaBounds"/></param>
    /// <param name="position">Tartget position</param>
    /// <param name="scale">Scale of <see cref="Block"/></param>
    /// <returns></returns>
    private float FitPositionToBounds(float minBound, float maxBound, float position, float scale)
    {
        float calcMinBound = minBound + scale;
        float calcMaxBound = maxBound - scale;
        if (position > calcMaxBound)
        {
            position = calcMaxBound;
        }
        else if (position < calcMinBound)
        {
            position = calcMinBound;
        }

        return position;
    }

    #endregion

    #region Delete block

    /// <summary>
    /// Delete block from game area
    /// </summary>
    public void Delete()
    {
        Destroy(gameObject);

        onDelete.Invoke(cost);
    }

    #endregion

    #region Instantiate Block

    /// <summary>
    /// Create new semitransparent material from another material
    /// </summary>
    /// <param name="material">Source material</param>
    /// <returns>Semitransparent material</returns>
    private static Material CreateTransparentMaterial(Material material)
    {
        Material newMaterial = new(material);
        
        newMaterial.SetOverrideTag("RenderType", "Transparent");
        newMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        newMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        newMaterial.SetInt("_ZWrite", 0);
        newMaterial.DisableKeyword("_ALPHATEST_ON");
        newMaterial.EnableKeyword("_ALPHABLEND_ON");
        newMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        newMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        Color color = newMaterial.color;
        color.a = 0.3f;
        newMaterial.color = color;

        return newMaterial;
    }

    /// <summary>
    /// Block position when dragging is started
    /// Value before dragging is <see cref="Vector3.positiveInfinity"/>
    /// </summary>
    private Vector3 startDragPosition = Vector3.positiveInfinity;

    // ABSTRACTION
    // POLYMORPHISM
    /// <summary>
    /// Instantiate new block on the center of game area
    /// </summary>
    private void Instantiate()
    {
        Vector3 instantiatePosition = (GameManager.Instance == null) ? Vector3.zero : GameManager.Instance.AreaBounds.Center;
        Instantiate(instantiatePosition);
    }

    // ABSTRACTION
    // POLYMORPHISM
    /// <summary>
    /// Instantiate new block on specified position of game area
    /// </summary>
    /// <param name="instantiatePosition">Position of instantiation</param>
    private void Instantiate(Vector3 instantiatePosition)
    {
        GameObject newBlock = Instantiate(gameObject, instantiatePosition, transform.rotation, GameManager.Instance.transform);
        newBlock.GetComponent<Block>().onPalette = false;

        onInstantiate.Invoke(cost);
    }

    // ABSTRACTION
    // POLYMORPHISM
    /// <summary>
    /// Is point on game area?
    /// Calc based on block size and position
    /// </summary>
    /// <param name="point">Point for checking</param>
    private bool IsPositionInBounds(Vector3 point)
    {
        if (GameManager.Instance == null)
        {
            return true;
        }

        return IsPositionInBounds(point, GameManager.Instance.AreaBounds);
    }

    // ABSTRACTION
    // POLYMORPHISM
    /// <summary>
    /// Is position in specified range?
    /// Calc based on block size 
    /// </summary>
    /// <param name="minBound">Min range value (included)</param>
    /// <param name="maxBound">Max range value (included)</param>
    /// <param name="position">Position for checking (one dimension)</param>
    /// <param name="scale">Block size (one dimension)</param>
    /// <returns></returns>
    private bool IsPositionInBounds(float minBound, float maxBound, float position, float scale)
    {
        float calcMinBound = minBound + scale;
        float calcMaxBound = maxBound - scale;

        return position >= calcMinBound && position <= calcMaxBound;
    }

    // ABSTRACTION
    // POLYMORPHISM
    /// <summary>
    /// Is point on specified bounds?
    /// Calc based on block size and position
    /// </summary>
    /// <param name="point">Point for checking</param>
    /// <param name="bound">Bounds</param>
    public bool IsPositionInBounds(Vector3 point, AreaBound bound)
    {
        return IsPositionInBounds(bound.MinX, bound.MaxX, point.x, transform.localScale.x / 2)
            && IsPositionInBounds(bound.MinY, bound.MaxY, point.y, transform.localScale.y / 2);
    }

    #endregion

    #region Disabling

    /// <summary>
    /// Is block enable on palette?
    /// </summary>
    private bool isEnabled = true;

    /// <summary>
    /// Check enabled of block by remaing points in game
    /// Switching of field <see cref="isEnabled"/> and Material of block
    /// </summary>
    /// <param name="remainingPoints">Remaing points</param>
    private void CheckRemainingPoint(int remainingPoints)
    {
        isEnabled = remainingPoints >= cost;

        GetComponent<Renderer>().material = (isEnabled) ? commonMaterial : dragMaterial;
    }

    #endregion

    #region Mouse Click Counting

    private int clickCount = 0;
    private float lastClickTime;
    private static readonly float clickDelay = 0.2f;

    private void OnMouseDown()
    {
        StartCoroutine(CountClick());
    }

    private IEnumerator CountClick()
    {
        clickCount++;
        float currentTime = Time.time;
        lastClickTime = currentTime;

        yield return new WaitForSeconds(clickDelay);

        if (currentTime == lastClickTime)
        {
            clickCount = 0;
        }
    }

    #endregion

    #region Showing Behavior

    /// <summary>
    /// Block moving on game area
    /// </summary>
    abstract protected void Move();

    /// <summary>
    /// Block moving on palette
    /// </summary>
    abstract protected void StaticMove();

    #endregion
}