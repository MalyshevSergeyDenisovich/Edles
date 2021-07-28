using System;
using UnityEngine;

namespace Bin.WorldGeneration
{
    public static class Noise
    {
        public enum NormalizeMode { Local, Global }
        public  static  float[,] GenerateNoseMap(
            int mapWidth,
            int mapHeight,
            int seed,
            float scale,
            int octaves,
            float persistence,
            float lacunarity,
            Vector2 offset,
            NormalizeMode normalizeMode
            )
        {
            var noiseMap = new float[mapWidth, mapHeight];

            var prng = new System.Random(seed);
            var octaveOffset = new Vector2[octaves];

            var maxPossibleHeight = 0f;
            var amplitude = 1f;
            var frequency = 1f;
            
            var halfWidth = mapWidth / 2f;
            var halfHeight = mapHeight / 2f;
            
            for (var i = 0; i < octaves; i++)
            {
                var offsetX = prng.Next(-100000, 100000) + offset.x;
                var offsetY = prng.Next(-100000, 100000) - offset.y;
                octaveOffset[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= persistence;
            }
            
            if (scale <= 0)
            {
                scale = .0001f;
            }

            var maxLocalNoiseHeight = float.MinValue;
            var minLocalNoiseHeight = float.MaxValue;
            
            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {

                    amplitude = 1f;
                    frequency = 1f;
                    var noiseHeight = 0f; 
                    
                    for (var i = 0; i < octaves; i++)
                    {
                        var sampleX = (x - halfWidth  + octaveOffset[i].x)  / scale * frequency;
                        var sampleY = (y - halfHeight + octaveOffset[i].y) / scale * frequency;

                        var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistence;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxLocalNoiseHeight)
                    {
                        maxLocalNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minLocalNoiseHeight)
                    {
                        minLocalNoiseHeight = noiseHeight;
                    }
                     
                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (var y = 0; y < mapHeight; y++)
            {
                for (var x = 0; x < mapWidth; x++)
                {
                    if (normalizeMode == NormalizeMode.Local)
                    {
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                    }
                    else
                    {
                        var normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight);
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            return noiseMap;
        }
    }
}