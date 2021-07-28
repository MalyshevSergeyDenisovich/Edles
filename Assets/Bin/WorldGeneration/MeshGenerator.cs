using UnityEditor.Animations;
using UnityEngine;

namespace Bin.WorldGeneration
{
    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
        {
            var localHeightCurve = new AnimationCurve(heightCurve.keys); 
            var width = heightMap.GetLength(0);
            var height = heightMap.GetLength(1);
            var topLeftX = (width - 1) / -2f;
            var topLeftZ = (height - 1) / 2f;

            var meshSimplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            var verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;
            
            var meshData = new MeshData(width, height);
            var vertexIndex = 0;
            
            for (var y = 0; y < height; y += meshSimplificationIncrement)
            {
                for (var x = 0; x < width; x += meshSimplificationIncrement)
                {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, localHeightCurve.Evaluate(heightMap[x,y]) * heightMultiplier, topLeftZ - y);
                    meshData.uvs[vertexIndex] = new Vector2(x / (float) width, y / (float) height);
                    
                    if (x < width - 1 && y < height - 1)
                    {
                        meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                        meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                    }
                    vertexIndex++;
                }
            }
            return meshData;
        }
    }

    public class MeshData
    {
        public readonly Vector3[] vertices;
        public readonly int[] triangles;
        public readonly Vector2[] uvs;
        
        private int _triangleIndex;
        
        public MeshData(int meshWidth, int meshHeight)
        {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        public void AddTriangle(int a, int b, int c)
        {
            triangles[_triangleIndex] = a;
            triangles[_triangleIndex + 1] = b;
            triangles[_triangleIndex + 2] = c;
            _triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            var mesh = new Mesh {vertices = vertices, triangles = triangles, uv = uvs};
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}