using System.Collections;
using System.Collections.Generic;
using Bin.WorldGeneration.Data;
using UnityEngine;

namespace Bin.WorldGeneration
{
	public class TerrainGenerator : MonoBehaviour {
		private const float ViewerMoveThresholdForChunkUpdate = 25f;
		private const float SqrViewerMoveThresholdForChunkUpdate = ViewerMoveThresholdForChunkUpdate * ViewerMoveThresholdForChunkUpdate;


		public int colliderLODIndex;
		public LODInfo[] detailLevels;

		public MeshSettings meshSettings;
		public HeightMapSettings heightMapSettings;
		public TextureData textureSettings;

		public Transform viewer;
		public Material mapMaterial;

		private Vector2 _viewerPosition;
		private Vector2 _viewerPositionOld;

		private float _meshWorldSize;
		private int _chunksVisibleInViewDst;

		private readonly Dictionary<Vector2, TerrainChunk> _terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
		private readonly List<TerrainChunk> _visibleTerrainChunks = new List<TerrainChunk>();

		private void Start() 
		{
			textureSettings.ApplyToMaterial(mapMaterial);
			textureSettings.UpdateMeshHeights (mapMaterial, heightMapSettings.MINHeight, heightMapSettings.MAXHeight);

			var maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
			_meshWorldSize = meshSettings.MeshWorldSize;
			_chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / _meshWorldSize);

			UpdateVisibleChunks();
		}

		private void Update() 
		{
			_viewerPosition = new Vector2 (viewer.position.x, viewer.position.z);

			if (_viewerPosition != _viewerPositionOld) 
			{
				foreach (var chunk in _visibleTerrainChunks) 
				{
					chunk.UpdateCollisionMesh ();
				}
			}

			if ((_viewerPositionOld - _viewerPosition).sqrMagnitude > SqrViewerMoveThresholdForChunkUpdate) 
			{
				_viewerPositionOld = _viewerPosition;
				UpdateVisibleChunks();
			}
		}

		private void UpdateVisibleChunks() {
			var alreadyUpdatedChunkCoords = new HashSet<Vector2> ();
			for (var i = _visibleTerrainChunks.Count-1; i >= 0; i--) {
				alreadyUpdatedChunkCoords.Add(_visibleTerrainChunks [i].Coord);
				_visibleTerrainChunks [i].UpdateTerrainChunk ();
			}
			
			var currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _meshWorldSize);
			var currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _meshWorldSize);

			for (var yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++) 
			{
				for (var xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++) 
				{
					var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);
					if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) {
						if (_terrainChunkDictionary.ContainsKey(viewedChunkCoord)) {
							_terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk ();
						} else {
							var newChunk = new TerrainChunk(viewedChunkCoord,heightMapSettings,meshSettings, detailLevels, colliderLODIndex, transform, viewer, mapMaterial);
							_terrainChunkDictionary.Add(viewedChunkCoord, newChunk);
							newChunk.ONVisibilityChanged += OnTerrainChunkVisibilityChanged;
							newChunk.Load ();
						}
					}

				}
			}
		}

		private void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
		{
			if (isVisible) {
				_visibleTerrainChunks.Add(chunk);
			} else {
				_visibleTerrainChunks.Remove(chunk);
			}
		}

	}

	[System.Serializable]
	public struct LODInfo 
	{
		[Range(0, MeshSettings.NumSupportedLODs-1)]
		public int lod;
		public float visibleDstThreshold;
		
		public float SqrVisibleDstThreshold => visibleDstThreshold * visibleDstThreshold;
	}
}