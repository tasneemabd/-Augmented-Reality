using System;
using UnityEngine;

public class ScaleRotateHandler : MonoBehaviour
{
    Vector3 minimumValue;
    Vector3 maximumValue;
    Vector3 initialScale;

    float initialDistance;
    int scaleFactor;
    float currentDistance;
    float previousDistance;
    bool isScalling = false;
    bool calledForScalling = false;
    bool withInScaleLimit = true;
    int rotateFactor;
    float initalRotation;
    float currentRotation = 0;
    float previousRotation = 0;
    bool isRotating = false;
    bool calledForRotating = false;
    bool isRotationEnabled = true;
    bool isScalingEnabled = true;

    public float maximumObjectScaleLimit = 40f;
    public float maximumObjectRotationLimit = 0.065f;

    private const float MINIMUMSCALEMULTIPLYERTOUCHINDICATOR = 0.4f;
    private const float MAXIMUMSCALEMULTIPLYERTOUCHINDICATOR = 10f;

    void Update()
    {
        if (TouchIndicatorHandler.hitObject != null)
        {
            scaleFactor = TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().ScaleFactor;
            minimumValue = TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale * MINIMUMSCALEMULTIPLYERTOUCHINDICATOR;
            maximumValue = TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale * MAXIMUMSCALEMULTIPLYERTOUCHINDICATOR;
            rotateFactor = TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().RotateFactor;
            isRotationEnabled = TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().EnableRotateFeature;
            isScalingEnabled = TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().EnableScaleFeature;
            LimitScale(minimumValue, maximumValue);

            if (Input.touchCount == 0)
            {
                isScalling = false;
                isRotating = false;
            }

            if (Input.touchCount > 1)
            {
                if (Input.GetTouch(1).phase == TouchPhase.Began)
                {
                    initialDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    initalRotation = Angle(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    currentDistance = initialDistance;
                    currentRotation = initalRotation;
                    previousDistance = initialDistance;
                    initialScale = TouchIndicatorHandler.hitObject.transform.localScale;
                    previousRotation = initalRotation;
                    isRotating = false;
                    isScalling = false;
                }
                else if (Input.GetTouch(1).phase == TouchPhase.Moved || Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    if (initialDistance == 0 || initalRotation == 0)
                    {
                        initialScale = TouchIndicatorHandler.hitObject.transform.localScale;
                        initialDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                        initalRotation = Angle(Input.GetTouch(0).position, Input.GetTouch(1).position);
                        currentDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                        currentRotation = Angle(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    }
                    else
                    {
                        currentDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                        currentRotation = Angle(Input.GetTouch(0).position, Input.GetTouch(1).position);
                    }
                    if (isRotationEnabled)
                    {
                        CheckRotation();
                    }
                    if (isScalingEnabled)
                    {
                        CheckScaling();
                    }
                    if (isScalling)
                    {
                        ChangeScale(GetScaleFactor(currentDistance));
                        if (InitialData._singleObjectPlacement)
                        {
                            PlaceOnPlane.ShowScalePercentageIndicator("" + (int)((TouchIndicatorHandler.hitObject.transform.localScale.magnitude / TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.magnitude) * 100f));
                        }
                        else
                        {
                            MultipleObjectPlacement.ShowScalePercentageIndicator("" + (int)((TouchIndicatorHandler.hitObject.transform.localScale.magnitude / TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.magnitude) * 100f));
                        }
                    }
                    else
                    {
                        if (InitialData._singleObjectPlacement)
                        {
                            PlaceOnPlane.HideScalePercentageIndicator();
                        }
                        else
                        {
                            MultipleObjectPlacement.HideScalePercentageIndicator();
                        }
                    }
                    if (isRotating)
                    {
                        ChangeRotation(GetRotateFactor(currentRotation));
                    }
                }
                else if (Input.GetTouch(1).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Ended)
                {
                    isScalling = false;
                    isRotating = false;
                    calledForRotating = false;
                    calledForScalling = false;
                    initialDistance = 0;
                    initalRotation = 0;
                    currentDistance = 0;
                    currentRotation = 0;
                }
            }
        }
        else
        {
            isScalling = false;
            isRotating = false;
            calledForRotating = false;
            calledForScalling = false;
            currentDistance = 0;
            currentRotation = 0;
            initialDistance = 0;
            initalRotation = 0;

            if (withInScaleLimit)
            {
                if (InitialData._singleObjectPlacement && TouchIndicatorHandler.hitObject != null)
                {
                    PlaceOnPlane.HideScalePercentageIndicator();
                }
                else if (TouchIndicatorHandler.hitObject != null)
                {
                    MultipleObjectPlacement.HideScalePercentageIndicator();
                }
            }
        }
    }

    /// <summary>
    /// Return the genarated scale factor
    /// </summary>
    void CheckScaling()
    {
        if ((Math.Abs(currentDistance - initialDistance) < maximumObjectScaleLimit) && !calledForScalling)
        {
            previousDistance = currentDistance;
            isScalling = false;
        }
        else if (!isRotating)
        {
            isScalling = true;
            calledForScalling = true;
            calledForRotating = false;
        }
    }

    /// <summary>
    /// Check whether user is performing rotation
    /// </summary>
    void CheckRotation()
    {
        if (Math.Abs(currentRotation - initalRotation) < maximumObjectRotationLimit && !calledForRotating)
        {
            isRotating = false;
            previousRotation = currentRotation;
        }
        else if (!isScalling)
        {
            isRotating = true;
            calledForRotating = true;
            calledForScalling = false;
        }
    }

    /// <summary>
    /// Getting scale factor
    /// </summary>
    float GetScaleFactor(float current)
    {
        float value;
        if (previousDistance == current)
        {
            value = 0;
        }
        else
        {
            value = (current - previousDistance) / scaleFactor;
            previousDistance = current;
        }
        return value;
    }

    /// <summary>
    /// Changed the scale of the object
    /// </summary>
    void ChangeScale(float factor)
    {
        if (!isRotating && withInScaleLimit)
        {
            if (factor > 0)
            {
                TouchIndicatorHandler.hitObject.transform.localScale = TouchIndicatorHandler.hitObject.transform.localScale + initialScale * (factor);
            }
            else
            {
                TouchIndicatorHandler.hitObject.transform.localScale = TouchIndicatorHandler.hitObject.transform.localScale + initialScale * (factor) * 0.7f;
            }
        }
    }

    /// <summary>
    /// Keep the object scale within the minimum and the maximum scale
    /// </summary>
    void LimitScale(Vector3 min, Vector3 max)
    {
        if (TouchIndicatorHandler.hitObject != null)
        {
            if (TouchIndicatorHandler.hitObject.transform.localScale.x < (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.x * 0.4f) ||
            TouchIndicatorHandler.hitObject.transform.localScale.y < (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.y * 0.4f) ||
            TouchIndicatorHandler.hitObject.transform.localScale.z < (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.z * 0.4f))
            {
                TouchIndicatorHandler.hitObject.transform.localScale = TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale * 0.4f;
            }
            else if ((TouchIndicatorHandler.hitObject.transform.localScale.x > (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.x * 12) ||
                TouchIndicatorHandler.hitObject.transform.localScale.y > (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.y * 12) ||
                TouchIndicatorHandler.hitObject.transform.localScale.z > (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.z * 12)))
            {
                TouchIndicatorHandler.hitObject.transform.localScale = TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale * 12f;
                withInScaleLimit = false;
            }
            else if ((TouchIndicatorHandler.hitObject.transform.localScale.x > (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.x * 10) ||
                TouchIndicatorHandler.hitObject.transform.localScale.y > (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.y * 10) ||
                TouchIndicatorHandler.hitObject.transform.localScale.z > (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.z * 10)))
            {
                if (InitialData._singleObjectPlacement)
                {
                    TouchIndicatorHandler.hitObject.transform.localScale -= TouchIndicatorHandler.hitObject.transform.localScale * Time.deltaTime;
                    PlaceOnPlane.ShowScalePercentageIndicator("" + (int)((TouchIndicatorHandler.hitObject.transform.localScale.magnitude / TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.magnitude) * 100f));
                }
                else
                {
                    TouchIndicatorHandler.hitObject.transform.localScale -= TouchIndicatorHandler.hitObject.transform.localScale * Time.deltaTime;
                    MultipleObjectPlacement.ShowScalePercentageIndicator("" + (int)((TouchIndicatorHandler.hitObject.transform.localScale.magnitude / TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialScale.magnitude) * 100f));
                }
            }
            else
            {
                withInScaleLimit = true; ;
            }
        }
    }

    /// <summary>
    /// Return the genarated Rotating Factor
    /// </summary>
    float GetRotateFactor(float current)
    {
        float value;
        if (previousRotation == current)
        {
            value = 0;
        }
        else
        {
            value = (current - previousRotation) * rotateFactor;
            previousRotation = current;
        }
        return value;
    }

    /// <summary>
    /// Change the rotation of the object according to rotating factor
    /// </summary>
    void ChangeRotation(float factor)
    {
        if (withInScaleLimit)
        {
            {
                if (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode.ToString() == "Horizontal")
                {
                    TouchIndicatorHandler.hitObject.transform.rotation = Quaternion.Euler(0, TouchIndicatorHandler.hitObject.transform.rotation.eulerAngles.y + factor, 0);
                }
                else if (TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().PlaneDetectionMode.ToString() == "Vertical")
                {
                    TouchIndicatorHandler.hitObject.transform.rotation = Quaternion.Euler(TouchIndicatorHandler.hitObject.transform.rotation.eulerAngles.x + factor, TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialPlacedRotation.eulerAngles.y, TouchIndicatorHandler.hitObject.GetComponent<SpawningObjectDetails>().InitialPlacedRotation.eulerAngles.z);
                }
            }
        }
    }

    /// <summary>
    /// Calculate the angle
    /// </summary>
    float Angle(Vector2 one, Vector2 two)
    {
        return (one.x - two.x) / Mathf.Sqrt((one.x - two.x) * (one.x - two.x) + (one.y - two.y) * (one.y - two.y));
    }
}