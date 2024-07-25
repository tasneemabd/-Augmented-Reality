using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.EventSystems;
using System.Linq;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARPlaneManager))]
[RequireComponent(typeof(InitialData))]
public class PlaceOnPlane : MonoBehaviour
{
    ARRaycastManager m_RaycastManager;
    ARPlaneManager aRPlaneManager;
    GameObject arCamera;

    public static GameObject spawnedObject;
    public static GameObject percentageIndicator;
    public GameObject qualityControlButton;
    static Vector3 initialScale;
    public Vector3 startMarker;
    public Vector3 endMarker;
    public float speed = 100f;
    public float minimumTimetoDisplayObject = 1.2f;
    public float minimumDistanceObjectPlaceFromCamera = 0.3f;
    public static bool isObjectPlaced = false;

    private float startTime;
    private float journeyLength;
    private float maxScaleNumber;
    private float speedR = 2.5f;
    private bool rotate = false;
    private bool wentToPosition = false;
    private float time = 0;
    private bool isPositioning = false;
    private bool gotMultipleTouchs = false;

    Quaternion toRotation;
    Quaternion fromRotation;
    Quaternion previousRotation;
    Vector3 previousPosition;
    Vector2 initialPosition = new Vector2(0, 0);
    Vector3 objectScreenPosition = new Vector2(0, 0);
    Vector2 distanceDifference = new Vector2(0, 0);

    private List<SpawningObj> spawningObj = new List<SpawningObj>();
    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    void Start()
    {
        InitialData._singleObjectPlacement = true;
        aRPlaneManager = FindObjectOfType<ARPlaneManager>();
        arCamera = GameObject.FindWithTag("MainCamera");
        m_RaycastManager = GetComponent<ARRaycastManager>();
        GameObject.FindWithTag("ScanSurfaceAnim").SetActive(true);
        isObjectPlaced = false;
        spawnedObject = Instantiate(InitialData.spawningObject);
        MeshRenderer[] allmeshRenders = spawnedObject.GetComponentsInChildren<MeshRenderer>();

        if (spawnedObject.GetComponent<SpawningObjectDetails>().EnableARQualityControl == true)
        {
            qualityControlButton.SetActive(true);
        }
        else
        {
            qualityControlButton.SetActive(false);
        }

        foreach (MeshRenderer mRender in allmeshRenders)
        {
            SpawningObj spj = new SpawningObj();
            spj.meshRenderer = mRender;
            Material[] mats = mRender.materials;
            int x = 0;
            spj.shaders = new Shader[mRender.materials.Length];
            foreach (Material m in mats)
            {
                if (m.shader.name != "AR/Occlusion")
                {
                    spj.shaders[x] = m.shader;
                    x++;
                    m.shader = (Shader)Resources.Load("TransparentShader", typeof(Shader));
                }
            }

            spawningObj.Add(spj);
        }

        aRPlaneManager.requestedDetectionMode = spawnedObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode;
        spawnedObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.transform.localScale = spawnedObject.GetComponent<Collider>().bounds.size * 0.0015f;
        spawnedObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.transform.position = new Vector3(0, spawnedObject.GetComponent<Collider>().bounds.size.y * 1.2f, 0);
        initialScale = spawnedObject.transform.localScale;
        spawnedObject.GetComponent<SpawningObjectDetails>().InitialScale = initialScale;
        KeepObjectWithInClippingValue(spawnedObject);
        spawnedObject.transform.parent = arCamera.transform.transform;
        spawnedObject.transform.position = arCamera.transform.position + new Vector3(0, 0, (spawnedObject.name == "ARPortal 1(Clone)") ? 3 : minimumDistanceObjectPlaceFromCamera);
        spawnedObject.SetActive(false);
        DelayToShowSpawnedObject();
    }

    void Update()
    {
        SpawnObjectOnPointPositionAndUpdatePosition();
        MultipleTouchHandler();
        FreezePositionWhenRotate();
        SendObjectToDetectedPosition();
    }

