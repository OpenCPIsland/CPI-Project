using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Core
{
	public static class BezierMath
	{
		public struct Curve4
		{
			public readonly Vector3 p0;

			public readonly Vector3 p1;

			public readonly Vector3 p2;

			public readonly Vector3 p3;

			public Curve4(Vector3 _p0, Vector3 _p1, Vector3 _p2, Vector3 _p3)
			{
				p0 = _p0;
				p1 = _p1;
				p2 = _p2;
				p3 = _p3;
			}
		}

		public struct Iterator3
		{
			public readonly int Steps;

			private Vector3 f;

			private Vector3 fd;

			private readonly Vector3 fdd;

			public Vector3 Current
			{
				get
				{
					return f;
				}
			}

			public Iterator3(Vector3 p0, Vector3 p1, Vector3 p2, int steps = 25)
			{
				Steps = steps;
				float num = 1f / (float)Steps;
				float d = num * num;
				Vector3 a = (p0 - p1 * 2f + p2) * d;
				f = p0;
				fd = a + (p1 - p0) * 2f * num;
				fdd = a * 2f;
			}

			public static int CalculateSteps(Vector3 p0, Vector3 p1, Vector3 p2, float scale)
			{
				Vector3 vector = p1 - p0;
				Vector3 vector2 = p2 - p1;
				float num = (vector.magnitude + vector2.magnitude) * 0.25f * scale;
				return Math.Max(Mathf.FloorToInt(num), 4);
			}

			public Vector3 Next()
			{
				f += fd;
				fd += fdd;
				return f;
			}
		}

		public struct Iterator4
		{
			public readonly int Steps;

			private Vector3 f;

			private Vector3 fd;

			private Vector3 fdd;

			private readonly Vector3 fddd;

			public Vector3 Current
			{
				get
				{
					return f;
				}
			}

			public Iterator4(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int steps = 25)
			{
				Steps = steps;
				float num = 1f / (float)Steps;
				float num2 = num * num;
				float num3 = num2 * num;
				float d = 3f * num;
				float d2 = 3f * num2;
				float d3 = 6f * num2;
				float d4 = 6f * num3;
				Vector3 a = p0 - p1 * 2f + p2;
				Vector3 a2 = (p1 - p2) * 3f - p0 + p3;
				f = p0;
				fd = (p1 - p0) * d + a * d2 + a2 * num3;
				fdd = a * d3 + a2 * d4;
				fddd = a2 * d4;
			}

			public static int CalculateSteps(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float scale)
			{
				Vector3 vector = p1 - p0;
				Vector3 vector2 = p2 - p1;
				Vector3 vector3 = p3 - p2;
				float num = (vector.magnitude + vector2.magnitude + vector3.magnitude) * 0.25f * scale;
				return Math.Max(Mathf.FloorToInt(num), 4);
			}

			public Vector3 Next()
			{
				f += fd;
				fd += fdd;
				fdd += fddd;
				return f;
			}
		}

		public static void Subdivide(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, List<Vector3> points, float tolerance = 0.0001f, int recursionLimit = 32)
		{
			points.Clear();
			Stack<Curve4> stack = new Stack<Curve4>(recursionLimit * 2 + 1);
			stack.Push(new Curve4(p0, p1, p2, p3));
			while (stack.Count > 0)
			{
				Curve4 curve = stack.Pop();
				Vector3 vector = (curve.p0 + curve.p1) * 0.5f;
				Vector3 vector2 = (curve.p1 + curve.p2) * 0.5f;
				Vector3 vector3 = (curve.p2 + curve.p3) * 0.5f;
				Vector3 vector4 = (vector + vector2) * 0.5f;
				Vector3 vector5 = (vector2 + vector3) * 0.5f;
				Vector3 vector6 = (vector4 + vector5) * 0.5f;
				Vector3 vector7 = curve.p0 + curve.p2 - curve.p1 - curve.p1;
				Vector3 vector8 = curve.p1 + curve.p3 - curve.p2 - curve.p2;
				if (vector7.sqrMagnitude + vector8.sqrMagnitude < tolerance)
				{
					points.Add(vector6);
				}
				else if (recursionLimit > 0)
				{
					stack.Push(new Curve4(curve.p0, vector, vector4, vector6));
					stack.Push(new Curve4(vector6, vector5, vector3, curve.p3));
					recursionLimit--;
				}
			}
		}

		public static Vector3 FindClosest(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 point, out float distance, int subdivisions = 50)
		{
			float num = float.MaxValue;
			Vector3 result = point;
			Iterator4 iterator = new Iterator4(p0, p1, p2, p3, subdivisions);
			for (int i = 0; i < subdivisions; i++)
			{
				Vector3 current = iterator.Current;
				Vector3 a = iterator.Next();
				Vector3 vector = a - current;
				Vector3 lhs = point - current;
				float value = Vector3.Dot(lhs, vector) / (Vector3.Dot(vector, vector) + float.Epsilon);
				Vector3 vector2 = current + Mathf.Clamp(value, 0f, 1f) * vector;
				float num2 = Vector3.SqrMagnitude(point - vector2);
				if (num2 < num)
				{
					num = num2;
					result = vector2;
				}
			}
			distance = Mathf.Sqrt(num);
			return result;
		}

		public static Vector3 Interpolate(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float u)
		{
			float num = 1f - u;
			float d = num * num * num;
			float d2 = 3f * u * num * num;
			float d3 = 3f * u * u * num;
			float d4 = u * u * u;
			return p0 * d + p1 * d2 + p2 * d3 + p3 * d4;
		}

		public static Vector3 Tangent(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float u)
		{
			float num = 1f - u;
			return 3f * num * num * (p1 - p0) + 6f * u * num * (p2 - p1) + 3f * u * u * (p3 - p2);
		}

		public static Vector3 Interpolate(Vector3 p0, Vector3 p1, Vector3 p2, float u)
		{
			float num = 1f - u;
			float d = num * num;
			float d2 = 2f * u * num;
			float d3 = u * u;
			return p0 * d + p1 * d2 + p2 * d3;
		}

		public static Vector3 Tangent(Vector3 p0, Vector3 p1, Vector3 p2, float u)
		{
			float num = 1f - u;
			return 2f * num * (p1 - p0) + 2f * u * (p2 - p1);
		}

		public static Vector3 Interpolate(Vector3 p0, Vector3 p1, float u)
		{
			float d = 1f - u;
			return p0 * d + p1 * u;
		}
	}
}
