using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Grass
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class GrassQuadGridGenerator : MonoBehaviour
	{
		[SerializeField]
		private Mesh _gridMesh;

		[SerializeField]
		private Mesh _quadMesh;

		private List<Vector3> _vertexBuffer;
		private List<Vector3> _normalBuffer;
		private List<Vector2> _uvBuffer;
		private List<int> _triangleBuffer;

		[SerializeField]
		private Mesh _resultMesh;

		[SerializeField]
		private GrassParameters _parameters;

		[ContextMenu("Generate")]
		private void Generate()
		{
			_vertexBuffer = new List<Vector3>();
			_normalBuffer = new List<Vector3>();
			_uvBuffer = new List<Vector2>();
			_triangleBuffer = new List<int>();

			_resultMesh = new Mesh();
			_resultMesh.MarkDynamic();

			var gridVertices = _gridMesh.vertices;
			foreach (var each in gridVertices)
			{
				AddMeshToBuffers(each, _quadMesh);
			}

			_resultMesh.SetVertices(_vertexBuffer);
			_resultMesh.SetNormals(_normalBuffer);
			_resultMesh.SetUVs(0, _uvBuffer);
			_resultMesh.SetTriangles(_triangleBuffer, 0);

			GetComponent<MeshFilter> ().sharedMesh = _resultMesh;
		}

		private void AddMeshToBuffers(Vector3 centerPosition, Mesh mesh)
		{
			var worldPosition = transform.localToWorldMatrix.MultiplyPoint3x4 (centerPosition);
			var scale = _parameters.GetScale (worldPosition);
			var offset = _parameters.GetOffset (worldPosition);

			_triangleBuffer.AddRange(mesh.triangles.Select(_ => _ + _vertexBuffer.Count).ToArray());

			_vertexBuffer.AddRange(mesh.vertices.Select(_ => _ * scale + offset + centerPosition).ToArray());
			_normalBuffer.AddRange(mesh.normals);
			_uvBuffer.AddRange(mesh.uv);
		}
	}
}