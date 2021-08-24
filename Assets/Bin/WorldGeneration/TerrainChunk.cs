using Bin.WorldGeneration.Data;
using UnityEngine;

namespace Bin.WorldGeneration
{
	public class TerrainChunk
	{
		private const float ColliderGenerationDistanceThreshold = 5;
		public event System.Action<TerrainChunk, bool> ONVisibilityChanged;
		public Vector2 Coord;

		private GameObject _meshObject;
		private readonly Vector2 _sampleCentre;
		private Bounds _bounds;

		private MeshRenderer _meshRenderer;
		private MeshFilter _meshFilter;
		private MeshCollider _meshCollider;
		private LODInfo[] _detailLevels;
		private LODMesh[] _lodMeshes;
		private int _colliderLODIndex;

		private HeightMap _heightMap;
		private bool _heightMapReceived;
		private int _previousLODIndex = -1;
		private bool _hasSetCollider;
		private float _maxViewDst;

		private HeightMapSettings _heightMapSettings;
		private MeshSettings _meshSettings;
		private readonly Transform _viewer;

		public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings,
			LODInfo[] detailLevels, int colliderLODIndex, Transform parent, Transform viewer, Material material)
		{
			Coord = coord;
			_detailLevels = detailLevels;
			_colliderLODIndex = colliderLODIndex;
			_heightMapSettings = heightMapSettings;
			_meshSettings = meshSettings;
			_viewer = viewer;

			_sampleCentre = coord * meshSettings.MeshWorldSize / meshSettings.meshScale;
			var position = coord * meshSettings.MeshWorldSize;
			_bounds = new Bounds(position, Vector2.one * meshSettings.MeshWorldSize);


			_meshObject = new GameObject("Terrain Chunk");
			_meshRenderer = _meshObject.AddComponent<MeshRenderer>();
			_meshFilter = _meshObject.AddComponent<MeshFilter>();
			_meshCollider = _meshObject.AddComponent<MeshCollider>();
			_meshRenderer.material = material;

			_meshObject.transform.position = new Vector3(position.x, 0, position.y);
			_meshObject.transform.parent = parent;
			SetVisible(false);

			_lodMeshes = new LODMesh[detailLevels.Length];
			for (var i = 0; i < detailLevels.Length; i++)
			{
				_lodMeshes[i] = new LODMesh(detailLevels[i].lod);
				_lodMeshes[i].UpdateCallback += UpdateTerrainChunk;
				if (i == colliderLODIndex)
				{
					_lodMeshes[i].UpdateCallback += UpdateCollisionMesh;
				}
			}

			_maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
		}

		public void Load()
		{
			ThreadedDataRequester.RequestData(
				() => HeightMapGenerator.GenerateHeightMap(_meshSettings.NumVertsPerLine, _meshSettings.NumVertsPerLine,
					_heightMapSettings, _sampleCentre), OnHeightMapReceived);
		}

		
		private void OnHeightMapReceived(object heightMapObject)
		{
			_heightMap = (HeightMap) heightMapObject;
			_heightMapReceived = true;

			UpdateTerrainChunk();
		}

		private Vector2 ViewerPosition => new Vector2(_viewer.position.x, _viewer.position.z);

		public void UpdateTerrainChunk() 
		{
			if (_heightMapReceived)
			{
				var viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));

				var wasVisible = IsVisible();
				var visible = viewerDstFromNearestEdge <= _maxViewDst;

				if (visible)
				{
					var lodIndex = 0;

					for (var i = 0; i < _detailLevels.Length - 1; i++) 
					{
						if (viewerDstFromNearestEdge > _detailLevels[i].visibleDstThreshold)
						{
							lodIndex = i + 1;
						} else {
							break;
						}
					}

					if (lodIndex != _previousLODIndex)
					{
						var lodMesh = _lodMeshes[lodIndex];
						if (lodMesh.HasMesh)
						{
							_previousLODIndex = lodIndex;
							_meshFilter.mesh = lodMesh.Mesh;
						} else if (!lodMesh.HasRequestedMesh)
						{
							lodMesh.RequestMesh(_heightMap, _meshSettings);
						}
					}
				}

				if (wasVisible != visible)
				{
					SetVisible(visible);
					ONVisibilityChanged?.Invoke(this, visible);
				}
			}
		}

		public void UpdateCollisionMesh() 
		{
			if (!_hasSetCollider)
			{
				var sqrDstFromViewerToEdge = _bounds.SqrDistance(ViewerPosition);

				if (sqrDstFromViewerToEdge < _detailLevels [_colliderLODIndex].SqrVisibleDstThreshold)
				{
					if (!_lodMeshes [_colliderLODIndex].HasRequestedMesh)
					{
						_lodMeshes[_colliderLODIndex].RequestMesh(_heightMap, _meshSettings);
					}
				}

				if (!(sqrDstFromViewerToEdge <
				      ColliderGenerationDistanceThreshold * ColliderGenerationDistanceThreshold)) return;
				if (!_lodMeshes[_colliderLODIndex].HasMesh) return;
				_meshCollider.sharedMesh = _lodMeshes[_colliderLODIndex].Mesh;
				_hasSetCollider = true;
			}
		}

		private void SetVisible(bool visible)
		{
			_meshObject.SetActive(visible);
		}

		private bool IsVisible()
		{
			return _meshObject.activeSelf;
		}

	}

	internal class LODMesh 
	{
		public Mesh Mesh;
		public bool HasRequestedMesh;
		public bool HasMesh;
		private readonly int _lod;
		public event System.Action UpdateCallback;

		public LODMesh(int lod) {
			_lod = lod;
		}

		private void OnMeshDataReceived(object meshDataObject)
		{
			Mesh = ((MeshData) meshDataObject).CreateMesh();
			HasMesh = true;

			UpdateCallback();
		}

		public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
		{
			HasRequestedMesh = true;
			ThreadedDataRequester.RequestData(
				() => MeshGenerator.GenerateTerrainMesh(heightMap.Values, meshSettings, _lod), OnMeshDataReceived);
		}

	}
}