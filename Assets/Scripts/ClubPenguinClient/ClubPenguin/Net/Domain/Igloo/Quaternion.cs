using UnityEngine;

namespace ClubPenguin.Net.Domain.Igloo
{
	public struct Quaternion
	{
		public float x;

		public float y;

		public float z;

		public float w;

		public UnityEngine.Quaternion ToUnityQaternion()
		{
			UnityEngine.Quaternion result = default(UnityEngine.Quaternion);
			result.w = w;
			result.x = x;
			result.y = y;
			result.z = z;
			return result;
		}
	}
}
