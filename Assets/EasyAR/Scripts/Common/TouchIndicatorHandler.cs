using UnityEngine;

public class TouchIndicatorHandler : MonoBehaviour
{
    public static GameObject hitObject = null;
    public static GameObject previousHitObject;
    public static bool isTouchedTheObject;
    public float minimumTimeToDisablePreviousObjectIndicator = 0.6f;

    private bool startTimer = false;
    private bool interactable = false;
    private float startTime = 0;

    void Update()
    {
        if ((Input.touchCount > 0))
        {
            // Select raycast hit object
            if (Input.touches[0].phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit Hit;
                if (Physics.Raycast(ray, out Hit))
                {
                    if(Hit.transform.gameObject.GetComponent<SpawningObjectDetails>() != null)
                    {
                        hitObject = Hit.transform.gameObject;
                        interactable = true;
                    }
                }
                else
                {
                    interactable = false;
                }
            }

            // Show touch Indicator when user move object 
            if (interactable && (Input.touches[0].phase == TouchPhase.Stationary || Input.touches[0].phase == TouchPhase.Moved))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit Hit;
                if (Physics.Raycast(ray, out Hit))
                {
                    if (Hit.transform.gameObject.GetComponent<SpawningObjectDetails>() != null)
                    {
                        hitObject = Hit.transform.gameObject;
                        if (previousHitObject == null)
                        {
                            previousHitObject = hitObject;
                        }
                    }

                    if (InitialData._singleObjectPlacement)
                    {
                        PlaceOnPlane.ShowTouchIndicator();
                    }
                    else
                    {
                        if (hitObject != null)
                            MultipleObjectPlacement.ShowTouchIndicator();
                    }

                    isTouchedTheObject = true;
                }

            }
            else if (Input.touches[0].phase == TouchPhase.Ended)
            {
                interactable = false;

                if (InitialData._singleObjectPlacement)
                {
                    PlaceOnPlane.HideTouchIndicator();
                }
                else
                {
                    MultipleObjectPlacement.HideTouchIndicator();
                }

                startTimer = true;
            }
        }

        //Hide touch indicator
        if (Input.touches.Length != 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isTouchedTheObject = false;

                if (InitialData._singleObjectPlacement)
                {
                    PlaceOnPlane.HideTouchIndicator();
                }
                else
                {
                    MultipleObjectPlacement.HideTouchIndicator();
                }

                startTimer = true;
            }
        }
        else
        {
            if (!isTouchedTheObject)
            {

                if (InitialData._singleObjectPlacement)
                {
                    PlaceOnPlane.HideTouchIndicator();
                }
                else
                {
                    MultipleObjectPlacement.HideTouchIndicator();
                    MultipleObjectPlacement.HideScalePercentageIndicator();
                }

                startTimer = true;
            }
        }

        DisablePreviousObjectIndicatorAndPercentage();
    }

    /// <summary>
    /// Disable previous selected object's touch indicator and percentage value canvas
    /// </summary>
    void DisablePreviousObjectIndicatorAndPercentage()
    {
       
        if (previousHitObject != hitObject && previousHitObject != null)
        {
            previousHitObject.GetComponent<SpawningObjectDetails>().TouchIndicator.SetActive(false);
            previousHitObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.SetActive(false);
            previousHitObject = hitObject;
        }

        if (startTimer)
        {
            if ((Time.time - startTime) > minimumTimeToDisablePreviousObjectIndicator)
            {
                if (hitObject != null)
                {
                    hitObject.GetComponent<SpawningObjectDetails>().TouchIndicator.SetActive(false);
                    hitObject.GetComponent<SpawningObjectDetails>().ScalePercentageIndicator.SetActive(false);
                }
                hitObject = null;
                startTimer = false;
            }
        }
        else
        {
            startTime = Time.time;
        }
    }

}