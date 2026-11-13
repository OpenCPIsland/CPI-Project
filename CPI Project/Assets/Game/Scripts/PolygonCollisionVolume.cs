using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PolygonCollisionVolume : MonoBehaviour
{
	public enum ColliderBehavior
	{
		SingleComplexMesh,
		SingleConvex,
		MultipleConvex
	}

	public ColliderBehavior colliderBehavior = ColliderBehavior.MultipleConvex;

	public bool generateTopAndBottom = true;

	public bool generateEdges = true;

	public bool isTrigger = true;

	[Tooltip("How tall will the extruded collider be?")]
	public float Height = 10f;

	[Range(0f, 1f)]
	[Tooltip("where to put the origin in the Y direction. 0 = bottom, 1 = top")]
	public float YOriginOffset = 0f;

	[SerializeField]
	[HideInInspector]
	private Vector2[] m_Points = new Vector2[4]
	{
		new Vector2(-1f, 1f),
		new Vector2(1f, 1f),
		new Vector2(1f, -1f),
		new Vector2(-1f, -1f)
	};

	[HideInInspector]
	[SerializeField]
	private bool m_Generated = false;

	public bool Dirty
	{
		get
		{
			return !m_Generated;
		}
	}

	public Vector2[] Points
	{
		get
		{
			return m_Points;
		}
		set
		{
			m_Points = value;
		}
	}

	private void Awake()
	{
	}

	private void Start()
	{
		if (!m_Generated)
		{
			_generateCollision();
		}
	}

	public void MarkDirty()
	{
		m_Generated = false;
	}

	public void Regenerate()
	{
		_generateCollision();
	}

	private void _generateCollision()
	{
		_cleanupChildren();
		List<Vector2[]> list = new List<Vector2[]>();
		list.Add(m_Points);
		List<Vector2[]> polysToProcess = list;
		List<Vector2[]> convexParts = new List<Vector2[]>();
		if (colliderBehavior == ColliderBehavior.MultipleConvex)
		{
			while (polysToProcess.Count > 0)
			{
				_convexifyFirstElement(ref polysToProcess, ref convexParts);
			}
		}
		else
		{
			convexParts.Add(m_Points);
		}
		foreach (Vector2[] item in convexParts)
		{
			GameObject gameObject = new GameObject("Collision Piece");
			MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = _buildMesh(item);
			if (colliderBehavior == ColliderBehavior.SingleConvex || colliderBehavior == ColliderBehavior.MultipleConvex || isTrigger)
			{
				meshCollider.convex = true;
			}
			if (isTrigger)
			{
				meshCollider.isTrigger = true;
			}
			gameObject.transform.SetParent(base.transform, false);
		}
		m_Generated = true;
	}

	private void _cleanupChildren()
	{
		while (base.transform.childCount > 0)
		{
			Transform child = base.transform.GetChild(0);
			child.SetParent(null);
			Object.Destroy(child.gameObject);
		}
	}

	private void _convexifyFirstElement(ref List<Vector2[]> polysToProcess, ref List<Vector2[]> convexParts)
	{
		Vector2[] poly = polysToProcess[0];
		bool flag = true;
		for (int i = 0; i < poly.Length; i++)
		{
			Vector2 ptA = poly[(i > 0) ? (i - 1) : (poly.Length - 1)];
			Vector2 ptB = poly[i];
			Vector2 ptC = poly[(i < poly.Length - 1) ? (i + 1) : 0];
			if (!TurnsRight(ptA, ptB, ptC))
			{
				flag = false;
				polysToProcess.Remove(poly);
				int connectionIdx;
				if (!_findConnectionVertex(poly, i, out connectionIdx))
				{
					connectionIdx = _insertSteinerVertex(ref poly, i);
				}
				List<Vector2> list = new List<Vector2>();
				for (int num = i; num != connectionIdx; num = (num + 1) % poly.Length)
				{
					list.Add(poly[num]);
				}
				list.Add(poly[connectionIdx]);
				polysToProcess.Add(list.ToArray());
				list.Clear();
				for (int num = connectionIdx; num != i; num = (num + 1) % poly.Length)
				{
					list.Add(poly[num]);
				}
				list.Add(poly[i]);
				polysToProcess.Add(list.ToArray());
				break;
			}
		}
		if (flag)
		{
			polysToProcess.Remove(poly);
			convexParts.Add(poly);
		}
	}

	public bool TurnsRight(Vector2 ptA, Vector2 ptB, Vector2 ptC)
	{
		Vector2 segA = ptB - ptA;
		Vector2 segB = ptC - ptB;
		return TurnsRight(segA, segB);
	}

	public bool TurnsRight(Vector2 segA, Vector2 segB)
	{
		float num = segA.x * segB.y - segA.y * segB.x;
		return num <= 0f;
	}

	public Vector2 GetPerpendicular(Vector2 a)
	{
		return new Vector2(a.y, 0f - a.x);
	}

	public bool LineIntersectionPoint(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2, out Vector2 intersectPoint)
	{
		intersectPoint = Vector2.zero;
		float num = a2.y - a1.y;
		float num2 = a1.x - a2.x;
		float num3 = num * a1.x + num2 * a1.y;
		float num4 = b2.y - b1.y;
		float num5 = b1.x - b2.x;
		float num6 = num4 * b1.x + num5 * b1.y;
		float num7 = num * num5 - num4 * num2;
		if (num7 == 0f)
		{
			return false;
		}
		intersectPoint = new Vector2((num5 * num3 - num2 * num6) / num7, (num * num6 - num4 * num3) / num7);
		return true;
	}

	public bool _findConnectionVertex(Vector2[] poly, int reflectionIdx, out int connectionIdx)
	{
		connectionIdx = -1;
		float num = float.PositiveInfinity;
		Vector2 ptA = poly[(reflectionIdx > 0) ? (reflectionIdx - 1) : (poly.Length - 1)];
		Vector2 vector = poly[reflectionIdx];
		Vector2 ptA2 = poly[(reflectionIdx < poly.Length - 1) ? (reflectionIdx + 1) : 0];
		for (int i = 0; i < poly.Length; i++)
		{
			if (i == reflectionIdx)
			{
				continue;
			}
			Vector2 vector2 = poly[i];
			if (TurnsRight(ptA, vector, vector2) && !TurnsRight(ptA2, vector, vector2))
			{
				float num2 = Vector2.Distance(vector, vector2);
				if (num2 < num)
				{
					num = num2;
					connectionIdx = i;
				}
			}
		}
		return connectionIdx >= 0;
	}

	public Vector2 _getPointNormal(Vector2[] poly, int pt)
	{
		Vector2 b = poly[(pt > 0) ? (pt - 1) : (poly.Length - 1)];
		Vector2 vector = poly[pt];
		Vector2 a = poly[(pt < poly.Length - 1) ? (pt + 1) : 0];
		return -(GetPerpendicular(vector - b).normalized + GetPerpendicular(a - vector).normalized).normalized;
	}

	public int _insertSteinerVertex(ref Vector2[] poly, int reflectionIdx)
	{
		Vector2 b = -_getPointNormal(poly, reflectionIdx);
		int num = -1;
		Vector2 vector = Vector2.zero;
		float num2 = float.PositiveInfinity;
		Vector2 ptA = poly[(reflectionIdx > 0) ? (reflectionIdx - 1) : (poly.Length - 1)];
		Vector2 vector2 = poly[reflectionIdx];
		Vector2 ptA2 = poly[(reflectionIdx < poly.Length - 1) ? (reflectionIdx + 1) : 0];
		for (int i = 0; i < poly.Length; i++)
		{
			if (i == reflectionIdx)
			{
				continue;
			}
			Vector2 vector3 = poly[i];
			Vector2 vector4 = poly[(i < poly.Length - 1) ? (i + 1) : 0];
			Vector2 intersectPoint;
			if (!TurnsRight(ptA, vector2, vector3) && TurnsRight(ptA2, vector2, vector4) && LineIntersectionPoint(vector3, vector4, vector2, vector2 + b, out intersectPoint))
			{
				float num3 = Vector2.Distance(vector2, intersectPoint);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
					vector = intersectPoint;
				}
			}
		}
		if (num >= 0)
		{
			Vector2[] array = new Vector2[poly.Length + 1];
			for (int i = 0; i <= num; i++)
			{
				array[i] = poly[i];
			}
			array[num + 1] = vector;
			for (int i = num + 1; i < poly.Length; i++)
			{
				array[i + 1] = poly[i];
			}
			poly = array;
		}
		return num + 1;
	}

	private Mesh _buildMesh(Vector2[] poly)
	{
		Mesh mesh = new Mesh();
		mesh.name = "PolygonVolumeMesh";
		int num = poly.Length;
		int num2 = num * 2;
		Vector3[] array = new Vector3[num2];
		int num3 = poly.Length * 2;
		int num4 = poly.Length * 2;
		int num5 = (generateTopAndBottom ? num3 : 0) + (generateEdges ? num4 : 0);
		int[] array2 = new int[num5 * 3];
		Vector2 lhs = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		Vector2 lhs2 = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
		foreach (Vector2 rhs in poly)
		{
			lhs = Vector2.Min(lhs, rhs);
			lhs2 = Vector2.Max(lhs2, rhs);
		}
		float y = Height * -0.5f * YOriginOffset;
		float y2 = Height * 0.5f * (1f - YOriginOffset);
		for (int i = 0; i < num; i++)
		{
			int num6 = i;
			int num7 = num + i;
			Vector2 vector = Vector2.zero;
			if (i < poly.Length)
			{
				vector = poly[i];
			}
			array[num6] = new Vector3(vector.x, y, vector.y);
			array[num7] = new Vector3(vector.x, y2, vector.y);
		}
		if (generateTopAndBottom)
		{
			for (int i = 1; i < poly.Length - 1; i++)
			{
				int num8 = i * 3;
				int num9 = poly.Length * 3 + i * 3;
				int num10 = i;
				int num11 = i + 1;
				int num12 = 0;
				array2[num8] = num10;
				array2[num8 + 1] = num12;
				array2[num8 + 2] = num11;
				array2[num9] = num10 + num;
				array2[num9 + 1] = num11 + num;
				array2[num9 + 2] = num12 + num;
			}
		}
		if (generateEdges)
		{
			for (int i = 0; i < poly.Length; i++)
			{
				int num13 = (generateTopAndBottom ? (num3 * 3) : 0) + i * 6;
				int num10 = i;
				int num11 = (i < poly.Length - 1) ? (i + 1) : 0;
				int num12 = num10 + num;
				int num14 = num11 + num;
				array2[num13] = num10;
				array2[num13 + 1] = num11;
				array2[num13 + 2] = num12;
				array2[num13 + 3] = num11;
				array2[num13 + 4] = num14;
				array2[num13 + 5] = num12;
			}
		}
		mesh.vertices = array;
		mesh.triangles = array2;
		mesh.RecalculateNormals();
		return mesh;
	}
}
