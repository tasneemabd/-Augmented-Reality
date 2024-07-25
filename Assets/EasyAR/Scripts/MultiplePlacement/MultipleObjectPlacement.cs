using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;

/// <summary>
/// Required components for this component.
/// </summary>
[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(AREnvironmentProbeManager))]
[RequireComponent(typeof(InitialData))]
public class MultipleObjectPlacement : MonoBehaviour
{
    [SerializeField]
    GameObject pointerIndicator;
    [SerializeField]
    GameObject canvas;
    GameObject _PointerIndicator;
    GameObject aRCamera;
    GameObject notification;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();
    static Vector3 initialScale;

    ARRaycastManager m_RaycastManager;
    ARPlaneManager aRPlaneManager;

    Vector2 initialPosition = new Vector2(0, 0);
    Vector3 ObjectScreenPosition = new Vector2(0, 0);
    Vector2 DistanceDifference = new Vector2(0, 0);
    Quaternion previousRotation;
    Vector3 previousPosition;

    private float startTime;
    private float startTimeLiftDown;
    private float journeyLength;
    private float speedS = 5f;
    private bool isPositioning = false;
    private bool gotMultipleTouchs = false;
    private bool wentToPosition = false;
    private bool wentToScale = false;
    private string detectedPlaneType = "";
    private bool iscalledToSpawn = false;
    private bool startTimeSet = false;
    private bool hideIndicator = false;

    public float speed = 100f;
    public Vector3 startMarker;
    public Vector3 endMarker;
    public static GameObject percentageIndicator;
    public static GameObject percentageIndicatorPrefab;
    public static GameObject scanSurface;
    public static GameObject spawnedObject;
    public static bool isObjectPlaced = false;

    void Start()
    {
        InitialData._singleObjectPlacement = false;
        aRPlaneManager = FindObjectOfType<ARPlaneManager>();
        aRCamera = GameObject.FindWithTag("MainCamera");
        m_RaycastManager = GetComponent<ARRaycastManager>();
        scanSurface = GameObject.FindWithTag("ScanSurfaceAnim");
        notification = GameObject.FindWithTag("NotificationPanel");
        scanSurface.SetActive(true);
    }

    void Update()
    {
        PointerIndicatorUpdate();
        ObjectPositionUpdate();
        ScalePersentageEditorRotationUpdate();
        MultipleTouchHandler();
        FreezePositionWhenRotate();
        SendObjectToDetectedPosition();
    }

