using System;

namespace Fabric
{
	[Serializable]
	public class Envelope
	{
		public Point[] _points;

		public void Sort()
		{
		}

		public float Calculate_x(float _y)
		{
			int i;
			for (i = 0; i < _points.Length && _points[i]._y < _y; i++)
			{
			}
			Point point = new Point();
			if (i == 0)
			{
				if (_points.Length >= 2)
				{
					point = _points[0];
				}
				else
				{
					point._x = 0f;
					point._y = 0f;
				}
			}
			else
			{
				point = _points[i - 1];
			}
			Point point2 = new Point();
			if (i >= _points.Length)
			{
				if (_points.Length >= 2)
				{
					point2 = _points[_points.Length - 1];
				}
				else
				{
					point._x = 0f;
					point._y = 0f;
				}
			}
			else
			{
				point2 = _points[i];
			}
			float num = 0.5f;
			if (point2._y != point._y)
			{
				num = (_y - point._y) / (point2._y - point._y);
			}
			return point._x + (point2._x - point._x) * num;
		}

		public float Calculate_y(float _x)
		{
			int i;
			for (i = 0; i < _points.Length && _points[i]._x < _x; i++)
			{
			}
			Point point = new Point();
			if (i == 0)
			{
				if (_points.Length >= 2)
				{
					point = _points[0];
				}
				else
				{
					point._x = 0f;
					point._y = 0f;
				}
			}
			else
			{
				point = _points[i - 1];
			}
			Point point2 = new Point();
			if (i >= _points.Length)
			{
				if (_points.Length >= 2)
				{
					point2 = _points[_points.Length - 1];
				}
				else
				{
					point._x = 0f;
					point._y = 0f;
				}
			}
			else
			{
				point2 = _points[i];
			}
			float num = 0.5f;
			if (point2._x != point._x)
			{
				num = (_x - point._x) / (point2._x - point._x);
			}
			return point._y + (point2._y - point._y) * num;
		}
	}
}
