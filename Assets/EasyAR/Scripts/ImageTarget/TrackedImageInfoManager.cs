using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;

public class TrackedImageInfoManager : MonoBehaviour
{
    private ARTrackedImageManager aRTrackedImageManager;

    public GameObject scaningAnimationObject;

    private void Awake()
    {
        aRTrackedImageManager = GetComponent<ARTrackedImageManager>();
    }

    void OnEnable() => aRTrackedImageManager.trackedImagesChanged += OnChanged;

    void OnDisable() => aRTrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (var newImage in eventArgs.added)
        {
            // Handle added event
            UpdateARObjects(newImage);
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            UpdateARObjects(updatedImage);
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
        }
    }

    void UpdateARObjects(ARTrackedImage trackedImage)
    {
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            // here is the code when the target image is not visible
            scaningAnimationObject.SetActive(false);
        }
        else
        {
            // here is the code when the target image is visible
            scaningAnimationObject.SetActive(true);
        }
    }
}
