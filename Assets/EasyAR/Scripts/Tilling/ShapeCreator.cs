using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
public class ShapeCreator : MonoBehaviour
{
    private float offsetX = 5;
    private float offsetY = 5;
    private float tillingValue = 2;
    private int[] triangles;
    private int[] indices;

    private Vector3[] verticies;
    private Vector2[] uv;
    private Vector2[] verticies2D;  
    
    private MeshRenderer meshRenderer;
    public Material[] materials;
    public Text areaText;
    public Slider tilling;

    public GameObject floorAreaPanel;
    public GameObject go;
    public GameObject selectorPanel;

    private Mesh mesh;
    private List<Vector2> UVs = new List<Vector2>();

    private const int NUMBEROFTILES = 3;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = this.mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        floorAreaPanel.SetActive(false);
    }

    /// <summary>
    /// Setup points for create shape
    /// </summary>
    public void SetupShape(Transform[] points)
    {
        verticies = new Vector3[points.Length];
        verticies2D = new Vector2[points.Length];
        uv = new Vector2[points.Length];
        Vector3[] verts = mesh.vertices;
        float minX = points[0].position.x, maxX = points[0].position.x, minY = points[0].position.z, maxY = points[0].position.z;
        Vector3 min = Vector3.one * float.PositiveInfinity;
        Vector3 max = Vector3.one * float.NegativeInfinity;

        foreach (Transform v in points)
        {
            if (v.position.x > max.x) max.x = v.position.x;
            if (v.position.y > max.y) max.y = v.position.y;
            if (v.position.z > max.z) max.z = v.position.z;
            if (v.position.x < min.x) min.x = v.position.x;
            if (v.position.y < min.y) min.y = v.position.y;
            if (v.position.z < min.z) min.z = v.position.z;
        }

        Vector3 size = max - min;

        for (int i = 0; i < points.Length; i++)
        {
            verticies[i] = points[i].position;
            verticies2D[i] = new Vector2(points[i].position.x, points[i].position.z);
        }

        for (int i = 0; i < points.Length; i++)
        {
            Vector3 v = verticies[i] - min;
            UVs.Add(new Vector2(v.x / size.x, v.z / size.z));
        }

        Trangulator trangulator = new Trangulator(verticies2D);
        indices = trangulator.Triangulate();
    }

    /// <summary>
    /// Change tile material
    /// </summary>
    public void ChangeTilling(float value)
    {
        tillingValue = value;
        meshRenderer.material.mainTextureScale = new Vector2(tillingValue, tillingValue);
        foreach(Material mat in materials)
        {
            mat.mainTextureScale = new Vector2(tillingValue, tillingValue);
        }
    }

    /// <summary>
    /// Change tile offset x
    /// </summary>
    public void ChangeOffsetX(float value)
    {
        offsetX = value;
        meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }

    /// <summary>
    /// Change tile offset y
    /// </summary>
    public void ChangeOffsetY(float value)
    {
        offsetY = value;
        meshRenderer.material.SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }

    /// <summary>
    /// Rotate tile UV clockwise
    /// </summary>
    public void UVRotationRight()
    {
        Vector2[] RotatedUVs = mesh.uv;//Store the existing UV's
        for (var i = 0; i < RotatedUVs.Length; i++)
        {
            RotatedUVs[i] += new Vector2(0, 0);
            var rot = Quaternion.Euler(0, 0, 45 * Time.deltaTime);
            RotatedUVs[i] = rot * RotatedUVs[i];
        }

        //re-apply the adjusted uvs
        mesh.uv = RotatedUVs;
    }

    /// <summary>
    /// Rotate tile UV anti-clockwise
    /// </summary>
    public void UVRotationLeft()
    {
        //Store the existing UV's
        Vector2[] RotatedUVs = mesh.uv;

        for (var i = 0; i < RotatedUVs.Length; i++)
        {
            RotatedUVs[i] += new Vector2(0, 0);
            var rot = Quaternion.Euler(0, 0, -45 * Time.deltaTime);
            RotatedUVs[i] = rot * RotatedUVs[i];
        }

        //re-apply the adjusted uvs
        mesh.uv = RotatedUVs;
    }

    /// <summary>
    /// Setting up mesh verticies, uv and other properties
    /// </summary>
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = verticies;
        mesh.uv = UVs.ToArray();
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        SelectionUIAppear();
        float areaCal =  CalculateSurfaceArea(mesh);
        areaText.text = areaCal.ToString("F1")+" m";
    }

    /// <summary>
    /// Show tile material change panel
    /// </summary>
    void SelectionUIAppear()
    {
        floorAreaPanel.SetActive(true);
        go.SetActive(false);
        selectorPanel.SetActive(true);
    }
    
    /// <summary>
    /// Change tile material
    /// </summary>
    public void ChangedTile(int tile)
    {
        meshRenderer.material = materials[tile];
    }

    /// <summary>
    /// Assign triangles to array
    /// </summary>
    public void Trangulator(int[] array)
    {
        triangles = new int[array.Length];
        triangles = array;
        UpdateMesh();
    }

    /// <summary>
    /// Calculate surface area 
    /// </summary>
    float CalculateSurfaceArea(Mesh mesh)
    {
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;
        double sum = 0.0;
        for (int i = 0; i < triangles.Length; i += NUMBEROFTILES)
        {
            Vector3 corner = vertices[triangles[i]];
            Vector3 a = vertices[triangles[i + 1]] - corner;
            Vector3 b = vertices[triangles[i + 2]] - corner;
            sum += Vector3.Cross(a, b).magnitude;
        }
        return (float)(sum / 2.0);
    }

}


