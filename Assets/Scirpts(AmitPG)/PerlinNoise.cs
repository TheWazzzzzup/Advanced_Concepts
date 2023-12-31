using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    [Header("Scale")]
    [SerializeField] int xyLength;
    [SerializeField] float scale;
    [SerializeField] float meshOffSet;

    [Header("Color")]
    [SerializeField] Gradient gradient;

    [Header("Mesh")]
    [SerializeField] Renderer perlinRenderer;
    [SerializeField] MeshFilter mesh;

    float[,] noiseMap;

    private void Start()
    {
        noiseMap = CreateNoiseMap();
        perlinRenderer.sharedMaterial.mainTexture = DrawNoiseMap(xyLength);
        mesh.mesh = MeshGen.GenerateMeshData(noiseMap);
        transform.localScale = new Vector3(1, 8, 1);
        mesh.mesh.RecalculateNormals();
    }

    private void OnValidate()
    {
        noiseMap = CreateNoiseMap();
        perlinRenderer.sharedMaterial.mainTexture = DrawNoiseMap(xyLength);
    }

    float[,] CreateNoiseMap()
    {
        noiseMap = new float[xyLength, xyLength];
        float Xf, Yf;

        for (int y = 0; y < xyLength; y++)
        {
            for (int x = 0; x < xyLength; x++)
            {
                Xf = x / scale;
                Yf = y / scale;

                noiseMap[x, y] = Mathf.PerlinNoise(Xf, Yf);
            }
        }
        return noiseMap;
    }

    Texture2D DrawNoiseMap(int length)
    {
        Texture2D texture = new Texture2D(length, length);

        Color[] colorsMap = new Color[length * length];

        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < length; x++)
            {
                colorsMap[y * length + x] = gradient.Evaluate(noiseMap[x, y]);
            }
        }

        texture.SetPixels(colorsMap);
        texture.Apply();

        return texture;
    }


}

public static class MeshGen
{
    public static Mesh GenerateMeshData(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int heigth = noiseMap.GetLength(1);

        Vector3[] verts = new Vector3[width * heigth];
        int[] triangles = new int[(width - 1) * (heigth - 1) * 6];
        Vector2[] uv = new Vector2[width * heigth];


        int vertRowLength = width - 1;

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int y = 0; y < heigth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                verts[vertexIndex] = new Vector3(1 * x, Mathf.Lerp(0, 1, noiseMap[x, y]), 1 * -y);
                uv[vertexIndex] = new Vector2(x / (float)width, y / (float)heigth);

                if (y < heigth - 1 && x < width - 1)
                {
                    // creates two triangles base on the current vert location and the surrounding three
                    triangles[triangleIndex] = vertexIndex;
                    triangles[triangleIndex + 1] = vertexIndex + vertRowLength + 1;
                    triangles[triangleIndex + 2] = vertexIndex + vertRowLength;
                    triangles[triangleIndex + 3] = vertexIndex + vertRowLength + 1;
                    triangles[triangleIndex + 4] = vertexIndex;
                    triangles[triangleIndex + 5] = vertexIndex + 1;
                    triangleIndex += 6;
                }

                vertexIndex++;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();

        return mesh;
    }

}
