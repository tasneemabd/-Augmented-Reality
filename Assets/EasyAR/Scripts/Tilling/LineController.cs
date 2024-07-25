using UnityEngine;

/// <summary>
/// Line controller for set position
/// </summary>
public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform[] points;

    void Awake() 
    {
        lineRenderer = GetComponent<LineRenderer>();      
    }

    void Update()
    {
        if (lineRenderer.positionCount > 0)
        {
            for (int i = 0; i < points.Length; i++)
            {
                lineRenderer.SetPosition(i, points[i].position);
            }
        }
    }

    /// <summary>
    /// Create line using points
    /// </summary>
    public void SetupLine(Transform[] points)
    {        
        lineRenderer.positionCount = points.Length;
        this.points = points;
    }
}
