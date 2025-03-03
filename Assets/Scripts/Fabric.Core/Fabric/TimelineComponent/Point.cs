using System;
using UnityEngine;

namespace Fabric.TimelineComponent
{
	[Serializable]
	public class Point
	{
		[SerializeField]
		public float _x;

		[SerializeField]
		public float _y;

		[SerializeField]
		public CurveTypes _curveType = CurveTypes.Linear;

		[SerializeField]
		public bool _locked;

		public Point()
		{
			_x = 0f;
			_y = 0f;
			_curveType = CurveTypes.Linear;
		}

		public static Point Alloc(float x, float y, CurveTypes curveType)
		{
			Point point = new Point();
			point._x = x;
			point._y = y;
			point._curveType = curveType;
			return point;
		}

		public static Point Alloc(float x, float y)
		{
			Point point = new Point();
			point._x = x;
			point._y = y;
			point._curveType = CurveTypes.Linear;
			return point;
		}
	}
}
