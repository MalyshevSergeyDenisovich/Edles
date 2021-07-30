using Bin.WorldGeneration.Data;
using UnityEngine;

namespace Bin.WorldGeneration
{
	public static class MeshGenerator 
	{
		public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail)
		{
			var meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;


			var borderedSize = heightMap.GetLength(0);
			var meshSize = borderedSize - 2 * meshSimplificationIncrement;
			var meshSizeUnsimplified = borderedSize - 2;

			var topLeftX = (meshSizeUnsimplified - 1) / -2f;
			var topLeftZ = (meshSizeUnsimplified - 1) / 2f;

			var verticesPerLine = (meshSize - 1) / meshSimplificationIncrement + 1;

			var meshData = new MeshData(verticesPerLine, meshSettings.useFlatShading);

			var vertexIndicesMap = new int[borderedSize][];
			for (var index = 0; index < borderedSize; index++)
			{
				vertexIndicesMap[index] = new int[borderedSize];
			}

			var meshVertexIndex = 0;
			var borderVertexIndex = -1;

			for (var y = 0; y < borderedSize; y += meshSimplificationIncrement) 
			{
				for (var x = 0; x < borderedSize; x += meshSimplificationIncrement)
				{
					var isBorderVertex = y == 0 || y == borderedSize - 1 || x == 0 || x == borderedSize - 1;

					if (isBorderVertex)
					{
						vertexIndicesMap[x][y] = borderVertexIndex;
						borderVertexIndex--;
					} else
					{
						vertexIndicesMap[x][y] = meshVertexIndex;
						meshVertexIndex++;
					}
				}
			}

			for (var y = 0; y < borderedSize; y += meshSimplificationIncrement) 
			{
				for (var x = 0; x < borderedSize; x += meshSimplificationIncrement)
				{
					var vertexIndex = vertexIndicesMap[x][y];
					var percent = new Vector2((x - meshSimplificationIncrement) / (float) meshSize,
						(y - meshSimplificationIncrement) / (float) meshSize);
					var height = heightMap[x, y];
					var vertexPosition =
						new Vector3((topLeftX + percent.x * meshSizeUnsimplified) * meshSettings.meshScale, height,
							(topLeftZ - percent.y * meshSizeUnsimplified) * meshSettings.meshScale);

					meshData.AddVertex(vertexPosition, percent, vertexIndex);

					if (x < borderedSize - 1 && y < borderedSize - 1)
					{
						var a = vertexIndicesMap[x][y];
						var b = vertexIndicesMap[x + meshSimplificationIncrement][y];
						var c = vertexIndicesMap[x][y + meshSimplificationIncrement];
						var d = vertexIndicesMap[x + meshSimplificationIncrement][y + meshSimplificationIncrement];
						meshData.AddTriangle(a, d, c);
						meshData.AddTriangle(d, a, b);
					}
				}
			}

			meshData.ProcessMesh();

			return meshData;

		}
	}

	public class MeshData
	{
		private Vector3[] _vertices;
		private readonly int[] _triangles;
		private Vector2[] _uvs;
		private Vector3[] _bakedNormals;

		private readonly Vector3[] _borderVertices;
		private readonly int[] _borderTriangles;

		private int _triangleIndex;
		private int _borderTriangleIndex;

		private readonly bool _useFlatShading;

		public MeshData(int verticesPerLine, bool useFlatShading)
		{
			_useFlatShading = useFlatShading;

			_vertices = new Vector3[verticesPerLine * verticesPerLine];
			_uvs = new Vector2[verticesPerLine * verticesPerLine];
			_triangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

			_borderVertices = new Vector3[verticesPerLine * 4 + 4];
			_borderTriangles = new int[24 * verticesPerLine];
		}

		public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
		{
			if (vertexIndex < 0)
			{
				_borderVertices[-vertexIndex - 1] = vertexPosition;
			} else
			{
				_vertices[vertexIndex] = vertexPosition;
				_uvs[vertexIndex] = uv;
			}
		}

		public void AddTriangle(int a, int b, int c) 
		{
			if (a < 0 || b < 0 || c < 0)
			{
				_borderTriangles[_borderTriangleIndex] = a;
				_borderTriangles[_borderTriangleIndex + 1] = b;
				_borderTriangles[_borderTriangleIndex + 2] = c;
				_borderTriangleIndex += 3;
			} else {
				_triangles[_triangleIndex] = a;
				_triangles[_triangleIndex + 1] = b;
				_triangles[_triangleIndex + 2] = c;
				_triangleIndex += 3;
			}
		}

		private Vector3[] CalculateNormals()
		{
			var vertexNormals = new Vector3[_vertices.Length];
			var triangleCount = _triangles.Length / 3;
			for (var i = 0; i < triangleCount; i++)
			{
				var normalTriangleIndex = i * 3;
				var vertexIndexA = _triangles[normalTriangleIndex];
				var vertexIndexB = _triangles[normalTriangleIndex + 1];
				var vertexIndexC = _triangles[normalTriangleIndex + 2];

				var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
				vertexNormals[vertexIndexA] += triangleNormal;
				vertexNormals[vertexIndexB] += triangleNormal;
				vertexNormals[vertexIndexC] += triangleNormal;
			}

			var borderTriangleCount = _borderTriangles.Length / 3;
			for (var i = 0; i < borderTriangleCount; i++)
			{
				var normalTriangleIndex = i * 3;
				var vertexIndexA = _borderTriangles[normalTriangleIndex];
				var vertexIndexB = _borderTriangles[normalTriangleIndex + 1];
				var vertexIndexC = _borderTriangles[normalTriangleIndex + 2];

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
			var pointA = (indexA < 0) ? _borderVertices[-indexA - 1] : _vertices[indexA];
			var pointB = (indexB < 0) ? _borderVertices[-indexB - 1] : _vertices[indexB];
			var pointC = (indexC < 0) ? _borderVertices[-indexC - 1] : _vertices[indexC];

			var sideAB = pointB - pointA;
			var sideAC = pointC - pointA;
			return Vector3.Cross(sideAB, sideAC).normalized;
		}

		public void ProcessMesh()
		{
			if (_useFlatShading)
			{
				FlatShading();
			} else {
				BakeNormals();
			}
		}

		private void BakeNormals()
		{
			_bakedNormals = CalculateNormals();
		}

		private void FlatShading()
		{
			var flatShadedVertices = new Vector3[_triangles.Length];
			var flatShadedUvs = new Vector2[_triangles.Length];

			for (var i = 0; i < _triangles.Length; i++)
			{
				flatShadedVertices[i] = _vertices[_triangles[i]];
				flatShadedUvs[i] = _uvs[_triangles[i]];
				_triangles[i] = i;
			}

			_vertices = flatShadedVertices;
			_uvs = flatShadedUvs;
		}

		
		public Mesh CreateMesh()
		{
			var mesh = new Mesh {vertices = _vertices, triangles = _triangles, uv = _uvs};
			if (_useFlatShading)
			{
				mesh.RecalculateNormals();
			} else {
				mesh.normals = _bakedNormals;
			}

			return mesh;
		}
	}
}