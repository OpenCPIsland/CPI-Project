using UnityEngine;

namespace ClubPenguin.Net.Domain.Scene
{
	public struct Quaternion
	{
		public float x;

		public float y;

		public float z;

		public float w;

		public static Quaternion FromUnityQuaternion(UnityEngine.Quaternion uq)
		{
			Quaternion result = default(Quaternion);
			result.x = uq.x;
			result.y = uq.y;
			result.z = uq.z;
			result.w = uq.w;
			return result;
		}

		public static UnityEngine.Quaternion ToUnityQuaternion(Quaternion q)
		{
			UnityEngine.Quaternion result = default(UnityEngine.Quaternion);
			result.w = q.w;
			result.x = q.x;
			result.y = q.y;
			result.z = q.z;
			return result;
		}
	}
}
