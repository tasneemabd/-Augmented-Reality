using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

[RequireComponent(typeof(ARFace))]
public class ARKitBlendShapeVisualizerDog : MonoBehaviour
{
    [SerializeField]
    float m_CoefficientScale = 100.0f;

    public float coefficientScale
    {
        get { return m_CoefficientScale; }
        set { m_CoefficientScale = value; }
    }

    [SerializeField]
    SkinnedMeshRenderer m_SkinnedMeshRenderer;

    public SkinnedMeshRenderer skinnedMeshRenderer
    {
        get
        {
            return m_SkinnedMeshRenderer;
        }
        set
        {
            m_SkinnedMeshRenderer = value;
            CreateFeatureBlendMapping();
        }
    }

#if UNITY_IOS && !UNITY_EDITOR
        ARKitFaceSubsystem m_ARKitFaceSubsystem;
        Dictionary<ARKitBlendShapeLocation, int> m_FaceArkitBlendShapeIndexMap;
#endif

    ARFace m_Face;

    void Awake()
    {
        m_Face = GetComponent<ARFace>();
        CreateFeatureBlendMapping();
    }

    /// <summary>
    /// Setting up blendshape mapping
    /// </summary>
    void CreateFeatureBlendMapping()
    {
        if (skinnedMeshRenderer == null || skinnedMeshRenderer.sharedMesh == null)
        {
            return;
        }

#if UNITY_IOS && !UNITY_EDITOR
            const string strPrefix ="blendShape1.";
            m_FaceArkitBlendShapeIndexMap = new Dictionary<ARKitBlendShapeLocation, int>();
            m_FaceArkitBlendShapeIndexMap[ARKitBlendShapeLocation.JawOpen             ]   = skinnedMeshRenderer.sharedMesh.GetBlendShapeIndex(strPrefix + "jaw_open");
#endif
    }

    void SetVisible(bool visible)
    {
        if (skinnedMeshRenderer == null) return;

        skinnedMeshRenderer.enabled = visible;
    }

    void UpdateVisibility()
    {
        var visible =
            enabled &&
            (m_Face.trackingState == TrackingState.Tracking) &&
            (ARSession.state > ARSessionState.Ready);

        SetVisible(visible);
    }

    void OnEnable()
    {
#if UNITY_IOS && !UNITY_EDITOR
            var faceManager = FindObjectOfType<ARFaceManager>();
            if (faceManager != null)
            {
                m_ARKitFaceSubsystem = (ARKitFaceSubsystem)faceManager.subsystem;
            }
#endif
        UpdateVisibility();
        m_Face.updated += OnUpdated;
        ARSession.stateChanged += OnSystemStateChanged;
    }

    void OnDisable()
    {
        m_Face.updated -= OnUpdated;
        ARSession.stateChanged -= OnSystemStateChanged;
    }

    void OnSystemStateChanged(ARSessionStateChangedEventArgs eventArgs)
    {
        UpdateVisibility();
    }

    void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
    {
        UpdateVisibility();
        UpdateFaceFeatures();
    }

    /// <summary>
    /// Start updating face feature
    /// </summary>
    void UpdateFaceFeatures()
    {
        if (skinnedMeshRenderer == null || !skinnedMeshRenderer.enabled || skinnedMeshRenderer.sharedMesh == null)
        {
            return;
        }

#if UNITY_IOS && !UNITY_EDITOR
            using (var blendShapes = m_ARKitFaceSubsystem.GetBlendShapeCoefficients(m_Face.trackableId, Allocator.Temp))
            {
                foreach (var featureCoefficient in blendShapes)
                {
                    int mappedBlendShapeIndex;
                    if (m_FaceArkitBlendShapeIndexMap.TryGetValue(featureCoefficient.blendShapeLocation, out mappedBlendShapeIndex))
                    {
                        if (mappedBlendShapeIndex >= 0)
                        {
                            skinnedMeshRenderer.SetBlendShapeWeight(mappedBlendShapeIndex, featureCoefficient.coefficient * coefficientScale);
                        }
                    }
                }
            }
#endif
    }
}
