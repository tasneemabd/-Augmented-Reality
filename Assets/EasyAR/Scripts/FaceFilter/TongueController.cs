using UnityEngine;

public class TongueController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public GameObject tongue;
    public float maxBlenshapeWeightOnMouth = 10f;

    void Update()
    {
        if (skinnedMeshRenderer.GetBlendShapeWeight(0) > maxBlenshapeWeightOnMouth)
        {
            tongue.SetActive(true);
        }
        else
        {
            tongue.SetActive(false);
        }
    }
}