    /// <summary>
    /// Check hit position and spawn object and update object position change
    /// </summary>
    private void SpawnObjectOnPointPositionAndUpdatePosition()
    {
        if (!isObjectPlaced)
        {
            DelayToShowSpawnedObject();
            Vector3 rayEmitPosition = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            if (m_RaycastManager.Raycast(rayEmitPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                var hitPose = s_Hits[0].pose;
                fromRotation = spawnedObject.transform.rotation;
                if (aRPlaneManager.requestedDetectionMode == PlaneDetectionMode.Vertical)
                {
                    Quaternion orientation = Quaternion.identity;
                    Quaternion zUp = Quaternion.identity;
                    GetWallPlacement(s_Hits[0], out orientation, out zUp);
                    spawnedObject.transform.rotation = zUp;
                    toRotation = Quaternion.Euler(spawnedObject.transform.eulerAngles.x, spawnedObject.transform.rotation.eulerAngles.y, 0);
                }
                else
                {
                    toRotation = Quaternion.Euler(0, spawnedObject.transform.rotation.eulerAngles.y, 0);
                }

                spawnedObject.transform.parent = null;
                spawnedObject.transform.localScale = initialScale;
                spawnedObject.GetComponent<SpawningObjectDetails>().InitialScale = spawnedObject.transform.localScale;
                startTime = Time.time;
                startMarker = spawnedObject.transform.position;
                endMarker = hitPose.position;
                wentToPosition = true;
                journeyLength = Vector3.Distance(startMarker, endMarker);
                rotate = true;
                previousRotation = hitPose.rotation;
                previousPosition = hitPose.position;
                isObjectPlaced = true;

                if (spawnedObject.GetComponent<SpawningObjectDetails>().EnableShadowPlane)
                    spawnedObject.GetComponent<SpawningObjectDetails>().ShadowPlane.SetActive(true);

                GameObject.FindWithTag("ScanSurfaceAnim").SetActive(false);
                MeshRenderer[] allmeshRenders = spawnedObject.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer mRender in allmeshRenders)
                {
                    Material[] mats = mRender.materials;
                    SpawningObj temp = spawningObj.Where(obj => obj.meshRenderer.name == mRender.name).SingleOrDefault();

                    int x = 0;

                    foreach (Material m in mats)
                    {
                        if (m.shader.name != "AR/Occlusion")
                        {
                            m.shader = temp.shaders[x];
                            x++;
                        }
                    }

                    spawningObj.Remove(temp);
                }
            }
        }
        else
        {
            //Finding the Dragging position 
            if (TouchIndicatorHandler.isTouchedTheObject && (Input.touchCount < 2) && !gotMultipleTouchs)
            {
                if (!TryGetTouchPosition(out Vector2 touchPosition))
                    return;

                if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon) && !IsPointerOverUIObject())
                {
                    if (isPositioning)
                    {
                        var hitPose = s_Hits[0].pose;
                        if (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().EnableDragFeature)
                            TouchIndicatorHandler.hitObject.transform.position = hitPose.position;
                        previousPosition = hitPose.position;

                    }
                }
            }
        }
        if (TouchIndicatorHandler.isTouchedTheObject)
        {
            TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.transform.rotation = Quaternion.Euler(arCamera.transform.rotation.eulerAngles.x, arCamera.transform.rotation.eulerAngles.y, 0);
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
            distanceDifference = new Vector2(0, 0);
        }
        else if (Input.touchCount > 1)
        {
            gotMultipleTouchs = true;
            distanceDifference = new Vector2(0, 0);
        }
    }

    /// <summary>
    /// To Get the touch position
    /// </summary>
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                initialPosition = Input.GetTouch(0).position;
                objectScreenPosition = arCamera.GetComponent<Camera>().WorldToScreenPoint(spawnedObject.transform.position);
                distanceDifference = new Vector2(objectScreenPosition.x, objectScreenPosition.y) - initialPosition;
                touchPosition = Input.GetTouch(0).position + distanceDifference;
                return true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                if (distanceDifference != new Vector2(0, 0))
                {
                    isPositioning = true;
                    touchPosition = Input.GetTouch(0).position + distanceDifference;
                    return true;
                }
                else
                {
                    initialPosition = Input.GetTouch(0).position;
                    objectScreenPosition = arCamera.GetComponent<Camera>().WorldToScreenPoint(spawnedObject.transform.position);
                    distanceDifference = new Vector2(objectScreenPosition.x, objectScreenPosition.y) - initialPosition;
                    touchPosition = Input.GetTouch(0).position + distanceDifference;
                    return true;
                }

            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                TouchIndicatorHandler.isTouchedTheObject = false;
                isPositioning = false;
                touchPosition = default;
                return false;
            }
            else
            {
                touchPosition = default;
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
        if (rotate || wentToPosition)
        {
            spawnedObject.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, (Time.time - startTime) * speedR);
            if (spawnedObject.transform.rotation == toRotation)
            {
                rotate = false;
            }
            speed = Vector3.Distance(startMarker, endMarker);
            float distCovered = (Time.time - startTime) * speed * 100 * Time.deltaTime;
            float fractionOfJourney = distCovered / journeyLength;
            spawnedObject.transform.position = Vector3.Lerp(startMarker, endMarker, fractionOfJourney);
            if (spawnedObject.transform.position == endMarker)
            {
                wentToPosition = false;
                rotate = false;
                spawnedObject.transform.rotation = toRotation;
            }
        }
    }

    /// <summary>
    /// Hide the touch scale Percenntage indicator
    /// </summary>
    public static void HideScalePercentageIndicator()
    {
        TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.SetActive(false);
    }

    /// <summary>
    /// Hide the touch scale Percenntage indicator
    /// </summary>
    public static void ShowScalePercentageIndicator(string Percentage)
    {
        TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.SetActive(true);
        TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = Percentage + "%";
    }

    /// <summary>
    /// Hide the touch indicator gameobject
    /// </summary>
    public static void HideTouchIndicator()
    {
        if (isObjectPlaced)
        {
            spawnedObject.GetComponent<SpawningObjectDetails>().TouchIndicator.SetActive(false);
        }

    }

    /// <summary>
    /// Show the touch indicator gameobject
    /// </summary>
    public static void ShowTouchIndicator()
    {
        if (isObjectPlaced && !IsPointerOverUIObject() && spawnedObject.GetComponent<SpawningObjectDetails>().EnableTouchIndicator)
        {
            spawnedObject.GetComponent<SpawningObjectDetails>().TouchIndicator.SetActive(true);
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
    /// Delay the spawning object appear inftront of camera
    /// </summary>
    void DelayToShowSpawnedObject()
    {
        if (time > minimumTimetoDisplayObject)
        {
            if (spawnedObject != null) { 
            if (spawnedObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode == PlaneDetectionMode.Vertical)
            {
                spawnedObject.transform.eulerAngles = new Vector3(-90, spawnedObject.transform.eulerAngles.y, spawnedObject.transform.eulerAngles.z);
            }
            spawnedObject.SetActive(true);
            }
        }
        else
        {
            time += Time.deltaTime;
        }
    }

    /// <summary>
    /// Freeze the position when object is rotating
    /// </summary>
    void FreezePositionWhenRotate()
    {
        if (isObjectPlaced && (Input.touchCount > 1))
        {
            if (previousRotation != spawnedObject.transform.rotation)
            {
                spawnedObject.transform.position = previousPosition;
                previousRotation = spawnedObject.transform.rotation;
            }
            else if (previousRotation == spawnedObject.transform.rotation)
            {
                previousPosition = spawnedObject.transform.position;
            }
        }
    }

    /// <summary>
    /// Reset scale to initial scale
    /// </summary>
    public static void ResetToInitialScale()
    {
        spawnedObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.SetActive(false);
        spawnedObject.transform.localScale = spawnedObject.GetComponent<SpawningObjectDetails>().InitialScale;
    }

    /// <summary>
    /// Keep the object within in the camera clipping values 
    /// </summary>
    void KeepObjectWithInClippingValue(GameObject Obj)
    {
        Collider collider = Obj.GetComponent<Collider>();

        maxScaleNumber = Mathf.Max(collider.bounds.size.x, collider.bounds.size.y, collider.bounds.size.z);
        if (maxScaleNumber > 1)
        {
            Obj.transform.localScale = (Obj.transform.localScale / maxScaleNumber) * 0.1f;
        }
        else
        {
            Obj.transform.localScale = (1 / maxScaleNumber * Obj.transform.localScale) * 0.1f;
        }
    }
}


[System.Serializable]
public class SpawningObj
{
    public MeshRenderer meshRenderer;
    public Shader[] shaders;
}