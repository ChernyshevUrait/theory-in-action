using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System;

// ABSTRACTION
abstract public class Block : MonoBehaviour
{
    #region Inspector properties

    [SerializeField] private bool onPalette;
    [SerializeField] private int cost;

    #endregion

    #region Materials

    private Material commonMaterial;
    private Material dragMaterial;

    #endregion

    #region Events

    public static UnityEvent<int> onDelete = new();
    public static UnityEvent<int> onInstantiate = new();

    #endregion

    #region Unity Messages

    private void Start()
    {
        commonMaterial = GetComponent<Renderer>().material;
        dragMaterial = CreateTransparentMaterial(commonMaterial);

        if (onPalette && GameManager.Instance != null)
        {
            GameManager.Instance.onPiontsChenged.AddListener(CheckRemainingPoint);
        }

        CheckRemainingPoint(-1);
    }

    private void Update()
    {
        Action moveAction = (onPalette) ? StaticMove : Move;
        moveAction();
    }

    private void OnMouseDrag()
    {
        if (!isEnable) { return; }

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
        if (!isEnable) { return; }

        GetComponent<Renderer>().material = commonMaterial;

        if (clickCount == 2)
        {
            if (onPalette)
            {
                Instantiate();
            }
            else
            {
                Delete();
            }
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

        point.x = FitPositionToBounds(GameManager.Instance.GameAreaBounds.MinX, GameManager.Instance.GameAreaBounds.MaxX, point.x, transform.localScale.x / 2);
        point.y = FitPositionToBounds(GameManager.Instance.GameAreaBounds.MinY, GameManager.Instance.GameAreaBounds.MaxY, point.y, transform.localScale.y / 2);

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

    private void Delete()
    {
        Destroy(gameObject);

        onDelete.Invoke(cost);
    }

    #endregion

    #region Instantiate Block

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

    private Vector3 startDragPosition = Vector3.positiveInfinity;

    // ABSTRACTION
    // POLYMORPHISM
    private void Instantiate()
    {
        Vector3 instantiatePosition = (GameManager.Instance == null) ? Vector3.zero : GameManager.Instance.GameAreaBounds.Center;
        Instantiate(instantiatePosition);
    }

    // ABSTRACTION
    // POLYMORPHISM
    private void Instantiate(Vector3 instantiatePosition)
    {
        GameObject newBlock = Instantiate(gameObject, instantiatePosition, transform.rotation);
        newBlock.GetComponent<Block>().onPalette = false;

        onInstantiate.Invoke(cost);
    }

    private bool IsPositionInBounds(Vector3 point)
    {
        if (GameManager.Instance == null)
        {
            return true;
        }

        return IsPositionInBounds(GameManager.Instance.GameAreaBounds.MinX, GameManager.Instance.GameAreaBounds.MaxX, point.x, transform.localScale.x / 2) 
            && IsPositionInBounds(GameManager.Instance.GameAreaBounds.MinY, GameManager.Instance.GameAreaBounds.MaxY, point.y, transform.localScale.y / 2);
    }

    private bool IsPositionInBounds(float minBound, float maxBound, float position, float scale)
    {
        float calcMinBound = minBound + scale;
        float calcMaxBound = maxBound - scale;

        return position >= calcMinBound && position <= calcMaxBound;
    }


    #endregion

    #region Disabling

    private bool isEnable = true;

    private void CheckRemainingPoint(int remainingPoints)
    {
        isEnable = remainingPoints >= cost;

        GetComponent<Renderer>().material = (isEnable) ? commonMaterial : dragMaterial;
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

    abstract protected void Move();

    abstract protected void StaticMove();

    #endregion
}