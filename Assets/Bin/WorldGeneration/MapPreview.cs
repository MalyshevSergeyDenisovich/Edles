using Bin.WorldGeneration.Data;
using UnityEngine;

namespace Bin.WorldGeneration
{
	public class MapPreview : MonoBehaviour 
	{

		public Renderer textureRender;
		public MeshFilter meshFilter;
		public MeshRenderer meshRenderer;

		public enum DrawMode
		{
			NoiseMap,
			Mesh,
			FalloffMap
		}

		public DrawMode drawMode;

		public MeshSettings meshSettings;
		public HeightMapSettings heightMapSettings;
		public TextureData textureData;

		public Material terrainMaterial;
		
		[Range(0,MeshSettings.NumSupportedLODs-1)]
		public int editorPreviewLOD;
		public bool autoUpdate;

		
		public void DrawMapInEditor()
		{
			textureData.ApplyToMaterial(terrainMaterial);
			textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.MINHeight, heightMapSettings.MAXHeight);
			var heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.NumVertsPerLine,
				meshSettings.NumVertsPerLine, heightMapSettings, Vector2.zero);

			if (drawMode == DrawMode.NoiseMap)
			{
				DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
			} else if (drawMode == DrawMode.Mesh)
			{
				DrawMesh (MeshGenerator.GenerateTerrainMesh(heightMap.Values,meshSettings, editorPreviewLOD));
			} else if (drawMode == DrawMode.FalloffMap)
			{
				DrawTexture(TextureGenerator.TextureFromHeightMap(
					new HeightMap(FalloffGenerator.GenerateFalloffMap(meshSettings.NumVertsPerLine), 0, 1)));
			}
		}


		private void DrawTexture(Texture2D texture)
		{
			textureRender.sharedMaterial.mainTexture = texture;
			textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;

			textureRender.gameObject.SetActive(true);
			meshFilter.gameObject.SetActive(false);
		}

		private void DrawMesh(MeshData meshData)
		{
			meshFilter.sharedMesh = meshData.CreateMesh();

			textureRender.gameObject.SetActive(false);
			meshFilter.gameObject.SetActive(true);
		}


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

		private void OnValidate()
		{
			if (meshSettings != null)
			{
				meshSettings.OnValuesUpdated -= OnValuesUpdated;
				meshSettings.OnValuesUpdated += OnValuesUpdated;
			}
			if (heightMapSettings != null)
			{
				heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
				heightMapSettings.OnValuesUpdated += OnValuesUpdated;
			}
			if (textureData != null)
			{
				textureData.OnValuesUpdated -= OnTextureValuesUpdated;
				textureData.OnValuesUpdated += OnTextureValuesUpdated;
			}
		}
	}
}
