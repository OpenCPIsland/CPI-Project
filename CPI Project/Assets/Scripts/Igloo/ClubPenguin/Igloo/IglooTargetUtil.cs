using ClubPenguin.ObjectManipulation;
using UnityEngine;

namespace ClubPenguin.Igloo
{
	public static class IglooTargetUtil
	{
		public static Vector3 GetBaseOfTargetPoint(ManipulatableObject mo, float minTargetDistance)
		{
			CollidableObject component = mo.GetComponent<CollidableObject>();
			if (component == null)
			{
				return Vector3.zero;
			}
			Bounds bounds = component.GetBounds();
			float num = bounds.size.z * 0.5f;
			if (num < minTargetDistance)
			{
				num = minTargetDistance;
			}
			Vector3 center = bounds.center;
			center.z += num;
			return center;
		}
	}
}
