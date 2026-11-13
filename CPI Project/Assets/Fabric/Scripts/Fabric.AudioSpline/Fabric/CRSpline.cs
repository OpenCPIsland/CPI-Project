using System;
using UnityEngine;

namespace Fabric
{
	[Serializable]
	public class CRSpline
	{
		public AudioSplinePoint[] pts;

		private Vector3[] _bakedPoints;

		private Vector3 _prevBakedPoint;

		public void AddPoint(int index, AudioSplinePoint point)
		{
			index++;
			pts = MyArray<AudioSplinePoint>.InsertAt(pts, index, point);
			if (pts[index - 1] != null && pts[index + 1] != null)
			{
				Vector3 position = Vector3.Lerp(pts[index - 1].transform.position, pts[index + 1].transform.position, 0.5f);
				point.transform.position = position;
			}
			RefreshPointNames();
		}

		public AudioSplinePoint RemovePoint(int index)
		{
			AudioSplinePoint result = pts[index];
			pts = MyArray<AudioSplinePoint>.RemoveAt(pts, index);
			RefreshPointNames();
			return result;
		}

		public void RefreshPointNames()
		{
			for (int i = 0; i < pts.Length; i++)
			{
				if (i == 0)
				{
					pts[i].name = "Point_Start";
				}
				else if (i == pts.Length - 1)
				{
					pts[i].name = "Point_End";
				}
				else
				{
					pts[i].name = "Point_" + i;
				}
			}
		}

		public int GetSplinePointIndex(AudioSplinePoint point)
		{
			for (int i = 0; i < pts.Length; i++)
			{
				if (pts[i] == point)
				{
					return i;
				}
			}
			return -1;
		}

		public void BakeSpline(float resolution)
		{
			int num = 5000;
			if (resolution > float.MinValue)
			{
				num = Math.Min(num, (int)(1f / resolution) + 1);
			}
			_bakedPoints = new Vector3[num];
			float num2 = 0f;
			for (int i = 0; i < num; i++)
			{
				_bakedPoints[i] = Interp(num2);
				num2 += resolution;
			}
		}

		public Vector3 GetNearestPointToListener(Vector3 position, ref float t, GameObject go)
		{
			float num = float.MaxValue;
			int num2 = 0;
			for (int i = 0; i < _bakedPoints.Length; i++)
			{
				float num3 = Vector3.Distance(go.transform.TransformPoint(_bakedPoints[i]), position);
				if (num3 < num)
				{
					num2 = i;
					num = num3;
				}
			}
			Vector3 result = _prevBakedPoint = Vector3.Lerp(go.transform.TransformPoint(_bakedPoints[num2]), _prevBakedPoint, 0.9f);
			t = (float)num2 / (float)_bakedPoints.Length;
			return result;
		}

		public void UpdateEventTriggers(Vector3 position)
		{
			for (int i = 0; i < pts.Length; i++)
			{
				AudioSplinePoint audioSplinePoint = pts[i];
				if (audioSplinePoint.HasEventTrigger())
				{
					float num = Vector3.Distance(audioSplinePoint.transform.position, position);
					if (num < audioSplinePoint._radius && !audioSplinePoint.IsEntered())
					{
						audioSplinePoint.OnEnter();
					}
					else if (num > audioSplinePoint._radius && audioSplinePoint.IsEntered())
					{
						audioSplinePoint.OnExit();
					}
				}
			}
		}

		public Vector3 Interp(float t)
		{
			if (pts.Length >= 3)
			{
				int num = pts.Length - 3;
				int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
				float num3 = t * (float)num - (float)num2;
				if (pts[num2] != null)
				{
					Vector3 localPosition = pts[num2].transform.localPosition;
					Vector3 localPosition2 = pts[num2 + 1].transform.localPosition;
					Vector3 localPosition3 = pts[num2 + 2].transform.localPosition;
					Vector3 localPosition4 = pts[num2 + 3].transform.localPosition;
					return 0.5f * ((-localPosition + 3f * localPosition2 - 3f * localPosition3 + localPosition4) * (num3 * num3 * num3) + (2f * localPosition - 5f * localPosition2 + 4f * localPosition3 - localPosition4) * (num3 * num3) + (-localPosition + localPosition3) * num3 + 2f * localPosition2);
				}
			}
			return default(Vector3);
		}

		public Vector3 Velocity(float t)
		{
			if (pts.Length >= 3)
			{
				int num = pts.Length - 3;
				int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
				float num3 = t * (float)num - (float)num2;
				if (pts[num2] != null)
				{
					Vector3 localPosition = pts[num2].transform.localPosition;
					Vector3 localPosition2 = pts[num2 + 1].transform.localPosition;
					Vector3 localPosition3 = pts[num2 + 2].transform.localPosition;
					Vector3 localPosition4 = pts[num2 + 3].transform.localPosition;
					return 1.5f * (-localPosition + 3f * localPosition2 - 3f * localPosition3 + localPosition4) * (num3 * num3) + (2f * localPosition - 5f * localPosition2 + 4f * localPosition3 - localPosition4) * num3 + 0.5f * localPosition3 - 0.5f * localPosition;
				}
			}
			return default(Vector3);
		}

		public void GizmoDraw(float t, Color splineColor, GameObject gameObject)
		{
			if (pts != null)
			{
				Gizmos.color = splineColor;
				Vector3 to = Interp(0f);
				for (int i = 1; i <= 60; i++)
				{
					float t2 = (float)i / 60f;
					Vector3 vector = gameObject.transform.TransformPoint(Interp(t2));
					Gizmos.DrawLine(vector, to);
					to = vector;
				}
			}
		}
	}
}
