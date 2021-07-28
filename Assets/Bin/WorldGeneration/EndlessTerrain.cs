using System.Collections.Generic;
using UnityEngine;

namespace Bin.WorldGeneration
{
    public class EndlessTerrain : MonoBehaviour
    {
        private const float ViewerMoveThresholdForChunkUpdate = 25f;
        private const float sqrViewerMoveThresholdForChunkUpdate = ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate; 
        
        public LODInfo[] detailLevels;
        public static float maxViewDist;
        public Transform viewer;
        public Material mapMaterial;
        
        

        public static Vector2 viewPosition;
        private Vector2 veiwerPositionOld;
        private static MapGenerator mapGenerator;
        
        private int chunkSize;
        private int chunksVisibleInViewDistance;

        private Dictionary<Vector2, TerrainChunk> terrainChunksDictionary = new Dictionary<Vector2, TerrainChunk>();
        private List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
        private void Start()
        {
            mapGenerator = FindObjectOfType<MapGenerator>();
            
            maxViewDist = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
            chunkSize = MapGenerator.MapChunkSize - 1;
            chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDist / chunkSize);
            
            UpdateVisibleChunks();
        }

        private void Update()
        {
            viewPosition = new Vector2(viewer.position.x, viewer.position.z);

            if ((veiwerPositionOld - viewPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
            {
                veiwerPositionOld = viewPosition;
                UpdateVisibleChunks();
            }
        }


        private void UpdateVisibleChunks()
        {
            foreach (var terrainChunk in terrainChunksVisibleLastUpdate)
            {
                terrainChunk.SetVisible(false);
            }
            terrainChunksVisibleLastUpdate.Clear();
            
            var currentChunkCoordX = Mathf.RoundToInt(viewPosition.x / chunkSize);
            var currentChunkCoordY = Mathf.RoundToInt(viewPosition.y / chunkSize);

            for (var yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++)
            {
                for (var xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++)
                {
                    var viewChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (terrainChunksDictionary.ContainsKey(viewChunkCoord)) {
                        terrainChunksDictionary[viewChunkCoord].UpdateTerrainChunk();
                        if (terrainChunksDictionary[viewChunkCoord].IsVisible())
                        {
                            terrainChunksVisibleLastUpdate.Add(terrainChunksDictionary[viewChunkCoord]);
                        }
                    } else {
                        terrainChunksDictionary.Add(viewChunkCoord, new TerrainChunk(viewChunkCoord, chunkSize, detailLevels, transform, mapMaterial));
                    }
                }
            }
        }
        
        public class TerrainChunk
        {
            private readonly GameObject _meshObject;
            private readonly Vector2 _position;
            private Bounds _bounds;



            private readonly MeshRenderer _meshRenderer;
            private readonly MeshFilter _meshFilter;

            private LODInfo[] detailLevels;
            private LODMesh[] lodMeshes;
            
            private MapData mapData;
            private bool mapDataRecieved;
            private int previousLodIndex = -1;
            public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
            {
                this.detailLevels = detailLevels;
                
                _position = coord * size;
                _bounds = new Bounds(_position,Vector2.one * size);
                var positionV3 = new Vector3(_position.x, 0, _position.y);
                
                
                _meshObject = new GameObject("TerrainChunk");
                _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
                _meshFilter = _meshObject.AddComponent<MeshFilter>();
                _meshRenderer.material = material;
                
                _meshObject.transform.position = positionV3;
                _meshObject.transform.parent = parent;
                SetVisible(false);

                lodMeshes = new LODMesh[detailLevels.Length];
                for (int i = 0; i < detailLevels.Length; i++)
                {
                    lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
                }
                
                mapGenerator.RequestMapData(_position, OnMapDataReceived);
            }

            private void OnMapDataReceived(MapData mapData)
            {
                this.mapData = mapData;
                mapDataRecieved = true;

                Texture2D texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.MapChunkSize, MapGenerator.MapChunkSize);
                _meshRenderer.material.mainTexture = texture;
                UpdateTerrainChunk();
                //mapGenerator.RequestMeshData(mapData, OnMeshDataReceived);
            }

            private void OnMeshDataReceived(MeshData meshData)
            {
                _meshFilter.mesh = meshData.CreateMesh();
            }


            public void UpdateTerrainChunk()
            {
                if (mapDataRecieved)
                {
                    var viewDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewPosition));
                    var visible = viewDstFromNearestEdge <= maxViewDist;

                    if (visible)
                    {
                        int lodIndex = 0;
                        for (int i = 0; i < detailLevels.Length - 1; i++)
                        {
                            if (viewDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                            {
                                lodIndex = i + 1;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (lodIndex != previousLodIndex)
                        {
                            LODMesh lodMesh = lodMeshes[lodIndex];
                            if (lodMesh.hasMesh)
                            {
                                previousLodIndex = lodIndex;
                                _meshFilter.mesh = lodMesh.mesh;
                            }
                            else if (!lodMesh.hasRequestedMesh)
                            {
                                lodMesh.RequestMesh(mapData);
                            }
                        }
                    }

                    SetVisible(visible);
                }
            }

            public void SetVisible(bool visible)
            {
                _meshObject.SetActive(visible);
            }

            public bool IsVisible()
            {
                return _meshObject.activeSelf;
            }

        }

        private class LODMesh
        {
            public Mesh mesh;
            public bool hasRequestedMesh;
            public bool hasMesh;
            private int lod;
            private System.Action updateCallback;
            public LODMesh(int lod, System.Action updateCallback)
            {
                this.lod = lod;
                this.updateCallback = updateCallback;
            }

            private void OnMeshDataReceived(MeshData meshData)
            {
                mesh = meshData.CreateMesh();
                hasMesh = true;
                updateCallback();
            }

            public void RequestMesh(MapData mapData)
            {
                hasRequestedMesh = true;
                mapGenerator.RequestMeshData(mapData,lod, OnMeshDataReceived);
            }
        }
        [System.Serializable]
        public struct  LODInfo
        {
            public int lod;
            public float visibleDstThreshold;
        }
    }
}