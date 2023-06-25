using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDistance = 450;
    [SerializeField] Transform viewer;
    [SerializeField] Material mapMaterial;
    
    public static Vector2 viewerPosition;
    static MapGenerator mapGenerator; 

    int chunckSize;
    int chunksVisiableInDist;

    Dictionary<Vector2, TerrainChunck> terrianChunckDic = new();
    List<TerrainChunck> terrainChunckVisable = new ();

    private void Start() {
        mapGenerator = FindObjectOfType<MapGenerator>();   
        chunckSize = MapGenerator.mapChunckSize - 1;
        chunksVisiableInDist = Mathf.RoundToInt(maxViewDistance / chunckSize);
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisableChuncks();
    }

    void UpdateVisableChuncks() {

        for (int i = 0; i < terrainChunckVisable.Count; i++) {
            terrainChunckVisable[i].SetVisable(false);
        }
        terrainChunckVisable.Clear();    


        int currentChunckCoordX = Mathf.RoundToInt(viewerPosition.x / chunckSize);
        int currentChunckCoordY = Mathf.RoundToInt(viewerPosition.y / chunckSize);

        for (int yOffset = -chunksVisiableInDist; yOffset <= chunksVisiableInDist; yOffset++) { 
            for (int  xOffset = -chunksVisiableInDist; xOffset <= chunksVisiableInDist; xOffset++) {

                Vector2 viewedChunckCoord = new Vector2(currentChunckCoordX + xOffset, currentChunckCoordY + yOffset);
                
                if (terrianChunckDic.ContainsKey(viewedChunckCoord)) {
                    terrianChunckDic[viewedChunckCoord].UpdateTerrainChunck();
                    if (terrianChunckDic[viewedChunckCoord].IsVisible()) {
                        terrainChunckVisable.Add(terrianChunckDic[viewedChunckCoord]);
                    }
                }
                else { 
                    terrianChunckDic.Add(viewedChunckCoord, new TerrainChunck(viewedChunckCoord,chunckSize, transform,mapMaterial ));
                }
            }
        }
    }

    public class TerrainChunck {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;

        MeshRenderer meshRender;
        MeshFilter meshFilter;

        public TerrainChunck(Vector2 coord, int size, Transform parent, Material material) {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x,0, position.y);

            meshObject = new GameObject("TerrainChunck");
            meshRender = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRender.material = material;

            meshObject.transform.position = positionV3;
            meshObject.transform.parent = parent;
            SetVisable(false);

            mapGenerator.RequestMapData(OnMapDataRecived);
        }

        public void UpdateTerrainChunck() {
            float viewerDistFromNearsetEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistFromNearsetEdge <= maxViewDistance;
            SetVisable(visible);
        }

        public void SetVisable(bool visible) { 
            meshObject.SetActive(visible);
        }

        public bool IsVisible() {
            return meshObject.activeSelf;
        }

        void OnMeshDataRecvied(MeshData meshData) {
            meshFilter.mesh = meshData.CreateMesh();
        }

        void OnMapDataRecived(MapData mapdata) {
            mapGenerator.RequestMeshData(mapdata, OnMeshDataRecvied);
        
        }
    }

    class LevelOfDetailMesh { 
        
    
    
    }

}
