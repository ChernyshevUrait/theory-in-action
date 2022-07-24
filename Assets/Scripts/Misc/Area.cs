using System.Collections;
using UnityEngine;
using System.Linq;

//[ExecuteInEditMode]
[RequireComponent(typeof(LineRenderer))]
public class Area : MonoBehaviour
{
    /// <summary>
    /// Distance to screen sides or another <see cref="Area"/>
    /// </summary>
    private const float HorizontalMargin = 1;
    
    #region Inspector Properties

    /// <summary>
    /// Bounds of area on screen
    /// </summary>
    [Tooltip("Bounds of area on screen")]
    [SerializeField] private AreaBound m_AreaBounds;

    // ENCAPSULATION
    /// <summary>
    /// Bounds of area on screen
    /// </summary>
    [HideInInspector] public AreaBound AreaBounds
    {
        get { return m_AreaBounds; }

        private set
        {
            if (value.Equals(m_AreaBounds)) { return; }

            Vector3 offset = value.Center - m_AreaBounds.Center;
            m_AreaBounds = value;
            UpdateBlocks(offset);
        }
    }

    /// <summary>
    /// Width of area is fixed
    /// </summary>
    [Tooltip("Width of area is fixed")]
    [SerializeField] private bool isFixedWidth = false;

    /// <summary>
    /// Fixed fixedWidth of area
    /// </summary>
    [Tooltip("Fixed width of area")]
    [SerializeField] private float fixedWidth = 20;

    #endregion

    #region Show & Update Bounds

    protected virtual void Update()
    {
        FitBoundsToScrean();
    }

    // ABSTRACTION
    /// <summary>
    /// Intialize positions of <see cref="LineRenderer"/> to display area bounds
    /// </summary>
    protected virtual void Initialize()
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.25f;
        lineRenderer.endWidth = 0.25f;

        lineRenderer.positionCount = 5;
        lineRenderer.SetPosition(0, new Vector3(AreaBounds.MinX, AreaBounds.MinY, 0));
        lineRenderer.SetPosition(1, new Vector3(AreaBounds.MinX, AreaBounds.MaxY, 0));
        lineRenderer.SetPosition(2, new Vector3(AreaBounds.MaxX, AreaBounds.MaxY, 0));
        lineRenderer.SetPosition(3, new Vector3(AreaBounds.MaxX, AreaBounds.MinY, 0));
        lineRenderer.SetPosition(4, new Vector3(AreaBounds.MinX, AreaBounds.MinY, 0));
    }

    // ABSTRACTION
    /// <summary>
    /// Fit <see cref="AreaBounds"/> of all <see cref="Area"/> to screen for different aspect ratio;
    /// </summary>
    private static void FitBoundsToScrean()
    {
        Camera camera = Camera.main;

        float screenMinX = camera.ScreenToWorldPoint(new(0, 0, 0)).x;
        float screenMaxX = camera.ScreenToWorldPoint(new(camera.pixelWidth, 0, 0)).x;

        IOrderedEnumerable<Area> orderedAreas = GameObject.FindObjectsOfType<Area>().OrderBy(a => a.AreaBounds.MinX);
        
        float sumFixedWidth = orderedAreas
                .Where(a => a.isFixedWidth)
                .Sum(a => HorizontalMargin + a.AreaBounds.MaxX - a.AreaBounds.MinX);
        float sumFloatWidth = screenMaxX - screenMinX - sumFixedWidth - HorizontalMargin;
        float floatWidth = sumFloatWidth - orderedAreas.Where(a => !a.isFixedWidth).Count() * HorizontalMargin / orderedAreas.Where(a => !a.isFixedWidth).Count();

        foreach (Area area in orderedAreas)
        {
            float offset = orderedAreas
                .Where(a => a.AreaBounds.MaxX < area.AreaBounds.MaxX)
                .Sum(a => HorizontalMargin + a.AreaBounds.MaxX - a.AreaBounds.MinX);

            float minX = screenMinX + HorizontalMargin + offset;
            float maxX = minX + ((area.isFixedWidth) ? area.fixedWidth : floatWidth);

            area.AreaBounds = new(minX, maxX, area.AreaBounds.MinY, area.AreaBounds.MaxY);
            area.Initialize();
        }
    }

    /// <summary>
    /// Update all <see cref="Block"/> in area
    /// </summary>
    private void UpdateBlocks(Vector3 offset)
    {
        Block[] blocks = transform.GetComponentsInChildren<Block>();

        foreach (Block block in blocks)
        {

            block.transform.Translate(offset, Space.World);
            if (!block.IsPositionInBounds(block.transform.position, AreaBounds))
            {
#if UNITY_EDITOR
                DestroyImmediate(block.gameObject);
#else
                block.Delete();
#endif
            }
        }
    } 

    #endregion
}
