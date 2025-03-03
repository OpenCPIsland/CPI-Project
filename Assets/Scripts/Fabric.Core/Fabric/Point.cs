using System;

namespace Fabric
{
	[Serializable]
	public class Point
	{
		public float _x;

		public float _y;

		public Point(float x, float y)
		{
			_x = x;
			_y = y;
		}

		public Point()
		{
			_x = 0f;
			_y = 0f;
		}
	}
}
