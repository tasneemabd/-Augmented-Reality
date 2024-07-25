using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    bool isHiddenSettings;
    public GameObject settingsPanel;
    public Button settingsButton;

    void Start()
    {
        isHiddenSettings = false;
    }

    void Update()
    {
        if (isHiddenSettings)
        {
            settingsPanel.SetActive(true);
            settingsButton.interactable = false;
        }
        else
        {
            settingsPanel.SetActive(false);
            settingsButton.interactable = true;
        }
    }

    //Disable/Enable settings panel
    public void HideSettings()
    {
        isHiddenSettings = !isHiddenSettings;
    }
}
