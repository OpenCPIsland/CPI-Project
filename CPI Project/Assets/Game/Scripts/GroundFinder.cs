using UnityEngine;

public static class GroundFinder
{
	public static Vector3 GetGroundFromSky(Vector3 start, Vector3 xzDestination, float maxDistance = 40f)
	{
		Vector3 start2 = xzDestination;
		start2.y = 10000f;
		return GetGround(start2, maxDistance);
	}

	public static Vector3 GetGroundFromArc(Vector3 start, Vector3 midPoint, Vector3 destination, float maxDistance = 40f)
	{
		Vector3 vector = midPoint - start;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		int layerMask = getLayerMask();
		RaycastHit hitInfo;
		if (Physics.Raycast(start, normalized, out hitInfo, magnitude, layerMask))
		{
			float magnitude2 = (hitInfo.point - start).magnitude;
			Vector3 origin = start + normalized * magnitude2 * 0.9f;
			if (Physics.Raycast(origin, Vector3.down, out hitInfo))
			{
				return hitInfo.point;
			}
		}
		Vector3 normalized2 = (destination - midPoint).normalized;
		if (Physics.Raycast(midPoint, normalized2, out hitInfo, layerMask))
		{
			return hitInfo.point;
		}
		return destination;
	}

	public static Vector3 GetGround(Vector3 start, float maxDistance = 40f)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(start, Vector3.down, out hitInfo, getLayerMask()) && (hitInfo.point - start).magnitude <= maxDistance)
		{
			return hitInfo.point;
		}
		return start;
	}

	private static int getLayerMask()
	{
		return 1 << LayerMask.NameToLayer("TerrainBarrier");
	}
}
