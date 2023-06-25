using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;

public static class Noise {

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapWidth"></param>
    /// <param name="mapHeight"></param>
    /// <param name="scale">the scale multiply of the noise, will be clamped above 0</param>
    /// <returns></returns>
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float presistance, float lacunarity,Vector2 offset) {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new(seed);
        Vector2[] octaveOffset = new Vector2[octaves];
        for (int i = 0; i < octaves; i++) {
            float offestX = prng.Next(-100000, 100000) + offset.x;
            float offestY = prng.Next(-100000, 100000) + offset.y;
            octaveOffset[i] = new Vector2(offestX, offestY);
        }

        #region Noise Clamp
        if (scale <= 0)
        {
            scale = 0.001f;
        }
        #endregion

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight  / 2f;

        for (int y = 0; y < mapHeight; y++) { 
            for (int x = 0; x < mapWidth; x++) {
                float amplitued = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffset[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffset[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1 ;
                    noiseHeight += perlinValue * amplitued;

                    amplitued *= presistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight; 

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }

        }
        return noiseMap;
    }
}
