using UnityEngine;
using UnityEngine.UI;

public class ConfiguratorController : MonoBehaviour
{
    public ColorButton[] textures;
    public Transform qualityControlButton;
    public Sprite ColorButtonSprite;
    public float buttonWidthHeight = 100f;
    public float buttonYOffset = 0;

    private GameObject spawnedObject;

    private void Start()
    {
        spawnedObject = PlaceOnPlane.spawnedObject;

        foreach (ColorButton t in textures)
        {
            GameObject button = new GameObject();
            button.transform.parent = gameObject.transform;
            button.AddComponent<RectTransform>();
            button.AddComponent<Image>();
            button.AddComponent<Button>();
            button.GetComponent<Image>().sprite = ColorButtonSprite;
            button.GetComponent<Image>().color = t.color;
            buttonYOffset -= 130f;
            button.transform.position = new Vector3(qualityControlButton.position.x, qualityControlButton.position.y + buttonYOffset, qualityControlButton.position.z);
            button.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonWidthHeight, buttonWidthHeight);
            button.GetComponent<Button>().onClick.AddListener(() => ChangeColor(t.texture));
        }
    }

    /// <summary>
    /// Change color texteture of material
    /// </summary>
    public void ChangeColor(Texture2D texture)
    {
        spawnedObject.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].SetTexture("_MainTex", texture);
    }
}

[System.Serializable]
public class ColorButton
{
    public Texture2D texture;
    public Color color;
}
