using UnityEngine;

namespace MixAvatar
{
	public struct BindPoseTransform
	{
		public readonly Vector3 LocalPosition;

		public readonly Quaternion LocalRotation;

		public BindPoseTransform(Vector3 localPosition, Quaternion localRotation)
		{
			LocalPosition = localPosition;
			LocalRotation = localRotation;
		}
	}
}
