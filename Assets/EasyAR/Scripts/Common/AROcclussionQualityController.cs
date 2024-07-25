using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/// <summary>
/// This script check whether your device support AROcclussion or not and setup AROcclusion quality level
/// </summary>
[RequireComponent(typeof(AROcclusionManager))]
public class AROcclussionQualityController : MonoBehaviour
{
    private AROcclusionManager aROcclusionManager;
    public GameObject qualityPanel;
    private Transform bestOptionButton;
    private bool supportDepth = false;
    public GameObject warningPanel;

    private void Awake()
    {
        aROcclusionManager = GetComponent<AROcclusionManager>();
    }

    private void Start()
    {
        qualityPanel.SetActive(false);
        warningPanel.SetActive(false);
        bestOptionButton = qualityPanel.transform.Find("Best");

#if UNITY_IOS
        bestOptionButton.gameObject.GetComponent<Button>().enabled = false;
#endif

        var occlusionDescriptors = new List<XROcclusionSubsystemDescriptor>();
        SubsystemManager.GetSubsystemDescriptors(occlusionDescriptors);

        if (occlusionDescriptors.Count > 0)
        {
            foreach (var occlusionDescriptor in occlusionDescriptors)
            {
                //Check whether your device support AR Occlusion or not by checking support Depth API to your device
                if (occlusionDescriptor.supportsEnvironmentDepthImage || occlusionDescriptor.supportsEnvironmentDepthConfidenceImage
                    || occlusionDescriptor.supportsHumanSegmentationDepthImage || occlusionDescriptor.supportsHumanSegmentationStencilImage)
                {
                    supportDepth = true;
                }
            }
        }
    }

    /// <summary>
    /// Set AR Occlusion quality level to Medium settings
    /// </summary>
    public void ChangeQualityToMedium()
    {
        aROcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Medium;
        ShowQualityPanel(false);
    }

    /// <summary>
    /// Set AR Occlusion quality level to Fastest settings
    /// </summary>
    public void ChangeQualityToFastest()
    {
        aROcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Fastest;
        ShowQualityPanel(false);
    }

    /// <summary>
    /// Set AR Occlusion quality level to Fastest settings
    /// </summary>
    public void ChangeQualityToBest()
    {
        aROcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Best;
        ShowQualityPanel(false);
    }

    /// <summary>
    /// Disable AR Occlusion
    /// </summary>
    public void ChangeQualityToNoOcclussion()
    {
        aROcclusionManager.requestedEnvironmentDepthMode = EnvironmentDepthMode.Disabled;
        ShowQualityPanel(false);
    }

    /// <summary>
    /// Show/Hide Quality control panel
    /// </summary>
    public void ShowQualityPanel(bool show)
    {
        if (supportDepth)
        {
            qualityPanel.SetActive(show);
        }
        else
        {
            //show warning panel for 2 seconds
            warningPanel.SetActive(true);
            StartCoroutine(ShowWarningPanel());
        }
    }

    IEnumerator ShowWarningPanel()
    {
        yield return new WaitForSeconds(2);
        warningPanel.SetActive(false);
    }
}

