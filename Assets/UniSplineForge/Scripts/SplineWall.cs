using System;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Splines;

namespace UniSplineForge.Scripts
{
	[StructLayout(LayoutKind.Sequential)]
	struct VertexData
	{
		public Vector3 position { get; set; }
		// public Vector3 normal { get; set; }
		// public Vector2 texture { get; set; }
	}
	
	[RequireComponent(typeof(SplineContainer), typeof(MeshFilter))]
	public class SplineWall : MonoBehaviour
	{
		[SerializeField, Range(2, 100)] private int divided = 10;
		[SerializeField] private float height = 5.0f;
		
		private SplineContainer splineContainer;
		private Mesh mesh;
		private MeshFilter meshFilter;

		private void Reset()
		{
			PrepareComponents();
			Rebuild();
		}
		private void OnEnable()
		{
			Debug.Log($"OnEnable");
			PrepareComponents();
			
			Spline.Changed += OnSplineChanged;
		}

		private void OnDisable()
		{
			Debug.Log($"OnDisable");
			Spline.Changed -= OnSplineChanged;
		}

		private void PrepareComponents()
		{
			TryGetComponent(out splineContainer);
			TryGetComponent(out meshFilter);
			mesh = new Mesh();
			meshFilter.sharedMesh = mesh;
		}

		private void OnSplineChanged(Spline spline, int knotIndex, SplineModification modification)
		{
			Debug.Log($"OnSplineChanged: {spline}, {knotIndex}, {modification}");
			if (splineContainer != null && splineContainer.Splines.Contains(spline))
			{
				Rebuild();
			}
		}

		public void Rebuild()
		{
			var spline = splineContainer.Spline;
			if (spline == null) return;

			mesh.Clear();
			var meshDataArray = Mesh.AllocateWritableMeshData(1);
			var meshData = meshDataArray[0];
			meshData.subMeshCount = 1;

			// 頂点数とインデックス数を計算する
			var vertexCount = 2 * (divided + 1);
			var indexCount = 6 * divided;
			
			// インデックスと頂点のフォーマットを指定する
			// var indexFormat = vertexCount >= ushort.MaxValue ? IndexFormat.UInt32 : IndexFormat.UInt16;
			var indexFormat = IndexFormat.UInt32;
			meshData.SetIndexBufferParams(indexCount, indexFormat);
			meshData.SetVertexBufferParams(vertexCount, new VertexAttributeDescriptor[]
			{
				new (VertexAttribute.Position),
				// new (VertexAttribute.Normal),
				// new (VertexAttribute.TexCoord0, dimension: 2)
			});

			var vertices = meshData.GetVertexData<VertexData>();
			var indices = meshData.GetIndexData<UInt32>();

			for (int i = 0; i <= divided; ++i)
			{
				// 頂点座標を計算する
				spline.Evaluate((float)i / divided, out var position, out var direction, out var splineUp);
				var p0 = vertices[2 * i];
				var p1 = vertices[2 * i + 1];
				p0.position = position;
				p1.position = position + new float3(0, height, 0);
				vertices[2 * i] = p0;
				vertices[2 * i + 1] = p1;
				Debug.Log($"{p0.position}, {p1.position}");
			}

			for (int i = 0; i < divided; ++i)
			{
				indices[6 * i + 0] = (UInt32)(2 * i + 0);
				indices[6 * i + 1] = (UInt32)(2 * i + 1);
				indices[6 * i + 2] = (UInt32)(2 * i + 2);
				indices[6 * i + 3] = (UInt32)(2 * i + 1);
				indices[6 * i + 4] = (UInt32)(2 * i + 3);
				indices[6 * i + 5] = (UInt32)(2 * i + 2);
			}
			
			meshData.SetSubMesh(0, new SubMeshDescriptor(0,  indexCount));
			
			Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
			mesh.RecalculateBounds();
		}
	}
}