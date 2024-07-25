using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SwitchFace : MonoBehaviour
{
    public FacePrefab[] facePrefabs;
    private ARFaceManager aRFaceManager;
    public GameObject content;
    public GameObject buttonPrefab;

    int faceID = 0;

    private void Start()
    {
        aRFaceManager = GetComponent<ARFaceManager>();

        foreach (FacePrefab fp in facePrefabs)
        {
            // Create buttons for available face prefabs in facePrefabs list and set SelectFace method for buttons
            GameObject faceSelectButton = Instantiate(buttonPrefab);
            faceSelectButton.name = "faceButton " + faceID;
            faceSelectButton.transform.parent = content.transform;
            if (fp.maskIcon != null)
            {
                faceSelectButton.GetComponent<Image>().sprite = fp.maskIcon;
            }
            faceSelectButton.GetComponent<Button>().onClick.AddListener(delegate { SelectFace(); });
            faceID++;
        }
    }

    /// <summary>
    /// Individually identify each face and show them, destroy other AR faces from scene
    /// </summary>
    public void SelectFace()
    {
        aRFaceManager = FindObjectOfType<ARFaceManager>();
        if (aRFaceManager != null)
        {
            DestroyImmediate(aRFaceManager);
        }

        foreach (GameObject m in GameObject.FindGameObjectsWithTag("Face"))
        {
            DestroyImmediate(m);
        }

        aRFaceManager = gameObject.AddComponent<ARFaceManager>();
        int faceId = int.Parse(EventSystem.current.currentSelectedGameObject.name.Split(' ')[1]);

        aRFaceManager.facePrefab = facePrefabs[faceId].maskPrefab;

    }

    /// <summary>
    /// Navigate to main menu
    /// </summary>
    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        LoaderUtility.Deinitialize();
    }

}

[System.Serializable]
public class FacePrefab
{
    public GameObject maskPrefab;
    public Sprite maskIcon;
}
