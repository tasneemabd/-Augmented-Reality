using UnityEngine;
using UnityEngine.XR.ARSubsystems;

public class SpawningObjectDetails : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Select Object Plane detection mode")]
    private PlaneDetectionMode _planeDetectionMode;

    [SerializeField]
    [Tooltip("Remove tick if you dont need Draging feature")]
    bool _enbleDragFeature = true;

    [SerializeField]
    [Tooltip("Remove tick if you dont need Rotating feature")]
    bool _enableRotateFeature = true;

    [SerializeField]
    [Tooltip("Enter rotate factor")]
    int _rotateFactor = 750;

    [SerializeField]
    [Tooltip("Remove tick if you dont need Scaling feature")]
    bool _enableScaleFeature = true;

    [SerializeField]
    [Tooltip("Remove tick if you dont need Scaling feature")]
    bool _enableARQualityControl = true;

    [SerializeField]
    [Tooltip("Use lower scalling factor for increase the scalling speed")]
    int _scaleFactor = 400;

    [SerializeField]
    [Tooltip("Remove tick if you dont need shadow plane feature")]
    bool _enableShadowPlane = true;

    [SerializeField]
    [Tooltip("Drag and drop shadow palne here")]
    private GameObject _shadowPlane;

    [SerializeField]
    [Tooltip("Remove tick if you dont need touch indicator feature")]
    bool _enableTouchIndicator = true;

    [SerializeField]
    [Tooltip("Drag and drop Touch Indicator here")]
    private GameObject _touchIndicator;

    [SerializeField]
    [Tooltip("Drag and drop Scale perceantage here")]
    private GameObject _scalePercentageIndicator;

    private Vector3 _initialScale = new Vector3(0, 0, 0);
    private Vector3 _limitScaleValue = new Vector3(0, 0, 0);

    private Quaternion _initialRotation;
    private Quaternion _initialPlacedRotation;

    void Start()
    {
        _initialPlacedRotation = gameObject.transform.rotation;
        _initialScale = gameObject.transform.localScale;
        _shadowPlane.SetActive(false);
        _scalePercentageIndicator.SetActive(false);
        _touchIndicator.SetActive(false);
    }

    /// <summary>
    /// Enable/Disable Touch Indicator
    /// </summary>
    public bool EnableTouchIndicator
    {
        get { return _enableTouchIndicator; }
    }

    /// <summary>
    /// Enable/Disable Shadow Planes
    /// </summary>
    public bool EnableShadowPlane
    {
        get { return _enableShadowPlane; }
    }

    /// <summary>
    /// Enable/Disable Drag factor
    /// </summary>
    public bool EnableDragFeature
    {
        get { return _enbleDragFeature; }
    }

    /// <summary>
    /// Enable/Disable Rotation
    /// </summary>
    public bool EnableRotateFeature
    {
        get { return _enableRotateFeature; }
    }

    /// <summary>
    /// Enable/Disable Scaling
    /// </summary>
    public bool EnableScaleFeature
    {
        get { return _enableScaleFeature; }
    }

    /// <summary>
    /// Enable/Disable AR qulaity control feature
    /// </summary>
    public bool EnableARQualityControl
    {
        get { return _enableARQualityControl; }
    }

    /// <summary>
    /// Change rotate factor
    /// </summary>
    public int RotateFactor
    {
        get { return _rotateFactor; }
    }

    /// <summary>
    /// change scale factor
    /// </summary>
    public int ScaleFactor
    {
        get { return _scaleFactor; }
    }

    /// <summary>
    /// Get initial rotation after placed
    /// </summary>
    public Quaternion InitialPlacedRotation
    {
        get { return _initialPlacedRotation; }
        set { _initialPlacedRotation = value; }
    }

    /// <summary>
    /// Get initial rotation
    /// </summary>
    public Quaternion InitialRotation
    {
        get { return _initialRotation; }
        set { _initialRotation = value; }
    }

    /// <summary>
    /// Get initial scale
    /// </summary>
    public Vector3 InitialScale
    {
        get { return _initialScale; }
        set { _initialScale = value; }
    }

    /// <summary>
    /// Set limit to object scale 
    /// </summary>
    public Vector3 LimitScaleValue
    {
        get { return _limitScaleValue; }
        set { _limitScaleValue = value; }
    }

    /// <summary>
    /// Get object plane detection mode
    /// </summary>
    public PlaneDetectionMode PlaneDetectionMode
    {
        get { return _planeDetectionMode; }
        set { _planeDetectionMode = value; }
    }

    /// <summary>
    /// Get shadow plane gameobject
    /// </summary>
    public GameObject ShadowPlane
    {
        get { return _shadowPlane; }
        set { _shadowPlane = value; }
    }

    /// <summary>
    /// Get scale percentage value
    /// </summary>
    public GameObject ScalePercentageIndicator
    {
        get { return _scalePercentageIndicator; }
        set { _scalePercentageIndicator = value; }
    }

    /// <summary>
    /// Get touch indicator gameobject
    /// </summary>
    public GameObject TouchIndicator
    {
        get { return _touchIndicator; }
        set { _touchIndicator = value; }
    }

}