    /// <summary>
    /// Update pointer indictor position,scale and rotation according to object placement vertical or horizontal
    /// </summary>
    public void PointerIndicatorUpdate()
    {
        if (!TouchIndicatorHandler.isTouchedTheObject)
        {
            // Raycast from middle in the screen and show pointer place where raycast collider hit surafce
            Vector3 rayEmitPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            if (m_RaycastManager.Raycast(rayEmitPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = s_Hits[0].pose;
                if (_PointerIndicator == null) { _PointerIndicator = Instantiate(pointerIndicator); }
                if (_PointerIndicator.activeSelf == false && !hideIndicator) { _PointerIndicator.SetActive(true); }
                _PointerIndicator.transform.position = hitPose.position;
                _PointerIndicator.transform.rotation = hitPose.rotation;
                scanSurface.SetActive(false);

                // Resize pointer scale according to distance between camera and surface
                if (Vector3.Distance(aRCamera.transform.position, hitPose.position) < 0.8f)
                {
                    _PointerIndicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
                else
                {
                    _PointerIndicator.transform.localScale = new Vector3(1, 1, 1);
                }

                // This section check whether placement vertical or horizontal, if placement vertical set gameobject direction to up
                if (iscalledToSpawn && ((PlaneRecognizor(s_Hits[0].trackable.transform) == spawnedObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode.ToString()) ||
                    spawnedObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode.ToString() == "Everything"))
                {
                    isObjectPlaced = true;
                    spawnedObject.SetActive(true);
                    if (spawnedObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode.ToString() == "Vertical")
                    {
                        Quaternion orientation = Quaternion.identity;
                        Quaternion zUp = Quaternion.identity;
                        GetWallPlacement(s_Hits[0], out orientation, out zUp);
                        spawnedObject.transform.rotation = zUp;
                    }
                    else
                    {
                        spawnedObject.transform.rotation = hitPose.rotation;
                    }

                    spawnedObject.GetComponent<SpawningObjectDetails>().InitialPlacedRotation = spawnedObject.transform.rotation;
                    previousRotation = hitPose.rotation;
                    previousPosition = hitPose.position;
                    spawnedObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                    spawnedObject.transform.localPosition = previousPosition + new Vector3(0, spawnedObject.GetComponent<Collider>().bounds.size.y / 4, 0);
                    startMarker = spawnedObject.transform.position + new Vector3(0, spawnedObject.GetComponent<Collider>().bounds.size.y / 4, 0);
                    endMarker = previousPosition;
                    journeyLength = Vector3.Distance(startMarker, endMarker);
                    startTime = Time.time;
                    //rotate = true;
                    wentToScale = true;
                    iscalledToSpawn = false;

                }

                // Show notification if user trying to place gameobject wrong direction
                // for example try to place wall art(vertical placement object) on floor(horizontal surface)
                if (iscalledToSpawn && (PlaneRecognizor(s_Hits[0].trackable.transform) != spawnedObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode.ToString()))
                {
                    notification.transform.GetChild(0).GetComponent<Text>().text = "Scan a " + spawnedObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode + " suface";
                    notification.GetComponent<Animator>().Play("notificationAnim");
                    iscalledToSpawn = false;
                    spawnedObject = null;
                }
            }
        }
        else
        {
            _PointerIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Finding the Dragging position 
    /// </summary>
    public void ObjectPositionUpdate()
    {
        if (TouchIndicatorHandler.isTouchedTheObject && (Input.touchCount < 2) && !gotMultipleTouchs)
        {
            if (!TryGetTouchPosition(out Vector2 touchPosition))
                return;

            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                if (isPositioning)
                {
                    var hitPose = s_Hits[0].pose;
                    if (TouchIndicatorHandler.hitObject != null &&
                    (PlaneRecognizor(s_Hits[0].trackable.transform) == TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode.ToString() ||
                    spawnedObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode.ToString() == "Everything"))
                    {
                        if (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().EnableDragFeature)
                            TouchIndicatorHandler.hitObject.transform.position = hitPose.position;
                    }
                    previousPosition = hitPose.position;
                }
            }
        }
    }

    /// <summary>
    /// Scale persentage indicator look towards AR camera
    /// </summary>
    public void ScalePersentageEditorRotationUpdate()
    {
        if (TouchIndicatorHandler.isTouchedTheObject && TouchIndicatorHandler.hitObject != null)
        {
            TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.transform.rotation = Quaternion.Euler(aRCamera.transform.rotation.eulerAngles.x, aRCamera.transform.rotation.eulerAngles.y, 0);
        }
    }

    /// <summary>
    /// Create rotation with given forward and up direction
    /// </summary>
    private void GetWallPlacement(ARRaycastHit _planeHit, out Quaternion orientation, out Quaternion zUp)
    {
        TrackableId planeHit_ID = _planeHit.trackableId;
        ARPlane planeHit = aRPlaneManager.GetPlane(planeHit_ID);
        Vector3 planeNormal = planeHit.normal;
        orientation = Quaternion.FromToRotation(Vector3.up, planeNormal);
        Vector3 forward = _planeHit.pose.position - (_planeHit.pose.position + Vector3.down);
        zUp = Quaternion.LookRotation(forward, planeNormal);
    }

    /// <summary>
    /// Handle multiple touches
    /// </summary>
    void MultipleTouchHandler()
    {
        if (Input.touchCount == 0)
        {
            gotMultipleTouchs = false;
            DistanceDifference = new Vector2(0, 0);
        }
        else if (Input.touchCount > 1)
        {
            gotMultipleTouchs = true;
            DistanceDifference = new Vector2(0, 0);
        }
    }

    /// <summary>
    /// To Get the touch position
    /// </summary>
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {

        if (Input.touchCount == 1 && TouchIndicatorHandler.hitObject != null)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                initialPosition = Input.GetTouch(0).position;
                ObjectScreenPosition = aRCamera.GetComponent<Camera>().WorldToScreenPoint(TouchIndicatorHandler.hitObject.transform.position);
                DistanceDifference = new Vector2(ObjectScreenPosition.x, ObjectScreenPosition.y) - initialPosition;
                touchPosition = Input.GetTouch(0).position + DistanceDifference;
                return true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (DistanceDifference != new Vector2(0, 0))
                {
                    isPositioning = true;
                    touchPosition = Input.GetTouch(0).position + DistanceDifference;
                    return true;
                }
                else
                {
                    initialPosition = Input.GetTouch(0).position;
                    ObjectScreenPosition = aRCamera.GetComponent<Camera>().WorldToScreenPoint(TouchIndicatorHandler.hitObject.transform.position);
                    DistanceDifference = new Vector2(ObjectScreenPosition.x, ObjectScreenPosition.y) - initialPosition;
                    touchPosition = Input.GetTouch(0).position + DistanceDifference;
                    return true;
                }
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                TouchIndicatorHandler.isTouchedTheObject = false;
                isPositioning = false;
                initialPosition = new Vector2(0, 0);
                touchPosition = default;
                return false;
            }
            else
            {
                touchPosition = default;
                initialPosition = new Vector2(0, 0);
                return false;
            }
        }
        else
        {
            TouchIndicatorHandler.isTouchedTheObject = false;
            isPositioning = false;
        }
        touchPosition = default;
        return false;
    }

    /// <summary>
    /// Sending the object to the detected position
    /// </summary>
    void SendObjectToDetectedPosition()
    {
        if (wentToPosition || wentToScale)
        {

            if (spawnedObject.transform.localScale == initialScale || spawnedObject.transform.localScale.magnitude > initialScale.magnitude)
            {
                spawnedObject.transform.localScale = initialScale;
                wentToScale = false;
                wentToPosition = true;
                speed = Vector3.Distance(startMarker, endMarker);
                startMarker = spawnedObject.transform.position;
                spawnedObject.GetComponent<SpawningObjectDetails>().InitialScale = spawnedObject.transform.localScale;
                if (spawnedObject.GetComponent<SpawningObjectDetails>().EnableShadowPlane)
                {
                    spawnedObject.GetComponent<SpawningObjectDetails>().ShadowPlane.SetActive(true);
                }
                else
                {
                    spawnedObject.GetComponent<SpawningObjectDetails>().ShadowPlane.SetActive(false);
                }
                if ((Time.time - startTimeLiftDown) > 0.15f)
                {
                    if (!startTimeSet) { startTime = Time.time; startTimeSet = true; }
                    float distCovered = (Time.time - startTime) * speed * 300 * Time.deltaTime;
                    float fractionOfJourney = distCovered / journeyLength;
                    spawnedObject.transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
                    if ((int)(spawnedObject.transform.position.magnitude - endMarker.magnitude) == 0)
                    {
                        spawnedObject.transform.position = endMarker;
                        wentToPosition = false;
                        startTimeSet = false;
                        //rotate = false;
                    }
                }

            }
            else
            {
                spawnedObject.transform.localScale = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.1f), initialScale, (Time.time - startTime) * speedS);
                startTimeLiftDown = Time.time;
            }
        }
    }

    /// <summary>
    /// Hide the touch scale Percenntage indicator
    /// </summary>
    public static void HideScalePercentageIndicator()
    {
        if (TouchIndicatorHandler.hitObject != null)
        {
            TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Hide the touch scale Percenntage indicator
    /// </summary>
    public static void ShowScalePercentageIndicator(string Percentage)
    {
        if (TouchIndicatorHandler.hitObject != null)
        {
            TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.SetActive(true);
            TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = Percentage + "%";

        }
    }
    /// <summary>
    /// Hide the touch indicator gameobject
    /// </summary>
    public static void HideTouchIndicator()
    {
        if (TouchIndicatorHandler.hitObject != null)
        {
            TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().TouchIndicator.SetActive(false);
        }

    }

    /// <summary>
    /// Check whether user touch UI object or not
    /// </summary>
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    /// <summary>
    /// Show the touch indicator gameobject
    /// </summary>
    public static void ShowTouchIndicator()
    {
        if (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>() != null)
        {
            if (isObjectPlaced && TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().EnableTouchIndicator)
            {
                TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().TouchIndicator.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Freeze the position when object is rotating
    /// </summary>
    void FreezePositionWhenRotate()
    {
        if (isObjectPlaced && (Input.touchCount > 1))
        {
            if (TouchIndicatorHandler.hitObject != null && spawnedObject != null)
            {
                if (previousRotation != spawnedObject.transform.rotation)
                {
                    TouchIndicatorHandler.hitObject.transform.position = previousPosition;
                    previousRotation = TouchIndicatorHandler.hitObject.transform.rotation;
                    TouchIndicatorHandler.isTouchedTheObject = true;
                }
                else if (previousRotation == TouchIndicatorHandler.hitObject.transform.rotation)
                {
                    previousPosition = TouchIndicatorHandler.hitObject.transform.position;
                }
            }
        }
    }

    /// <summary>
    /// Reset scale to initial scale
    /// </summary>
    public static void ResetToInitialScale()
    {
        if (TouchIndicatorHandler.hitObject != null)
            TouchIndicatorHandler.hitObject.transform.localScale = TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale;
    }

    /// <summary>
    /// Instabciate the choosed object to spawn
    /// </summary>
    public void SpawnObject(GameObject go)
    {
        iscalledToSpawn = true;
        isObjectPlaced = false;
        spawnedObject = Instantiate(go);
        initialScale = spawnedObject.transform.localScale;
        spawnedObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.transform.localScale = spawnedObject.GetComponent<Collider>().bounds.size * 0.0015f;
        spawnedObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.transform.position = new Vector3(0, spawnedObject.GetComponent<Collider>().bounds.size.y * 1.2f, 0);
        spawnedObject.SetActive(false);
    }

    /// <summary>
    /// Detect the ray hit plane type
    /// </summary>
    string PlaneRecognizor(Transform ArPlaneTransform)
    {
        if ((Mathf.Round(ArPlaneTransform.eulerAngles.x) == 0 || Mathf.Round(ArPlaneTransform.eulerAngles.x) == 360) && (Mathf.Round(ArPlaneTransform.eulerAngles.z) == 0 || Mathf.Round(ArPlaneTransform.eulerAngles.z) == 360))
        {
            detectedPlaneType = "Horizontal";
        }
        else
        {
            detectedPlaneType = "Vertical";
        }
        return detectedPlaneType;
    }

    /// <summary>
    /// Pointer indicator handler
    /// </summary>
    public void ShowHideCanvas()
    {
        hideIndicator = !hideIndicator;
        canvas.SetActive(!canvas.activeSelf);
        _PointerIndicator.SetActive(!_PointerIndicator.activeSelf);
    }
}
