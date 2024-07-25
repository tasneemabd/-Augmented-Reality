using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class BoundaryManager : MonoBehaviour
{
    [SerializeField]
    private ARSessionOrigin arSessionOrigin;
    [SerializeField]
    private ARRaycastManager arRaycastManager;
    [SerializeField]
    private LineController line;
    [SerializeField]
    private ShapeCreator shape;

    ARPlaneManager aRPlaneManager;
    LineRenderer lineRendererCom;

    private bool setShapeActive = false;
    private bool placementPoseIsValid = false;
    private float distance = 0;
    private int numOfPoints = 0;
    private float startingPointHeight = 0;
    private float scalingFactor = 2f;
    private float finalScale = 0;
    private Pose placementPose;
    private Vector3 previousPose = new Vector3(0, 0, 0);
    
    List<int> trangulation = new List<int>();
    private List<Transform> pointList = new List<Transform>();

    [SerializeField]
    TrackableType trackableType = TrackableType.PlaneWithinPolygon;

    public Button goButton;
    public GameObject placementIndicator;
    private GameObject arCamera;
    public GameObject place;
    public GameObject go;
    public GameObject back;
    public GameObject startPanel;
    public GameObject lineRenderer;
    public GameObject objectToPlace;
    public Material lineRendererShadowMtl;

    private const float INDICATORSCALEMULTIPLYER = 2;
    
    private void Awake()
    {
        aRPlaneManager = FindObjectOfType<ARPlaneManager>();
        aRPlaneManager.requestedDetectionMode = PlaneDetectionMode.Horizontal;
    }

    private void Start()
    {
        back.SetActive(true);
        go.SetActive(true);
        lineRenderer.SetActive(true);
        goButton.interactable = false;
        arCamera = GameObject.FindWithTag("MainCamera");
    }

    void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        UpdateDistanceFromCamera();
    }

    /// <summary>
    /// Place new points
    /// </summary>
    public void PlaceNewPoints()
    {
        if (placementPoseIsValid)
        {
            GameObject point;

            if (numOfPoints == 0)
            {
                point = Instantiate(objectToPlace, placementPose.position, placementPose.rotation);
                startingPointHeight = point.transform.position.y;
            }
            else
            {
                point = Instantiate(objectToPlace, new Vector3(placementPose.position.x, startingPointHeight, placementPose.position.z), placementPose.rotation);
            }

            pointList.Add(point.transform);
            numOfPoints += 1;

            if (numOfPoints == 3)
            {
                goButton.interactable = true;
            }

            if (numOfPoints > 2)
            {

                CheckTrangulation(numOfPoints - 1);
            }

            distance = Vector3.Distance(previousPose, placementPose.position);
            line.SetupLine(pointList.ToArray());
            previousPose = placementPose.position;
        }
    }

    /// <summary>
    /// Indicator size change according to camera disatance
    /// </summary>
    void UpdateDistanceFromCamera()
    {
        float cameraDistance = Vector3.Distance(arCamera.transform.position, placementPose.position);
        finalScale = cameraDistance * scalingFactor;
        placementIndicator.transform.localScale = new Vector3(finalScale / INDICATORSCALEMULTIPLYER, finalScale / INDICATORSCALEMULTIPLYER, finalScale / INDICATORSCALEMULTIPLYER);
    }

    /// <summary>
    /// Reload scene 
    /// </summary>
    public void ReloadScene()
    {
        goButton.interactable = false;
        place.SetActive(true);
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ARTilling");
        placementIndicator.SetActive(true);
    }

    /// <summary>
    /// Clear the scene
    /// </summary>
    public void Clear()
    {
        SceneManager.LoadScene("ARTilling");
    }

    /// <summary>
    /// Go to menu scene
    /// </summary>
    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        LoaderUtility.Deinitialize();
    }

    /// <summary>
    /// Create shape starting and setting up line width and material 
    /// </summary>
    [Obsolete]
    public void SetShape()
    {
        setShapeActive = true;
        shape.SetupShape(pointList.ToArray());
        shape.Trangulator(trangulation.ToArray());
        place.SetActive(false);
        lineRendererCom = lineRenderer.GetComponent<LineRenderer>();
        lineRendererCom.SetWidth(0.02f, 0.02f);
        lineRendererCom.material = lineRendererShadowMtl;
        GameObject[] targets = GameObject.FindGameObjectsWithTag("TileTargets");

        foreach (GameObject tar in targets)
        {
            tar.SetActive(false);
        }
    }

    /// <summary>
    /// Show indicator if placement position valid
    /// </summary>
    private void UpdatePlacementIndicator()
    {
        if (placementPoseIsValid && !setShapeActive)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(placementPose.position, placementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    /// <summary>
    /// Check placement position valid or not
    /// </summary>
    private void UpdatePlacementPose()
    {
        var screenCenter = arSessionOrigin.camera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        arRaycastManager.Raycast(screenCenter, hits, trackableType);
        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            placementPose = hits[0].pose;
            startPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Check trangulation direction clockwise or anti-clockwise
    /// </summary>
    private void CheckTrangulation(int point)
    {
        //trangulation - 0,1,2 for 3 points (least)
        if (pointList[2].position.x > pointList[0].position.x)
        {
            trangulation.Add(0);
            for (int i = 1; i >= 0; i--)
            {
                trangulation.Add(point - i);
            }
        }
        //trangulation - 0,2,1 for 3 points (least)
        else
        {
            trangulation.Add(0);
            for (int i = 0; i < 2; i++)
            {
                trangulation.Add(point - i);
            }
        }
    }
}