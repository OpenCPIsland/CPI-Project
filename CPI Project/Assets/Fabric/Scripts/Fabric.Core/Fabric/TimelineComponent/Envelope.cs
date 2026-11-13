using System;
using UnityEngine;

namespace Fabric.TimelineComponent
{
	[Serializable]
	public class Envelope
	{
		[Serializable]
		private struct LocalPoint
		{
			[SerializeField]
			public float _x;

			[SerializeField]
			public float _y;

			[SerializeField]
			public CurveTypes _curveType;
		}

		[SerializeField]
		public Point[] _points;

		[SerializeField]
		public int _selectedPoint;

		private LocalPoint[] point = new LocalPoint[4];

		private LocalPoint ab = default(LocalPoint);

		private LocalPoint bc = default(LocalPoint);

		private LocalPoint cd = default(LocalPoint);

		private LocalPoint abbc = default(LocalPoint);

		private LocalPoint bccd = default(LocalPoint);

		public bool PointIntersects(float pointX, float pointY)
		{
			float num = Calculate_y(pointX);
			if (Mathf.Abs(num - pointY) < 0.05f)
			{
				return true;
			}
			return false;
		}

		public void StretchX(float factor)
		{
			for (int i = 0; i < _points.Length; i++)
			{
				_points[i]._x *= factor;
				AudioTools.Limit(ref _points[i]._x, _points[0]._x, _points[_points.Length - 1]._x);
			}
		}

		public int GetPointIndex(float x)
		{
			int i;
			for (i = 0; i < _points.Length && _points[i]._x < x; i++)
			{
			}
			return i;
		}

		public float Calculate_y(float _x)
		{
			int i;
			for (i = 0; i < _points.Length && _points[i]._x < _x; i++)
			{
			}
			LocalPoint localPoint = default(LocalPoint);
			if (i == 0)
			{
				if (_points.Length >= 2)
				{
					CopyPointToLocalPoint(_points[0], ref localPoint);
				}
				else
				{
					localPoint._x = 0f;
					localPoint._y = 0f;
				}
			}
			else
			{
				CopyPointToLocalPoint(_points[i - 1], ref localPoint);
			}
			LocalPoint localPoint2 = default(LocalPoint);
			if (i >= _points.Length)
			{
				if (_points.Length >= 2)
				{
					CopyPointToLocalPoint(_points[i - 1], ref localPoint2);
				}
				else
				{
					localPoint._x = 0f;
					localPoint._y = 0f;
				}
			}
			else
			{
				CopyPointToLocalPoint(_points[i], ref localPoint2);
			}
			float num = 0.5f;
			if (localPoint2._x != localPoint._x)
			{
				num = (_x - localPoint._x) / (localPoint2._x - localPoint._x);
			}
			if (localPoint._y > localPoint2._y)
			{
				num = 1f - num;
				num = 1f - CalculateValueByType(localPoint._curveType, 1f, num);
			}
			else
			{
				num = CalculateValueByType(localPoint._curveType, 1f, num);
			}
			return localPoint._y + (localPoint2._y - localPoint._y) * num;
		}

		public float CalculatePoints(Point start, Point end, float _x)
		{
			float num = 0.5f;
			if (end._x != start._x)
			{
				num = (_x - start._x) / (end._x - start._x);
			}
			if (start._y > end._y)
			{
				num = 1f - num;
				num = 1f - CalculateValueByType(start._curveType, 1f, num);
			}
			else
			{
				num = CalculateValueByType(start._curveType, 1f, num);
			}
			return start._y + (end._y - start._y) * num;
		}

		private float CalculateValueByType(CurveTypes curveType, float value, float t)
		{
			float result = 0f;
			switch (curveType)
			{
			case CurveTypes.Bezier:
			{
				point[0] = default(LocalPoint);
				point[1] = default(LocalPoint);
				point[2] = default(LocalPoint);
				point[3] = default(LocalPoint);
				point[0]._x = 0f;
				point[0]._y = 1f;
				point[3]._x = 1f;
				point[3]._y = 1f - value;
				point[1]._x = 1f;
				point[1]._y = 1f;
				point[2]._x = 0f;
				point[2]._y = 1f - value;
				LocalPoint dest = default(LocalPoint);
				Bezier(ref dest, point, t);
				result = 1f - dest._y;
				break;
			}
			case CurveTypes.Linear:
				result = t * value;
				break;
			case CurveTypes.Raised:
				result = (float)Math.Sqrt(t) * value;
				break;
			case CurveTypes.Flat:
			{
				float num4 = t - 0.5f;
				result = 0.5f - 4f * (num4 * num4 * num4);
				break;
			}
			case CurveTypes.Log:
			{
				float num = 1024f;
				float num2 = (float)Math.Log(1f + num) / (float)Math.Log(2.0);
				float num3 = -1f;
				result = ((float)Math.Pow(2.0, num2 * t) + num3) / num;
				break;
			}
			}
			return result;
		}

		private void CopyPointToLocalPoint(Point point, ref LocalPoint localPoint)
		{
			localPoint._x = point._x;
			localPoint._y = point._y;
			localPoint._curveType = point._curveType;
		}

		private void Bezier(ref LocalPoint dest, LocalPoint[] point, float t)
		{
			ab._x = point[0]._x + (point[1]._x - point[0]._x) * t;
			ab._y = point[0]._y + (point[1]._y - point[0]._y) * t;
			bc._x = point[1]._x + (point[2]._x - point[1]._x) * t;
			bc._y = point[1]._y + (point[2]._y - point[1]._y) * t;
			cd._x = point[2]._x + (point[3]._x - point[2]._x) * t;
			cd._y = point[2]._y + (point[3]._y - point[2]._y) * t;
			abbc._x = ab._x + (bc._x - ab._x) * t;
			abbc._y = ab._y + (bc._y - ab._y) * t;
			bccd._x = bc._x + (cd._x - bc._x) * t;
			bccd._y = bc._y + (cd._y - bc._y) * t;
			dest._x = abbc._x + (bccd._x - abbc._x) * t;
			dest._y = abbc._y + (bccd._y - abbc._y) * t;
		}
	}
}
