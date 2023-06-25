using System.Configuration;
using UnityEngine;

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
        mesh.mesh = TriangleGenerator.GenerateMesh(noiseMap, meshOffSet);
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

public static class TriangleGenerator
{
    static Vector3[] vertices;
    static int[] triangle;

    public static Mesh GenerateMesh(float[,] noiseMap, float xzOffset)
    {
        Mesh mesh = new Mesh();
        int xyLen = noiseMap.GetLength(0);

        vertices = CreateVerts(xyLen, noiseMap, xzOffset);
        triangle = CreateTriangle(vertices, xyLen);


        mesh.vertices = vertices;
        mesh.triangles = triangle;

        mesh.RecalculateNormals();
        return mesh;
    }

    static Vector3[] CreateVerts(int xyLength, float[,] noiseMap, float xzOffset)
    {
        Vector3[] vertices;
        vertices = new Vector3[xyLength * xyLength];

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        int trianglesNumber = (xyLength - 1) * (xyLength - 1) * 6;

        int vertCount = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // TODO : Add height according to the noise
                vertices[vertCount] = new Vector3(xzOffset * x, 0 , xzOffset * y);
                vertCount++;
            }
        }

        return vertices;
    }

    static int[] CreateTriangle(Vector3[] verts, int xyLength)
    {
        int[] triangles = new int[(xyLength - 1) * (xyLength - 1) * 6];

        int triIndex = 0;

        for(int i = 0; i < verts.Length; i++)
        {
            if (i != 0 && i % (xyLength - 1) == 0) break;
            if (triIndex >= triangles.Length) return triangles;

            triangles[triIndex] = i;
            triIndex++;
            triangles[triIndex] = i + xyLength + 1;
            triIndex++;
            triangles[triIndex] = i + xyLength;
            triIndex++;
            triangles[triIndex] = i;
            triIndex++;
            triangles[triIndex] = i + 1;
            triIndex++;
            triangles[triIndex] = i + xyLength + 1;
        }

        return triangles;
    }


}
