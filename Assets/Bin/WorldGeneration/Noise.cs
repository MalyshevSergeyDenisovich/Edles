using System;
using UnityEngine;

namespace Bin.WorldGeneration
{
    public static class Noise
    {
        public  static  float[,] GenerateNoseMap(
            int mapWidth,
            int mapHeight,
            int seed,
            float scale,
            int octaves,
            float persistence,
            float lacunarity,
            Vector2 offset)
        {
            var noiseMap = new float[mapWidth, mapHeight];

            var prng = new System.Random(seed);
            var octaveOffset = new Vector2[octaves];

            var halfWidth = mapWidth / 2f;
            var halfHeight = mapHeight / 2f;
            
            for (var i = 0; i < octaves; i++)
            {
                var offsetX = prng.Next(-100000, 100000) + offset.x;
                var offsetY = prng.Next(-100000, 100000) + offset.y;
                octaveOffset[i] = new Vector2(offsetX, offsetY);
            }
            
            if (scale <= 0)
            {
                scale = .0001f;
            }

            var maxNoiseHeight = float.MinValue;
            var minNoiseHeight = float.MaxValue;
            
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {

                    var amplitude = 1f;
                    var frequency = 1f;
                    var noiseHeight = 0f; 
                    
                    for (var i = 0; i < octaves; i++)
                    {
                        var sampleX = (x - halfWidth)  / scale * frequency + octaveOffset[i].x;
                        var sampleY = (y - halfHeight) / scale * frequency + octaveOffset[i].y;

                        var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }
                     
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
}