using UnityEngine;

/// <summary>
/// Getting Initial Data for handlers
/// </summary>
public class InitialData : MonoBehaviour
{
    public static GameObject spawningObject;
    public static bool _singleObjectPlacement;

    /// <summary>
    /// Forward custom prefab into AR scene
    /// </summary>
    /// <param name="spwaningObject"></param>
    public void ShowPrefabInARView(GameObject spwaningObject)
    {
        spawningObject = spwaningObject;
        SceneHandler.GoToNextView();
    }

    /// <summary>
    /// Forward prefab into AR configurator scene
    /// </summary>
    /// <param name="spwaningObject"></param>
    public void ARConfigurator(GameObject spwaningObject)
    {
        spawningObject = spwaningObject;
        SceneHandler.ARConfigurator();
    }
}
