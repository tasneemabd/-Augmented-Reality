using UnityEngine;

/// <summary>
/// To detect Double tap
/// </summary>
public class DoubleTap : MonoBehaviour
{
    private int tapCount = 0;
    private bool waitingTimeStarted = false;
    private float time = 0;
    private float waitingTime = 0.5f;

    private const float MINIMUMTIMETOSECONDTAPSTART = 0.2f;

    void Update()
    {
        // Handle screen touches
        if (Input.touchCount == 1)
        {
            if (!waitingTimeStarted)
            {
                time = 0;
                tapCount = 0;
            }
            waitingTimeStarted = true;
            Touch touch = Input.GetTouch(0);
            if (tapCount < 1)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    tapCount++;
                }
                else if (touch.phase == TouchPhase.Moved && time > MINIMUMTIMETOSECONDTAPSTART)
                {
                    tapCount = 0;
                    time = 0;
                }
            }
            else
            {
                if (touch.phase == TouchPhase.Began)
                {
                    tapCount++;
                }
                else if (touch.phase == TouchPhase.Moved && time > MINIMUMTIMETOSECONDTAPSTART)
                {
                    tapCount = 0;
                    time = 0;
                }

            }

            if (time <= waitingTime && tapCount > 1)
            {
                //Double Tap trigger in here
                if (InitialData._singleObjectPlacement)
                {
                    PlaceOnPlane.ResetToInitialScale();
                }
                else
                {
                    MultipleObjectPlacement.ResetToInitialScale();
                }
                tapCount = 0;
                time = 0;
            }
        }

        if (time > waitingTime)
        {
            time = 0;
            waitingTimeStarted = false;
            tapCount = 0;
        }
        else
        {
            if (waitingTimeStarted)
            {
                time += Time.deltaTime;
            }
        }
    }
}