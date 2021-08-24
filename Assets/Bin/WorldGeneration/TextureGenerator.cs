﻿using UnityEngine;

namespace Bin.WorldGeneration
{
	public static class TextureGenerator {
		private static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height) {
			var texture = new Texture2D(width, height)
			{
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp
			};
			texture.SetPixels (colourMap);
			texture.Apply();
			return texture;
		}


		public static Texture2D TextureFromHeightMap(HeightMap heightMap)
		{
			var width = heightMap.Values.GetLength(0);
			var height = heightMap.Values.GetLength(1);

			var colourMap = new Color[width * height];
			for (var y = 0; y < height; y++) 
			{
				for (var x = 0; x < width; x++)
				{
					colourMap[y * width + x] = Color.Lerp(Color.black, Color.white,
						Mathf.InverseLerp(heightMap.MINValue, heightMap.MAXValue, heightMap.Values[x, y]));
				}
			}

			return TextureFromColourMap(colourMap, width, height);
		}
	}
}