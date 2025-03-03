#define UNITY_ASSERTIONS
using UnityEngine;

namespace ClubPenguin.Core
{
	public class SmoothBezierCurve : MonoBehaviour
	{
		public Color Color = Color.white;

		private SmoothBezierNode[] cachedNodes;

		private SmoothBezierNode[] nodes
		{
			get
			{
				if (false || cachedNodes == null)
				{
					cachedNodes = GetComponentsInChildren<SmoothBezierNode>();
				}
				return cachedNodes;
			}
		}

		public int SegmentCount
		{
			get
			{
				return NodeCount - 1;
			}
		}

		public int NodeCount
		{
			get
			{
				return nodes.Length;
			}
		}

		public void Awake()
		{
			cachedNodes = GetComponentsInChildren<SmoothBezierNode>();
		}

		public Vector3 GetClosestPoint(Vector3 p, ref int closestSeg, out float distance, int searchRange = 5, int subdivisions = 50)
		{
			distance = float.MaxValue;
			Vector3 result = p;
			if (nodes.Length > 1)
			{
				int num = Mathf.Max(closestSeg - searchRange, 1);
				int num2 = Mathf.Min(closestSeg + searchRange, nodes.Length - 1);
				for (int i = num; i <= num2; i++)
				{
					Transform transform = nodes[i - 1].transform;
					Transform transform2 = nodes[i].transform;
					Vector3 position = transform.position;
					Vector3 p2 = transform.position + transform.forward * nodes[i - 1].OutHandleLength;
					Vector3 p3 = transform2.position - transform2.forward * nodes[i].InHandleLength;
					Vector3 position2 = transform2.position;
					float distance2;
					Vector3 vector = BezierMath.FindClosest(position, p2, p3, position2, p, out distance2, subdivisions);
					if (distance2 < distance)
					{
						distance = distance2;
						result = vector;
						closestSeg = i;
					}
				}
			}
			return result;
		}

		public Vector3 Interpolate(float u)
		{
			float num = 1f / (float)SegmentCount;
			int num2 = Mathf.FloorToInt(u / num);
			u = (u - (float)num2 * num) / num;
			return InterpolateSegment(num2, u);
		}

		public Vector3 InterpolateSegment(int segment, float u)
		{
			Debug.Assert(nodes.Length > segment, "Invalid segment number.");
			SmoothBezierNode smoothBezierNode = nodes[segment];
			SmoothBezierNode smoothBezierNode2 = nodes[segment + 1];
			Transform transform = smoothBezierNode.transform;
			Transform transform2 = smoothBezierNode2.transform;
			Vector3 position = transform.position;
			Vector3 p = transform.position + transform.forward * smoothBezierNode.OutHandleLength;
			Vector3 p2 = transform2.position - transform2.forward * smoothBezierNode2.InHandleLength;
			Vector3 position2 = transform2.position;
			return BezierMath.Interpolate(position, p, p2, position2, u);
		}

		public Vector3 InterpolateTangent(float u, out Vector3 point)
		{
			float num = 1f / (float)SegmentCount;
			int num2 = Mathf.FloorToInt(u / num);
			u = (u - (float)num2 * num) / num;
			return InterpolateSegmentTangent(num2, u, out point);
		}

		public Vector3 InterpolateSegmentTangent(int segment, float u, out Vector3 point)
		{
			Debug.Assert(nodes.Length > segment, "Invalid segment number.");
			SmoothBezierNode smoothBezierNode = nodes[segment];
			SmoothBezierNode smoothBezierNode2 = nodes[segment + 1];
			Transform transform = smoothBezierNode.transform;
			Transform transform2 = smoothBezierNode2.transform;
			Vector3 position = transform.position;
			Vector3 p = transform.position + transform.forward * smoothBezierNode.OutHandleLength;
			Vector3 p2 = transform2.position - transform2.forward * smoothBezierNode2.InHandleLength;
			Vector3 position2 = transform2.position;
			point = BezierMath.Interpolate(position, p, p2, position2, u);
			return BezierMath.Tangent(position, p, p2, position2, u);
		}

		public float EstimateLength(int subdivisions = 50)
		{
			float num = 0f;
			for (int i = 0; i < nodes.Length - 1; i++)
			{
				num += EstimateSegmentLength(i, subdivisions);
			}
			return num;
		}

		public float EstimateSegmentLength(int segment, int subdivisions = 50)
		{
			float num = 0f;
			Debug.Assert(nodes.Length > segment, "Invalid segment number.");
			SmoothBezierNode smoothBezierNode = nodes[segment];
			SmoothBezierNode smoothBezierNode2 = nodes[segment + 1];
			Transform transform = smoothBezierNode.transform;
			Transform transform2 = smoothBezierNode2.transform;
			Vector3 position = transform.position;
			Vector3 p = transform.position + transform.forward * smoothBezierNode.OutHandleLength;
			Vector3 p2 = transform2.position - transform2.forward * smoothBezierNode2.InHandleLength;
			Vector3 position2 = transform2.position;
			Vector3 zero = Vector3.zero;
			Vector3 vector = BezierMath.Interpolate(position, p, p2, position2, 0f);
			float num2 = 1f / (float)subdivisions;
			for (int i = 0; i < subdivisions; i++)
			{
				zero = vector;
				vector = BezierMath.Interpolate(position, p, p2, position2, num2 * (float)(i + 1));
				num += Vector3.Distance(zero, vector);
			}
			return num;
		}

		public void OnDrawGizmos()
		{
			Gizmos.color = Color;
			for (int i = 0; i < nodes.Length - 1; i++)
			{
				Transform transform = nodes[i].transform;
				Transform transform2 = nodes[i + 1].transform;
				Vector3 position = transform.position;
				Vector3 p = transform.position + transform.forward * nodes[i].OutHandleLength;
				Vector3 p2 = transform2.position - transform2.forward * nodes[i + 1].InHandleLength;
				Vector3 position2 = transform2.position;
				BezierMath.Iterator4 iterator = new BezierMath.Iterator4(position, p, p2, position2);
				for (int j = 0; j < iterator.Steps; j++)
				{
					Gizmos.DrawLine(iterator.Current, iterator.Next());
				}
			}
			renameAllNodes();
		}

		private void renameAllNodes()
		{
			for (int i = 0; i < nodes.Length; i++)
			{
				string text = "Node " + i;
				if (!nodes[i].name.Equals(text))
				{
					nodes[i].name = text;
				}
			}
		}
	}
}
