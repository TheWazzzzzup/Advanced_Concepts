using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using UnityEngine;
using PlayFab.ClientModels;

public class MapGenerator : MonoBehaviour {

    public bool autoUpdate;
    public enum DrawMode { Noise, ColorMap , DrawMesh}
    public DrawMode drawMode;

    public const int mapChunckSize = 241;

    [Range(0, 6)]
    [SerializeField] int levelOfDetail;
    [SerializeField] float noiseScale;
    [SerializeField] int octaves;
    [Range(0, 1)]
    [SerializeField] float presistance;
    [SerializeField] float lacunarity;
    [SerializeField] int seed;
    [SerializeField] Vector2 offset;

    [SerializeField] float meshHeightMultiplier;
    [SerializeField] AnimationCurve meshHieghtCurve;

    [SerializeField] TerrainType[] regions;

    [SerializeField] MapDisplay mapDisplay;

    Queue <MapTheardInformation <MapData>> mapDataTheardInfoQueue = new();
    Queue <MapTheardInformation <MeshData>> meshDataTheardInfoQueue = new();

    private void Update()
    {
        if (mapDataTheardInfoQueue.Count > 0) { 
            for (int i = 0; i < mapDataTheardInfoQueue.Count; i++) { 
                MapTheardInformation<MapData> theardInfo = mapDataTheardInfoQueue.Dequeue();
                theardInfo.callback(theardInfo.paramater);
            }
        }

        if (meshDataTheardInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataTheardInfoQueue.Count; i++)
            {
                MapTheardInformation<MeshData> theardInfo = meshDataTheardInfoQueue.Dequeue();
                theardInfo.callback(theardInfo.paramater);
            }
        }
    }

    public void DrawMapEditor() {
        MapData mapData = GenerateMapData(); 
        if (drawMode == DrawMode.Noise) mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        else if (drawMode == DrawMode.ColorMap)
        {
            mapDisplay.DrawTexture(TextureGenerator.TextureForColorMap(mapData.colorMap, mapChunckSize, mapChunckSize));
        }
        else if (drawMode == DrawMode.DrawMesh)
        {
            mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHieghtCurve, levelOfDetail), TextureGenerator.TextureForColorMap(mapData.colorMap, mapChunckSize, mapChunckSize));
        }
    }

    public void RequestMapData(Action<MapData> mapDataCallBack) {
        ThreadStart threadStart = delegate {
            MapDataTherad(mapDataCallBack);
        };

        new Thread(threadStart).Start();
    }
        
    public void RequestMeshData(MapData mapData, Action<MeshData> meshDataCallback) {
        ThreadStart threadStart = delegate {
            MeshDataTheard(mapData, meshDataCallback);
        };
        new Thread(threadStart).Start();
    }

    void MeshDataTheard(MapData mapData, Action<MeshData> meshDataCallback) {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHieghtCurve, levelOfDetail);
        lock (meshDataTheardInfoQueue) {
            meshDataTheardInfoQueue.Enqueue(new MapTheardInformation<MeshData>(meshDataCallback, meshData));
        }
    
    }

    void MapDataTherad(Action<MapData> mapDataCallBack) {
        MapData mapData = GenerateMapData();
        lock (mapDataTheardInfoQueue) {
            mapDataTheardInfoQueue.Enqueue(new MapTheardInformation<MapData>(mapDataCallBack, mapData));
        }
    }
    
    MapData GenerateMapData() {
        float [,] noiseMap =Noise.GenerateNoiseMap(mapChunckSize, mapChunckSize, seed, noiseScale,octaves,presistance,lacunarity, offset);

        Color[] colorMap = new Color[mapChunckSize * mapChunckSize];
        for (int y = 0; y < mapChunckSize; y++) {
            for (int x = 0; x < mapChunckSize; x++) { 
                float currenHeight = noiseMap[x,y];
                for (int i= 0; i < regions.Length; i++) { 
                    if (currenHeight <= regions[i].height)
                    {
                        colorMap [y* mapChunckSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colorMap);
    }

    private void OnValidate() {
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;

    }

    struct MapTheardInformation<T> {
        public readonly Action<T> callback;
        public readonly T paramater;

        public MapTheardInformation(Action<T> callback, T paramater)
        {
            this.callback = callback;
            this.paramater = paramater;
        }
    }
}

[System.Serializable]
public struct TerrainType {
    public string name;
    public float height;
    public Color color;
     
}

public struct MapData {
    public readonly float[,] heightMap;
    public readonly Color[] colorMap;

    public MapData(float[,] heightMap, Color[] colorMap)
    {
        this.heightMap = heightMap;
        this.colorMap = colorMap;
    }
}
