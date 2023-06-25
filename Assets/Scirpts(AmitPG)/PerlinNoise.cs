using UnityEngine;
using UnityEngine.Assertions.Must;

public class PerlinNoise : MonoBehaviour
{
    [SerializeField] int xyLength;
    [SerializeField] float scale;

    [SerializeField] Gradient gradient;

    [SerializeField] Renderer perlinRenderer;

    [SerializeField] float meshOffSet;

    [SerializeField] MeshFilter mesh;

    float[,] noiseMap;

    Vector3[] set;

    private void Start()
    {
        noiseMap = CreateNoiseMap();
        perlinRenderer.sharedMaterial.mainTexture = DrawNoiseMap(xyLength);
        transform.localScale = new Vector3(xyLength, 1, xyLength);
        mesh.mesh = MeshGen.GenerateMeshData(noiseMap);
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

        Vector3[] verts = new Vector3[width*heigth];
        int[] triangles = new int[(width - 1) * (heigth - 1) * 6];

        int vertRowLength = width - 1;

        int vertexIndex = 0;
        int triangleIndex = 0;

        for (int y = 0; y < heigth; y++)
        {
            for (int x = 0; x < width; x++)
            {
                verts[vertexIndex] = new Vector3(1 * x, 0, 1 * - y);

                if (y < heigth - 1 && x < width - 1)
                {
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

        return mesh;
    }

}
