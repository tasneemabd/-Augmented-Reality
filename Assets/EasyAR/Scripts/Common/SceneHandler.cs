using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    /// <summary>
    /// Navigate to next scene 
    /// </summary>
    public static void GoToNextView()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
    }

    /// <summary>
    /// Back to menu scene
    /// </summary>
    public void BackFromCurrentScene()
    {
        if (Application.CanStreamedLevelBeLoaded("Menu"))
        {
            PlaceOnPlane.isObjectPlaced = false;
            MultipleObjectPlacement.isObjectPlaced = false;
            Destroy(PlaceOnPlane.spawnedObject);
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            LoaderUtility.Deinitialize();
        }
    }

    /// <summary>
    /// Navigate to AR measurement scene 
    /// </summary>
    public static void ARMeasurement()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ARMeasurement", LoadSceneMode.Single);
    }

    /// <summary>
    /// Navigate to ARTilling scene 
    /// </summary>
    public static void ARTile()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ARTilling", LoadSceneMode.Single);
    }

    /// <summary>
    /// Navigate to AR Multiple scene 
    /// </summary>
    public static void ARMultipleObjects()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ARMultipleObjects", LoadSceneMode.Single);
    }

    /// <summary>
    /// Navigate to AR Configurator scene 
    /// </summary>
    public static void ARConfigurator()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ARConfigurator", LoadSceneMode.Single);
    }

    /// <summary>
    /// Navigate to AR Face Filter scene 
    /// </summary>
    public static void ARFaceFilter()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene("FaceFilter", LoadSceneMode.Single);
    }


    /// <summary>
    /// Navigate to AR Image Target Scene
    /// </summary>
    public static void ARImageTracking()
    {
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ImageTarget", LoadSceneMode.Single);
    }
}
