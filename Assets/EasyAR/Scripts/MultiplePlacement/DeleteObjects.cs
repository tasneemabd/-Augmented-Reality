using UnityEngine;

public class DeleteObjects : MonoBehaviour
{
    public GameObject deleteConfirmationPanel;
    public GameObject deleteButton;

    private GameObject selectedObject = null;

    private void Start()
    {
        deleteButton.SetActive(false);
    }

    private void Update()
    {
        if ((Input.touchCount > 0))
        {
            if ((Input.touches[0].phase == TouchPhase.Began) && !MultipleObjectPlacement.IsPointerOverUIObject())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                RaycastHit Hit;
                if (Physics.Raycast(ray, out Hit))
                {
                    if (Hit.transform.gameObject.GetComponent<SpawningObjectDetails>())
                    {
                        selectedObject = Hit.transform.gameObject;
                        ShowDeleteButton(true);
                    }
                    else
                    {
                        selectedObject = null;
                        ShowDeleteButton(false);
                    }
                }
                else
                {
                    selectedObject = null;
                    ShowDeleteButton(false);
                }
            }
        }
    }

    /// <summary>
    /// Show/Hide Delete Button
    /// </summary>
    private void ShowDeleteButton(bool show)
    {
        deleteButton.SetActive(show);
    }

    /// <summary>
    /// Show/Hide Delete Confirmation Panel
    /// </summary>
    public void ShowDeletePanel(bool show)
    {
        deleteConfirmationPanel.SetActive(show);
    }

    /// <summary>
    /// Delete selected game object
    /// </summary>
    public void DeleteGameObject()
    {
        if(selectedObject != null)
        {
            TouchIndicatorHandler.hitObject = null;
            Destroy(selectedObject);
        }
    }

}
