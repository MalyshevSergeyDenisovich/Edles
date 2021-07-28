﻿using UnityEditor.Animations;
using UnityEngine;

namespace Bin.WorldGeneration
{
    public static class MeshGenerator
    {
        public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
        {
            var localHeightCurve = new AnimationCurve(heightCurve.keys);
            
            var meshSimplificationIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            var borderSize = heightMap.GetLength(0);
            var meshSize = borderSize - 2 * meshSimplificationIncrement;
            var meshSizeUnsimplified = borderSize - 2;
                
            var topLeftX = (meshSizeUnsimplified - 1) / -2f;
            var topLeftZ = (meshSizeUnsimplified - 1) / 2f;


            var verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;
            
            var meshData = new MeshData(borderSize);


            var vertexIndicesMap = new int[borderSize][];
            for (var index = 0; index < borderSize; index++)
            {
                vertexIndicesMap[index] = new int[borderSize];
            }

            var meshVertexIndex = 0;
            var borderVertexIndex = -1;

            for (var y = 0; y < borderSize; y += meshSimplificationIncrement)
            {
                for (var x = 0; x < borderSize; x += meshSimplificationIncrement)
                {
                    var isBorderVertex = y == 0 || y == borderSize - 1 || x == 0 || x == borderSize - 1;

                    if (isBorderVertex) {
                        vertexIndicesMap[x][y] = borderVertexIndex;
                        borderVertexIndex--;
                    } else {
                        vertexIndicesMap[x][y] = meshVertexIndex;
                        meshVertexIndex++;
                    }
                }
            }


            for (var y = 0; y < borderSize; y += meshSimplificationIncrement)
            {
                for (var x = 0; x < borderSize; x += meshSimplificationIncrement)
                {
                    var vertexIndex = vertexIndicesMap[x][y];
                    var percent = new Vector2((x - meshSimplificationIncrement) / (float) meshSize,
                        (y - meshSimplificationIncrement) / (float) meshSize);
                    var height = localHeightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;
                    var vertexPosition = new Vector3(topLeftX + percent.x * meshSizeUnsimplified, height, topLeftZ - percent.y * meshSizeUnsimplified);
                    
                    meshData.AddVertex(vertexPosition, percent, vertexIndex);
                    
                    if (x < borderSize - 1 && y < borderSize - 1)
                    {
                        var a = vertexIndicesMap[x][y];
                        var b = vertexIndicesMap[x + meshSimplificationIncrement][y];
                        var c = vertexIndicesMap[x][y + meshSimplificationIncrement];
                        var d = vertexIndicesMap[x + meshSimplificationIncrement][y + meshSimplificationIncrement];
                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }
                    vertexIndex++;
                }
            }
            
            meshData.BakeNormals();

            return meshData;
        }
    }

    public class MeshData
    {
        private readonly Vector3[] vertices;
        private readonly int[] triangles;
        private readonly Vector2[] uvs;
        private Vector3[] bakedNormals;

        private Vector3[] borderVertices;
        private int[] borderTriangles;
        
        private int triangleIndex;
        private int borderTriangleIndex;
        
        public MeshData(int verticesPerLine)
        {
            vertices = new Vector3[verticesPerLine * verticesPerLine];
            uvs = new Vector2[verticesPerLine * verticesPerLine];
            triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

            borderVertices = new Vector3[verticesPerLine * 4 + 4];
            borderTriangles = new int[24 * verticesPerLine];
        }

        public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
        {
            if (vertexIndex < 0)
            {
                borderVertices[-vertexIndex - 1] = vertexPosition;
            } else {
                vertices[vertexIndex] = vertexPosition;
                uvs[vertexIndex] = uv;
            }
        }

        public void AddTriangle(int a, int b, int c)
        {
            if (a < 0 || b < 0 || c < 0)
            {
                borderTriangles[borderTriangleIndex] = a;
                borderTriangles[borderTriangleIndex + 1] = b;
                borderTriangles[borderTriangleIndex + 2] = c;
                borderTriangleIndex += 3;
            }
            else
            {
                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = b;
                triangles[triangleIndex + 2] = c;
                triangleIndex += 3;
            }
        }

        private Vector3[] CalculateNormals()
        {
            var vertexNormals = new Vector3[vertices.Length];
            var triangleCount = triangles.Length / 3;
            for (var i = 0; i < triangleCount; i++)
            {
                var normalTriangleIndex = i * 3;
                var vertexIndexA = triangles[normalTriangleIndex];
                var vertexIndexB = triangles[normalTriangleIndex + 1];
                var vertexIndexC = triangles[normalTriangleIndex + 2];

                var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;

            }
            
            var borderTriangleCount = borderTriangles.Length / 3;
            for (var i = 0; i < borderTriangleCount; i++)
            {
                var normalTriangleIndex = i * 3;
                var vertexIndexA = borderTriangles[normalTriangleIndex];
                var vertexIndexB = borderTriangles[normalTriangleIndex + 1];
                var vertexIndexC = borderTriangles[normalTriangleIndex + 2];

                var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                if (vertexIndexA >= 0)
                {
                    vertexNormals[vertexIndexA] += triangleNormal; 
                }

                if (vertexIndexB >= 0)
                {
                    vertexNormals[vertexIndexB] += triangleNormal; 
                }

                if (vertexIndexC >= 0)
                {
                    vertexNormals[vertexIndexC] += triangleNormal;
                }
            }

            for (var i = 0; i < vertexNormals.Length; i++)
            {
                vertexNormals[i].Normalize();
            }
            
            return vertexNormals;
        }

        private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
        {
            var pointA = indexA < 0 ? borderVertices[-indexA - 1] : vertices[indexA];
            var pointB = indexB < 0 ? borderVertices[-indexB - 1] : vertices[indexB];
            var pointC = indexC < 0 ? borderVertices[-indexC - 1] : vertices[indexC];

            var sideAB = pointB - pointA;
            var sideAC = pointC - pointA;
            return Vector3.Cross(sideAB, sideAC);
        }

        public void BakeNormals()
        {
            bakedNormals = CalculateNormals();
        }

        public Mesh CreateMesh()
        {
            var mesh = new Mesh {vertices = vertices, triangles = triangles, uv = uvs, normals = bakedNormals};
            return mesh;
        }
    }
}