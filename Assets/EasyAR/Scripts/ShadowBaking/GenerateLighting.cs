using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class GenerateLighting : MonoBehaviour
{
    public Shader shader;
    public Texture2D texture;
    public GameObject selectedObjectForBaking;

    bool isBakingStarted = true;
    bool isBakingFinished = false;
    bool isObjectAttached = false;

    /// <summary>
    /// Check whether object attched to gameobject or not before baking start
    /// </summary>
    public void Bake()
    {
        if (isObjectAttached)
        {
            DelayUseAsync();
            isBakingStarted = true;
        }
    }

    /// <summary>
    /// Updating baking process on selected item
    /// </summary>
    public void UpdateBakeProgress()
    {
        if (selectedObjectForBaking.transform.childCount >= 1)
        {
            isObjectAttached = true;
        }
        else
        {
            isObjectAttached = false;
        }

        if (Lightmapping.isRunning)
        {
            isBakingFinished = false;
        }
        else if (!Lightmapping.isRunning)
        {
            isBakingFinished = true;
        }

        if (isBakingStarted && isBakingFinished && isObjectAttached)
        {
            Lightmapping.BakeAsync();
            GameObject _plane = GameObject.CreatePrimitive(PrimitiveType.Plane);

            _plane.GetComponent<Renderer>().material = new Material(Shader.Find(shader.name));
            _plane.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
            _plane.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            Material newMat = new Material(shader);
            string texName = "";
            texName = selectedObjectForBaking.transform.GetChild(0).name + System.DateTime.Now.ToString("HHmmss");

            AssetDatabase.CopyAsset("Assets/EasyAR/Utilities/ShadowBake/Lightmap-0_comp_light.exr", "Assets/EasyAR/Prefabs/ShadowPlanes/Textures/" + texName.ToString() + ".png");
            AssetDatabase.CreateAsset(newMat, "Assets/EasyAR/Prefabs/ShadowPlanes/Materials/" + "Shadow" + texName.ToString() + ".mat");
            string localPath = "Assets/EasyAR/Prefabs/ShadowPlanes/" + texName + "_Shadow.prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            Material j = new Material(Shader.Find(shader.name));
            _plane.GetComponent<Renderer>().material = newMat;
            newMat.mainTexture = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/EasyAR/Prefabs/ShadowPlanes/Textures/" + texName.ToString() + ".png", typeof(Texture2D)); ;
            PrefabUtility.SaveAsPrefabAssetAndConnect(_plane, localPath, InteractionMode.UserAction);
            GameObject.DestroyImmediate(_plane);
            gameObject.transform.GetChild(0).GetComponent<Renderer>().material = newMat;

            isBakingStarted = false;
            isBakingFinished = false;
        }
    }

    /// <summary>
    /// Start light baking
    /// </summary>
    void DelayUseAsync()
    {
        Lightmapping.BakeAsync();
    }
}
#endif