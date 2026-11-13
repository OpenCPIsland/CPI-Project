using UnityEngine;

namespace ClubPenguin.Net.Utils
{
	public struct NVector3
	{
		public double x;

		public double y;

		public double z;

		public NVector3(Vector3 source)
		{
			x = source.x;
			y = source.y;
			z = source.z;
		}

		public Vector3 toVector3()
		{
			return new Vector3((float)x, (float)y, (float)z);
		}
	}
}
