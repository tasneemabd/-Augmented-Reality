using UnityEditor;
using UnityEngine;

/// <summary>
/// Work only in Unity Editor
/// </summary>
#if UNITY_EDITOR

[CustomEditor(typeof(GenerateLighting))]
public class BakeHandler : Editor
{
    /// <summary>
    /// Customize the unity inspector for light baking button
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GenerateLighting generateLighting = (GenerateLighting)target;

        if(GUILayout.Button("Generate Shadow"))
        {
            generateLighting.Bake();
        }
        generateLighting.UpdateBakeProgress();
    }
}

#endif