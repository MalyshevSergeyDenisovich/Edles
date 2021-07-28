using System.Collections.Generic;
using UnityEngine;

namespace Bin.WorldGeneration
{
    public class EndlessTerrain : MonoBehaviour
    {
        private const float Scale = 2f;
        
        private const float ViewerMoveThresholdForChunkUpdate = 25f;
        private const float SqrViewerMoveThresholdForChunkUpdate = ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate; 
        
        public LODInfo[] detailLevels;
        public static float maxViewDist;
        public Transform viewer;
        public Material mapMaterial;
        
        

        public static Vector2 viewPosition;
        private Vector2 veiwerPositionOld;
        private static MapGenerator mapGenerator;
        
        private int _chunkSize;
        private int _chunksVisibleInViewDistance;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunksDictionary = new Dictionary<Vector2, TerrainChunk>();
        private static readonly List<TerrainChunk> TerrainChunksVisibleLastUpdate = new List<TerrainChunk>();
        private void Start()
        {
            mapGenerator = FindObjectOfType<MapGenerator>();
            
            maxViewDist = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
            _chunkSize = MapGenerator.MapChunkSize - 1;
            _chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDist / _chunkSize);
            
            UpdateVisibleChunks();
        }

        private void Update()
        {
            viewPosition = new Vector2(viewer.position.x, viewer.position.z) / Scale;

            if ((veiwerPositionOld - viewPosition).sqrMagnitude > SqrViewerMoveThresholdForChunkUpdate)
            {
                veiwerPositionOld = viewPosition;
                UpdateVisibleChunks();
            }
        }


        private void UpdateVisibleChunks()
        {
            foreach (var terrainChunk in TerrainChunksVisibleLastUpdate)
            {
                terrainChunk.SetVisible(false);
            }
            TerrainChunksVisibleLastUpdate.Clear();
            
            var currentChunkCoordX = Mathf.RoundToInt(viewPosition.x / _chunkSize);
            var currentChunkCoordY = Mathf.RoundToInt(viewPosition.y / _chunkSize);

            for (var yOffset = -_chunksVisibleInViewDistance; yOffset <= _chunksVisibleInViewDistance; yOffset++)
            {
                for (var xOffset = -_chunksVisibleInViewDistance; xOffset <= _chunksVisibleInViewDistance; xOffset++)
                {
                    var viewChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (_terrainChunksDictionary.ContainsKey(viewChunkCoord)) {
                        _terrainChunksDictionary[viewChunkCoord].UpdateTerrainChunk();

                    } else {
                        _terrainChunksDictionary.Add(viewChunkCoord, new TerrainChunk(viewChunkCoord, _chunkSize, detailLevels, transform, mapMaterial));
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
            private readonly MeshCollider _meshCollider;

            private readonly LODInfo[] _detailLevels;
            private readonly LODMesh[] _lodMeshes;
            private readonly LODMesh collisionLODMesh;
            
            private MapData _mapData;
            private bool _mapDataRecieved;
            private int _previousLodIndex = - 1;
            public TerrainChunk(Vector2 coord, int size, LODInfo[] detailLevels, Transform parent, Material material)
            {
                _detailLevels = detailLevels;
                
                _position = coord * size;
                _bounds = new Bounds(_position,Vector2.one * size);
                var positionV3 = new Vector3(_position.x, 0, _position.y);
                
                
                _meshObject = new GameObject("TerrainChunk");
                _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
                _meshFilter = _meshObject.AddComponent<MeshFilter>();
                _meshCollider = _meshObject.AddComponent<MeshCollider>();
                _meshRenderer.material = material;
                
                _meshObject.transform.position = positionV3 * Scale;
                _meshObject.transform.parent = parent;
                _meshObject.transform.localScale = Vector3.one * Scale;
                SetVisible(false);

                _lodMeshes = new LODMesh[detailLevels.Length];
                for (var i = 0; i < detailLevels.Length; i++)
                {
                    _lodMeshes[i] = new LODMesh(detailLevels[i].lod, UpdateTerrainChunk);
                    if (detailLevels[i].useForCollied)
                    {
                        collisionLODMesh = _lodMeshes[i];
                    }
                }
                
                mapGenerator.RequestMapData(_position, OnMapDataReceived);
            }

            private void OnMapDataReceived(MapData mapData)
            {
                _mapData = mapData;
                _mapDataRecieved = true;

                var texture = TextureGenerator.TextureFromColorMap(mapData.colorMap, MapGenerator.MapChunkSize, MapGenerator.MapChunkSize);
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
                if (_mapDataRecieved)
                {
                    var viewDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(viewPosition));
                    var visible = viewDstFromNearestEdge <= maxViewDist;

                    if (visible)
                    {
                        var lodIndex = 0;
                        for (var i = 0; i < _detailLevels.Length - 1; i++)
                        {
                            if (viewDstFromNearestEdge > _detailLevels[i].visibleDstThreshold)
                            {
                                lodIndex = i + 1;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (lodIndex != _previousLodIndex)
                        {
                            var lodMesh = _lodMeshes[lodIndex];
                            if (lodMesh.hasMesh)
                            {
                                _previousLodIndex = lodIndex;
                                _meshFilter.mesh = lodMesh.mesh;
                            }
                            else if (!lodMesh.hasRequestedMesh)
                            {
                                lodMesh.RequestMesh(_mapData);
                            }
                        }

                        if (lodIndex == 0)
                        {
                            if (collisionLODMesh.hasMesh) {
                                _meshCollider.sharedMesh = collisionLODMesh.mesh;
                            } else if (!collisionLODMesh.hasRequestedMesh) {
                                collisionLODMesh.RequestMesh(_mapData);
                            }
                        }

                        TerrainChunksVisibleLastUpdate.Add(this);
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
            public bool useForCollied;
        }
    }
}