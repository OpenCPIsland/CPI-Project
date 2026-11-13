using UnityEngine;

namespace Disney.Kelowna.Common
{
	public static class TransformExtensions
	{
		public static void LookAway(this Transform transform, Vector3 worldPosition)
		{
			transform.rotation = Quaternion.LookRotation(transform.position - worldPosition);
		}

		public static Transform FindParent(this Transform transform, string name)
		{
			if (transform.parent == null)
			{
				return null;
			}
			if (transform.parent.name == name)
			{
				return transform.parent;
			}
			return transform.parent.FindParent(name);
		}
	}
}
