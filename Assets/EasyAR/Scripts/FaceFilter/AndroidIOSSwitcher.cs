using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AndroidIOSSwitcher : MonoBehaviour
{
    private ARFaceMeshVisualizer faceMeshVisualizerIOS;
    private CustomARFaceMeshVisualizer faceMeshVisualizerAndroid;
    private Renderer m_Renderer;

    public Texture textureIOS;
    public Texture textureAndroid;

    private void Awake()
    {
        m_Renderer = GetComponent<Renderer>();
        faceMeshVisualizerIOS = GetComponent<ARFaceMeshVisualizer>();
        faceMeshVisualizerAndroid = GetComponent<CustomARFaceMeshVisualizer>();

#if UNITY_IPHONE
        faceMeshVisualizerAndroid.enabled = false;
        faceMeshVisualizerIOS.enabled = true;
        m_Renderer.material.SetTexture("_BaseMap", clownTextureIOS);
#endif
#if UNITY_ANDROID
        faceMeshVisualizerAndroid.enabled = true;
        faceMeshVisualizerIOS.enabled = false;
         m_Renderer.material.SetTexture("_BaseMap", textureAndroid);
#endif
    }
}
