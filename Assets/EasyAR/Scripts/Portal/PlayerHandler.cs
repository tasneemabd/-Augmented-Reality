using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    public GameObject otherHalfOfEnvironment;
    public GameObject mainEnvironment;
    public GameObject occulusionOut;
    public GameObject occulusionDoor;
    public Animator anim;  

    void Start()
    {
        otherHalfOfEnvironment.SetActive(false);
        mainEnvironment.SetActive(true);
        occulusionDoor.SetActive(true);
        occulusionOut.SetActive(true);
    }

    private void Update()
    {
        if(FindObjectOfType<PlaceOnPlane>() != null && anim != null)
        {
            if (PlaceOnPlane.isObjectPlaced)
            {
                anim.Play("PortalDoor");
            }
        }
    }

    /// <summary>
    /// When user enter into the portal other side of the portal enable
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if(other.name == "AR Camera")
        {
            occulusionDoor.SetActive(true);
            occulusionOut.SetActive(true);
            otherHalfOfEnvironment.SetActive(true);
            mainEnvironment.SetActive(true);
        }
    }

    /// <summary>
    /// When user exit from the portal the layer with occlusion door will be appear
    /// </summary>
    void OnTriggerExit (Collider other)
    {
        if (other.name == "AR Camera")
        {
            otherHalfOfEnvironment.SetActive(false);
            mainEnvironment.SetActive(true);
            occulusionDoor.SetActive(false);
        }
            
    }
}
