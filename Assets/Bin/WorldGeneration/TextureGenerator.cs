using UnityEngine;

namespace Bin.WorldGeneration
{
    public static class TextureGenerator
    {
        public static Texture2D TextureFromColorMap(Color[] colourMap, int width, int height)
        {
            var texture = new Texture2D(width, height);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.SetPixels(colourMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D TextureFromHeightMap(float[,] heightmap)
        {
            var width = heightmap.GetLength(0);
            var height = heightmap.GetLength(1);
            
            var texture = new Texture2D(width, height);
            
            var colourMap = new Color[width * height];
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightmap[x, y]);
                }
            }

            return TextureFromColorMap(colourMap, width, height);
        }
    }
}