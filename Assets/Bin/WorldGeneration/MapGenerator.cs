using System;
using System.Collections.Generic;
using System.Threading;
using Bin.WorldGeneration.Data;
using UnityEngine;
using TerrainData = Bin.WorldGeneration.Data.TerrainData;


namespace Bin.WorldGeneration
{
    public class MapGenerator : MonoBehaviour
    {
        public enum DrawMode { NoiseMod, Mesh, FalloffMap}
        public DrawMode drawMode;

        public TerrainData terrainData;
        public NoiseData noiseData;
        public TextureData textureData;

        public Material terrainMaterial;
        
        [Range(0,6)]
        public int editorPreviewLOD;
   
        public bool autoUpdate;

        private float[,] falloffMap;
        
        private readonly Queue<MapThreadInfo<MapData>> _mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
        private readonly Queue<MapThreadInfo<MeshData>> _meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();

        private void Awake()
        {
            
        }

        public int MapChunkSize => terrainData.useFlatShading ? 95 : 239;

        private void OnValuesUpdated()
        {
            if (!Application.isPlaying)
            {
                DrawMapInEditor();
            }
        }

        private void OnTextureValuesUpdated()
        {
            textureData.ApplyToMaterial(terrainMaterial);
            
        }

        public void DrawMapInEditor()
        {
            var mapData = GenerateMapData(Vector2.zero);
            var display = FindObjectOfType<MapDisplay>();
            if (drawMode == DrawMode.NoiseMod) {
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));    
            } else if (drawMode == DrawMode.Mesh) {
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, editorPreviewLOD, terrainData.useFlatShading));
            } else if(drawMode == DrawMode.FalloffMap) {
                 display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFallofMap(MapChunkSize)));
            }
        }

        public void RequestMapData(Vector2 centre, Action<MapData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MapDataThread(centre, callback);
            };
            new Thread(threadStart).Start();
        }

        private void MapDataThread(Vector2 centre, Action<MapData> callback)
        {
            var mapData = GenerateMapData(centre);
            lock (_mapDataThreadInfoQueue)
            {
                _mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
            }
        }

        public void RequestMeshData(MapData mapData, int lod, Action<MeshData> callback)
        {
            ThreadStart threadStart = delegate
            {
                MeshDataThread(mapData, lod, callback);
            };
            new Thread(threadStart).Start();
        }
        
        private void MeshDataThread(MapData mapData, int lod, Action<MeshData> callback)
        {
            var meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, terrainData.meshHeightMultiplier, terrainData.meshHeightCurve, lod, terrainData.useFlatShading);
            lock (_meshDataThreadInfoQueue)
            {
                _meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
            }
        }

        private void Update()
        {
            if (_mapDataThreadInfoQueue.Count > 0)
            {
                for (var i = 0; i < _mapDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MapData> threadInfo = _mapDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
            
            if (_meshDataThreadInfoQueue.Count > 0)
            {
                for (var i = 0; i < _meshDataThreadInfoQueue.Count; i++)
                {
                    MapThreadInfo<MeshData> threadInfo = _meshDataThreadInfoQueue.Dequeue();
                    threadInfo.callback(threadInfo.parameter);
                }
            }
        }

        private MapData GenerateMapData(Vector2 centre)
        {
            var noiseMap = Noise.GenerateNoseMap(MapChunkSize + 2, MapChunkSize + 2,
                noiseData.seed,
                noiseData.noiseScale,
                noiseData.octaves,
                noiseData.persistence,
                noiseData.lacunarity, centre + noiseData.offset, noiseData.normalizeMode);

            if (terrainData.useFalloff) {
                if (falloffMap == null)
                {
                    falloffMap = FalloffGenerator.GenerateFallofMap(MapChunkSize + 2);
                }
                for (var y = 0; y < MapChunkSize + 2; y++)
                {
                    for (var x = 0; x < MapChunkSize + 2; x++)
                    {
                        if (terrainData.useFalloff)
                        {
                            noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x, y]);
                        }
                    }
                }
            }
            

            return new MapData(noiseMap);
        }

        private void OnValidate()
        {
            if (terrainData != null) {
                terrainData.OnValuesUpdated -= OnValuesUpdated;
                terrainData.OnValuesUpdated += OnValuesUpdated;
            } 
            if (noiseData != null) {
                noiseData.OnValuesUpdated -= OnValuesUpdated;
                noiseData.OnValuesUpdated += OnValuesUpdated;
            }
            if (textureData != null) {
                textureData.OnValuesUpdated -= OnTextureValuesUpdated;
                textureData.OnValuesUpdated += OnTextureValuesUpdated;
            }
        }
        
        private struct MapThreadInfo<T>
        {
            public readonly Action<T> callback;
            public readonly T parameter;

            public MapThreadInfo(Action<T> callback, T parameter)
            {
                this.callback = callback;
                this.parameter = parameter;
            }
        }
    }
    
    public struct MapData
    {
        public readonly float[,] heightMap;

        public MapData(float[,] heightMap)
        {
            this.heightMap = heightMap;
        }
    }
}